using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HttpListenerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Listen().GetAwaiter().GetResult();
            Console.WriteLine();
        }

        static async Task Listen()
        {
            string redirectURI = $@"http://{IPAddress.Loopback}:{GetRandomUnusedPort()}/";

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectURI);

            Console.WriteLine($"[{redirectURI}]: Listening...");

            http.Start();
            while (true)
            {
                var context = await http.GetContextAsync();
                var response = context.Response;
                response.Headers.Clear();
                response.SendChunked = false;
                response.StatusCode = 200;
                response.Headers.Add("Server", String.Empty);
                response.Headers.Add("Date", String.Empty);
                var content = String.IsNullOrWhiteSpace(context.Request.Url.PathAndQuery)
                    || context.Request.Url.PathAndQuery.Equals("/", StringComparison.InvariantCultureIgnoreCase)
                        ? "Hello World"
                        : $"Hello, {context.Request.Url.PathAndQuery}";
                var encodedString = Encoding.UTF8.GetBytes(content);
                response.ContentLength64 = Encoding.UTF8.GetByteCount(content);
                response.OutputStream.Write(encodedString);
                response.OutputStream.Flush();
                response.Close();
            }
        }


        static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
