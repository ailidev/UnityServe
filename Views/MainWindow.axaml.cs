using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using UnityServe.Utils;
using MB.Web;

namespace UnityServe.Views
{
    public partial class MainWindow : Window
    {
        SimpleWebServer _server;
        bool _serverStatus = false;
        string _defaultDirectory;
        ushort _defaultPort = 8090;

        public MainWindow()
        {
            InitializeComponent();

            _defaultDirectory = SaveLoadLastPath.LoadLastOpenedFile();
            GamePath.Text = _defaultDirectory;
            Port.Text = _defaultPort.ToString();
        }

        // public static int Main(string[] args)
        // {
        //     bool flag = args.Length < 1 || args.Length > 4;
        //     if (flag)
        //     {
        //         Console.WriteLine("usage: SimpleWebServer.exe source_directory [port] [pid]");
        //         return 1;
        //     }
        //     basePath = args[0];
        //     bool flag2 = args.Length >= 2;
        //     if (flag2)
        //     {
        //         port = int.Parse(args[1]);
        //     }
        //     bool flag3 = args.Length >= 3;
        //     if (flag3)
        //     {
        //         int pid = int.Parse(args[2]);
        //         monitorProcess = Process.GetProcessById(pid);
        //     }
        //     Thread serverThread = new Thread(new ThreadStart(RunServer));
        //     serverThread.Start();
        //     for (; ; )
        //     {
        //         bool flag4 = monitorProcess != null && monitorProcess.HasExited;
        //         if (flag4)
        //         {
        //             Console.WriteLine("Associated process has died. Exiting server.");
        //             Environment.Exit(0);
        //         }
        //         Thread.Sleep(100);
        //     }
        // }

        // private static HttpListener RunServer(string basePath, ushort port)
        // {
        //     HttpListener server = new();
        //     server.Prefixes.Add("http://localhost:" + port.ToString() + "/");
        //     Console.WriteLine("Starting web server at http://localhost:" + port.ToString() + "/");
        //     server.Start();
        //     // for (; ; )
        //     // {
        //         try
        //         {
        //             HttpListenerContext context = server.GetContext();
        //             HttpListenerResponse response = context.Response;
        //             response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        //             response.Headers["Pragma"] = "no-cache";
        //             response.Headers["Expires"] = "0";
        //             string path = Uri.UnescapeDataString(context.Request.Url.LocalPath);
        //             Console.WriteLine("Handling request: " + path);
        //             bool flag = path == "/";
        //             if (flag)
        //             {
        //                 path = "/index.html";
        //             }
        //             bool flag2 =
        //                 Path.GetExtension(path) == ".html"
        //                 || path.EndsWith(".js")
        //                 || path.EndsWith(".js.gz")
        //                 || path.EndsWith(".js.br");
        //             if (flag2)
        //             {
        //                 response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";
        //                 response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";
        //                 response.Headers["Cross-Origin-Resource-Policy"] = "cross-origin";
        //             }
        //             bool flag3 = Path.GetExtension(path) == ".gz";
        //             if (flag3)
        //             {
        //                 response.AddHeader("Content-Encoding", "gzip");
        //             }
        //             else
        //             {
        //                 bool flag4 = Path.GetExtension(path) == ".br";
        //                 if (flag4)
        //                 {
        //                     response.AddHeader("Content-Encoding", "br");
        //                 }
        //             }
        //             bool flag5 = context.Request.Headers.Get("Range") != null;
        //             if (flag5)
        //             {
        //                 response.AddHeader("Accept-Ranges", "bytes");
        //             }
        //             bool flag6 =
        //                 path.EndsWith(".wasm")
        //                 || path.EndsWith(".wasm.gz")
        //                 || path.EndsWith(".wasm.br");
        //             if (flag6)
        //             {
        //                 response.ContentType = "application/wasm";
        //             }
        //             else
        //             {
        //                 bool flag7 =
        //                     path.EndsWith(".js")
        //                     || path.EndsWith(".js.gz")
        //                     || path.EndsWith(".js.br");
        //                 if (flag7)
        //                 {
        //                     response.ContentType = "application/javascript";
        //                 }
        //                 else
        //                 {
        //                     bool flag8 = path.EndsWith(".data.gz");
        //                     if (flag8)
        //                     {
        //                         response.ContentType = "application/gzip";
        //                     }
        //                     else
        //                     {
        //                         bool flag9 = path.EndsWith(".data") || path.EndsWith(".data.br");
        //                         if (flag9)
        //                         {
        //                             response.ContentType = "application/octet-stream";
        //                         }
        //                     }
        //                 }
        //             }
        //             string page = basePath + path;
        //             string msg = null;
        //             bool flag10 = !context.Request.IsLocal;
        //             if (flag10)
        //             {
        //                 Console.WriteLine("Forbidden.");
        //                 msg = "<HTML><BODY>403 Forbidden.</BODY></HTML>";
        //                 response.StatusCode = 403;
        //             }
        //             else
        //             {
        //                 bool flag11 = !File.Exists(page);
        //                 if (flag11)
        //                 {
        //                     Console.WriteLine("Not found.");
        //                     msg = "<HTML><BODY>404 Not found.</BODY></HTML>";
        //                     response.StatusCode = 404;
        //                 }
        //                 else
        //                 {
        //                     FileStream fileStream = File.Open(page, FileMode.Open);
        //                     BinaryReader reader = new BinaryReader(fileStream);
        //                     try
        //                     {
        //                         response.ContentLength64 = fileStream.Length;
        //                         byte[] buffer = reader.ReadBytes(4096);
        //                         while (buffer.Length != 0)
        //                         {
        //                             response.OutputStream.Write(buffer, 0, buffer.Length);
        //                             buffer = reader.ReadBytes(4096);
        //                         }
        //                     }
        //                     catch (Exception e)
        //                     {
        //                         string str = "Caught Exception sending file: ";
        //                         Exception ex = e;
        //                         Console.WriteLine(str + ((ex != null) ? ex.ToString() : null));
        //                     }
        //                     reader.Close();
        //                 }
        //             }
        //             bool flag12 = msg != null;
        //             if (flag12)
        //             {
        //                 byte[] buffer2 = Encoding.UTF8.GetBytes(msg);
        //                 response.ContentLength64 = (long)buffer2.Length;
        //                 response.OutputStream.Write(buffer2, 0, buffer2.Length);
        //             }
        //             response.Close();
        //         }
        //         catch (Exception e2)
        //         {
        //             string str2 = "Caught Exception handling request: ";
        //             Exception ex2 = e2;
        //             Console.WriteLine(str2 + ((ex2 != null) ? ex2.ToString() : null));
        //         }

