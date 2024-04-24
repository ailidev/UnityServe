using System;
using System.IO;
using System.Net;

namespace MB.Web
{
    /// <summary>
    /// Simple web server. Serves the contents of a specified
    /// directory to an address plus port.
    /// </summary>
    public class SimpleWebServer
    {
        private readonly string _rootDir;
        private readonly SimpleHttpListener _listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleWebServer"/> class.
        /// Does not start the server automatically.
        /// If no file name is provided, index.html is returned (if it exists).
        /// </summary>
        /// <param name="prefix">The htpp address prefix.</param>
        /// <param name="rootDir">The root directory to serve the files from.</param>
        public SimpleWebServer(string prefix, string rootDir)
        {
            _rootDir = rootDir;

            _listener = new SimpleHttpListener(prefix) { OnReceivedRequest = OnReceivedRequest, };
        }

        /// <summary>
        /// Starts the web server.
        /// </summary>
        public void Start()
        {
            _listener.Start();
        }

        /// <summary>
        /// Stop the web server.
        /// </summary> <summary>
        ///
        /// </summary>
        public void Stop()
        {
            _listener.Stop();
        }

        private void OnReceivedRequest(HttpListenerRequest request, HttpListenerResponse response)
        {
            // string? fileName = request.Url.AbsolutePath;
            string? fileName = Uri.UnescapeDataString(request.Url.LocalPath);

            Console.WriteLine("request: {0}", fileName);

            fileName = fileName?[1..];

            // if no filename is given, use index.html
            if (string.IsNullOrEmpty(fileName))
                fileName = "index.html";
            else
            {
                string[]? parts = fileName.Split('/');
                if (parts.Length > 0 && string.IsNullOrEmpty(parts[^1]))
                    fileName = Path.Combine(fileName, "index.html");
            }

            fileName = Path.Combine(_rootDir, fileName);

            //! TODO: Fix Content-Type header
            response.ContentType = MimeHelper.GetMimeType(fileName);

            response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            response.Headers["Pragma"] = "no-cache";
            response.Headers["Expires"] = "0";

            if (
                Path.GetExtension(fileName) == ".html"
                || fileName.EndsWith(".js")
                || fileName.EndsWith(".js.gz")
                || fileName.EndsWith(".js.br")
            )
            {
                response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";
                response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";
                response.Headers["Cross-Origin-Resource-Policy"] = "cross-origin";
            }

            if (Path.GetExtension(fileName) == ".gz")
                response.AddHeader("Content-Encoding", "gzip");
            if (Path.GetExtension(fileName) == ".br")
                response.AddHeader("Content-Encoding", "br");

            FileStream? fileStream = null;

            try
            {
                fileStream = new FileStream(fileName, FileMode.Open);

                byte[]? buffer = new byte[1024 * 16];
                int nbytes;

                while ((nbytes = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    response.OutputStream.Write(buffer, 0, nbytes);
            }
            catch (Exception e)
            {
                Console.WriteLine($"error serving file {e}");
                response.StatusCode = 404;
            }
            finally
            {
                fileStream?.Close();
                response.OutputStream.Close();
            }
        }
    }
}
