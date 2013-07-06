using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ServiceStack.Text;

namespace POS.ServerApi
{
    public class Jq
    {
        private readonly JsonObject _o;
        private readonly Action<HttpRequestMessage> _action;

        public Jq(JsonObject o, Action<HttpRequestMessage> action)
        {
            _o = o;
            _action = action;
        }

        internal string GetText(string p)
        {
            return _o.Get<string>(p);
        }

        public TForm GetForm(string form)
        {
            var template = _o.ArrayObjects("templates").FirstOrDefault(x => x.Get<string>("rel") == form);
            if (template == null)
                return TForm.Disabled;
            var method = template.Get<string>("method") == "post" ? HttpMethod.Post : HttpMethod.Get;
            var href = template.Get<string>("href");
            var fields = template.ArrayObjects("data")
                                 .ToDictionary(x => x.Get<string>("name"), x => x.Get<string>("value"));
            return new TForm(method, new string[0], href, fields, _action);
        }

        public bool IsTitle(string title, Action<Jq> action = null)
        {
            var result = GetText("title") == title;
            if (result && action != null)
                action(this);
            return result;
        }

        public IEnumerable<T> All<T>(string selector, Func<Jq, T> action)
        {
            return (from i in _o.ArrayObjects(selector)
                    let q = new Jq(i, _action)
                    let item = action(q)
                    select item);
        }

        public TLink GetLink(string gadakhdisforma)
        {
            var link = _o.ArrayObjects("links").FirstOrDefault(x => x.Get<string>("rel") == gadakhdisforma);
            if (null == link)
                return new TLink(null, _action);
            var url = link.Get<string>("href");
            return new TLink(url, _action);
        }
    }
}