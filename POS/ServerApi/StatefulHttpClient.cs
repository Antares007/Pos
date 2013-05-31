using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace POS.ServerApi
{
    public class StatefulHttpClient
    {
        private readonly HttpClient _client;
        private readonly CookieContainer _cookie;

        public StatefulHttpClient(HttpClient client, CookieContainer cookie)
        {
            _client = client;
            _cookie = cookie;
        }
        public async Task<StatefulResponse> SendAsync(HttpRequestMessage req)
        {
            req.Headers.Add("Cookie", GetCookies(req.RequestUri));
            var res = await _client.SendAsync(req);
            SetCookies(res);
            return ToStatefulResponse(res);
        }

        private StatefulResponse ToStatefulResponse(HttpResponseMessage res)
        {
            return new StatefulResponse() { Message = res, Client = this };
        }

        public async Task<StatefulResponse> GetAsync(string uriString)
        {
            var uri = new Uri(uriString);
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            req.Headers.Add("Cookie", GetCookies(uri));
            var res = await _client.SendAsync(req);
            SetCookies(res);
            return ToStatefulResponse(res);
        }

        private void SetCookies(HttpResponseMessage res)
        {
            IEnumerable<string> cookies;
            res.Headers.TryGetValues("Set-Cookie", out cookies);
            if (cookies != null)
                foreach (var cookie in cookies)
                    _cookie.SetCookies(res.RequestMessage.RequestUri, cookie);
        }

        private IEnumerable<string> GetCookies(Uri uri)
        {
            return _cookie
               .GetCookies(uri)
               .Cast<Cookie>()
               .Where(c => !c.Expired)
               .Select(c => c.ToString());
        }
    }
}