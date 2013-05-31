using System.Net.Http;

namespace POS.ServerApi
{
    public class StatefulResponse
    {
        public HttpResponseMessage Message { get; set; }
        public StatefulHttpClient Client { get; set; }
    }
}