namespace ZTR.Framework.Security
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IWebServiceClientBase
    {
        // Have to use Func<HttpRequestMessge, Task> instead of Action<HttpRequestMessage> since Action<..> won't wait the method to finish
        public Func<HttpRequestMessage, Task> RequestHandler { get; set; }

        public Func<HttpClient, Task> ClientHandler { get; set; }
    }
}
