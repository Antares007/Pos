using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ServiceStack.Text;
using ServiceStack.Text.Json;

namespace POS.ServerApi
{
    public class StatefulHttpClient : INavigator
    {
        private Uri _lastUri;
        private readonly HttpClient _client;
        private readonly CookieContainer _cookie;
        private readonly IShell _shell;
        private readonly Stack<Uri> _stack = new Stack<Uri>();
        public StatefulHttpClient(HttpClient client, CookieContainer cookie, IShell shell)
        {
            _client = client;
            _cookie = cookie;
            _shell = shell;
        }
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage req)
        {
            req.Headers.Add("Cookie", GetCookies(req.RequestUri));
            req.Headers.Add("Accept", "application/json");
            var res = await _client.SendAsync(req);
            SetCookies(res);
            return res;
        }

        public async Task<HttpResponseMessage> GetAsync(string uriString)
        {
            var uri = new Uri(uriString);
            var req = new HttpRequestMessage(HttpMethod.Get, uri);
            req.Headers.Add("Cookie", GetCookies(uri));
            var res = await _client.SendAsync(req);
            SetCookies(res);
            return res;
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

        public async void Navigate(HttpRequestMessage request)
        {
            var response = await this.SendAsync(request);

            var sc = response.StatusCode;
            var redirected = sc == HttpStatusCode.TemporaryRedirect ||
                sc == HttpStatusCode.Moved || sc == HttpStatusCode.MovedPermanently ||
                sc == HttpStatusCode.Found;
            if (redirected)
            {
                var req = new HttpRequestMessage(HttpMethod.Get,
                                                 new Uri(response.RequestMessage.RequestUri,
                                                         response.Headers.Location));
                Navigate(req);
                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            var jq = new Jq((JsonObject)JsonReader<JsonObject>.Parse(content), Navigate);
            if (jq.GetText("title") == "Home")
            {
                TForm form = jq.GetForm("airchiePosi");
                form["posisNomeri"] = ConfigurationManager.AppSettings["pos"];
                form.Execute(null);
                return;
            }

            var url = request.Method != HttpMethod.Get ? response.Headers.Location : request.RequestUri;

            if (_lastUri != null && (_stack.Count == 0 || _stack.Peek() != request.RequestUri))
                _stack.Push(_lastUri);
            _lastUri = _lastUri == url ? null : url;
            _shell.Show(new ScreenActivationContext(jq, Navigate));
        }

        public void GoBack()
        {
            if (_stack.Count != 0)
            {
                _lastUri = null;
                Navigate(new HttpRequestMessage(HttpMethod.Get, _stack.Pop()));
            }
        }

        public bool CanGoBack { get { return _stack.Count != 0; } }
    }
}