        //     return server;
        //     // }
        // }

        void ToggleServer(object? sender, RoutedEventArgs args)
        {
            switch (_serverStatus)
            {
                case false:
                    _defaultDirectory = GamePath.Text.Trim();
                    _defaultPort =
                        Port.Text != string.Empty ? ushort.Parse(Port.Text.Trim()) : _defaultPort;

                    if (_defaultDirectory == string.Empty)
                        return;

                    ToggleServerButton.Content = "Stop Server";
                    ToggleServerButton.Background = Brush.Parse("#d33c3c");

                    _server = new($"http://localhost:{_defaultPort}/", _defaultDirectory);
                    _server.Start();
                    Process.Start(
                        new ProcessStartInfo($"http://localhost:{_defaultPort}")
                        {
                            UseShellExecute = true
                        }
                    );
                    _serverStatus = true;
                    break;
                case true:
                    ToggleServerButton.Content = "Start Server";
                    ToggleServerButton.Background = Brush.Parse("#3cd37e");
                    _server.Stop();
                    _serverStatus = false;
                    break;
            }
        }

        async void OpenGamePath(object? sender, RoutedEventArgs args)
        {
            SaveLoadLastPath.LoadLastOpenedFile();
            string selectedFolder = await OpenFolderDialogAsync(this);
            SaveLoadLastPath.SaveLastOpenedFile(selectedFolder);

            _defaultDirectory = selectedFolder;
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
            IReadOnlyList<IStorageFolder> selectedFolder =
                await topLevel.StorageProvider.OpenFolderPickerAsync(
                    new FolderPickerOpenOptions { Title = "Open Game Path" }
                );

            return selectedFolder.Count > 0 ? selectedFolder[0].Path.LocalPath : string.Empty;
        }
    }
}
