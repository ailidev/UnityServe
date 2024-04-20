using System.IO;
using System.Text;
using WatsonWebserver;
using WatsonWebserver.Core;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using System;
using Avalonia;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Input;

namespace UnityServe.Views
{
    public partial class MainWindow : Window
    {
        bool _serverStatus = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        void ToggleServer(object? sender, RoutedEventArgs args)
        {
            int defaultPort = 8080;
            Webserver server = SetupServer("localhost", defaultPort);
            switch (_serverStatus)
            {
                case false:
                    if (Port.Text != string.Empty)
                        server.Settings.Port = int.Parse(Port.Text.Trim());
                    else
                    {
                        server.Settings.Port = defaultPort;
                        Port.Text = defaultPort.ToString();
                    }
                    server.Start();
                    ToggleServerButton.Content = "Stop Server";
                    Process.Start(server.DefaultPages.Pages[0].Content);
                    break;
                case true:
                    server.Stop();
                    ToggleServerButton.Content = "Start Server";
                    break;
            }
        }

        static Webserver SetupServer(string hostname, int port)
        {
            WebserverSettings settings = new(hostname, port);
            return new(settings, DefaultRoute);
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

            return selectedFolder[0].Path.LocalPath;
        }

        static async Task DefaultRoute(HttpContextBase ctx) =>
            await ctx.Response.Send("Hello from the default route!");
    }
}