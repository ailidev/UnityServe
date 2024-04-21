using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using System;
using Avalonia;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Avalonia.Media;
using GenHTTP.Engine;
using GenHTTP.Modules.IO;
using GenHTTP.Modules.Practices;
using GenHTTP.Modules.StaticWebsites;
using System.Net;
using GenHTTP.Modules.StaticWebsites.Provider;
using GenHTTP.Api.Infrastructure;
using GenHTTP.Modules.IO.FileSystem;
using GenHTTP.Modules.Layouting;
using GenHTTP.Modules.Layouting.Provider;
using GenHTTPResources = GenHTTP.Modules.IO.Resources;
using GenHTTP.Modules.Compression;
using Avalonia.Controls.ApplicationLifetimes;
using GenHTTP.Modules.Compression.Providers;
using GenHTTP.Api.Protocol;
using System.IO.Compression;

namespace UnityServe.Views
{
    public partial class MainWindow : Window
    {
        bool _serverStatus = false;
        public readonly IServerHost _server = Host.Create();
        ushort _defaultPort = 8090;

        public MainWindow()
        {
            InitializeComponent();
            Port.Text = _defaultPort.ToString();
        }

        void ToggleServer(object? sender, RoutedEventArgs args)
        {
            switch (_serverStatus)
            {
                case false:
                    if (GamePath.Text == string.Empty) return;
                    _defaultPort = Port.Text != string.Empty ? ushort.Parse(Port.Text.Trim()) : _defaultPort;

                    LayoutBuilder layout = Layout.Create()
                        .Add(CompressedContent.Default());
                    DirectoryTreeBuilder resources = ResourceTree.FromDirectory(GamePath.Text.Trim());
                    StaticWebsiteBuilder website = StaticWebsite.From(resources);
                    layout.Add(GenHTTPResources.From(resources));
                    _server
                        .Handler(website)
                        .Bind(IPAddress.Parse("127.0.0.1"), _defaultPort)
                        .Defaults()
                        .Compression(CompressedContent.Default().Add(new BrotliCompression()))
                        .Console()
                    #if DEBUG
                        .Development()
                    #endif
                        .Start();

                    Port.Text = _defaultPort.ToString();
                    ToggleServerButton.Content = "Stop Server";
                    ToggleServerButton.Background = Brush.Parse("#d33c3c");

                    Process.Start(new ProcessStartInfo($"http://127.0.0.1:{_defaultPort}") { UseShellExecute = true });
                    _serverStatus = !_serverStatus;
                    break;
                case true:
                    _server.Stop();
                    ToggleServerButton.Content = "Start Server";
                    ToggleServerButton.Background = Brush.Parse("#3cd37e");
                    _serverStatus = !_serverStatus;
                    break;
            }
        }

        async void OpenGamePath(object? sender, RoutedEventArgs args)
        {
            string selectedFolder = await OpenFolderDialogAsync(this);
            GamePath.Text = selectedFolder;
        }

        /// <summary>
        /// Open file dialog.
        /// </summary>
        /// <param name="topLevelVisual"></param>
        /// <param name="fileExtensions"></param>
        /// <returns></returns>
        public static async Task<string> OpenFolderDialogAsync(Visual topLevelVisual)
        {
            // Get top level from the current control. Alternatively, you can use Window reference instead.
            TopLevel? topLevel = GetTopLevel(topLevelVisual);
            IReadOnlyList<IStorageFolder> selectedFolder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Open Game Path",
                SuggestedStartLocation = topLevel.StorageProvider.TryGetFolderFromPathAsync(AppContext.BaseDirectory).Result,
            });

            return selectedFolder.Count > 0 ? selectedFolder[0].Path.LocalPath : string.Empty;
        }
    }

    public class BrotliCompression : ICompressionAlgorithm
    {
        public string Name => "brotli";
        public Priority Priority => Priority.Medium;

        public IResponseContent Compress(IResponseContent content, CompressionLevel level)
        {
            return new CompressedResponseContent(content, (target) => new BrotliStream(target, level, false));
        }
    }
}
