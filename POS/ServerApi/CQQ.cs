using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using CsQuery;

namespace POS.ServerApi
{
    public class CQQ
    {
        private readonly Action<HttpRequestMessage> _action;
        public CQQ(CQ cq, Action<HttpRequestMessage> action)
        {
            _cq = cq;
            _action = action;
        }

        private readonly CQ _cq;

        public TForm GetForm(string selector)
        {
            return MakeTForm(selector, _action);
        }
        public string GetText(string selector)
        {
            return _cq[selector].Text();
        }
        public string GetText()
        {
            return _cq.Text();
        }
        public IEnumerable<T> All<T>(string selector, Func<CQQ, T> action)
        {
            return (from i in _cq[selector]
                    let q = CQ.Create(i)
                    let item = action(new CQQ(q, _action))
                    select item);
        }

        public string GetAttr(string selector, string attr)
        {
            return _cq[selector].Attr(attr);
        }
        public string GetAttr(string attr)
        {
            return _cq.Attr(attr);
        }
        private TForm MakeTForm(string selector, Action<HttpRequestMessage> action)
        {
            var data = _cq[selector + " [name]"].Select(x => new
            {
                Name = x.Attributes["name"],
                Value = x.Attributes["value"],
                Required = x.Attributes["required"] != null
            }).ToList();
            var form = _cq[selector].First();
            var href = form.Attr("action");
            var m = form.Attr("method");
            var method = m == "post" ? HttpMethod.Post : HttpMethod.Get;
            return new TForm(method, data.Where(x => x.Required).Select(x => x.Name), href, data.ToDictionary(x => x.Name, x => x.Value), action);
        }

        public TLink GetLink(string selector)
        {
            return new TLink(GetAttr(selector, "href"), _action);
        }
        public TLink GetLink()
        {
            return new TLink(_cq.Attr("href"), _action);
        }

        public bool IsTitle(string title,Action<CQQ> action = null)
        {
            var result = this.GetText("head title") == title;
            if (result && action!=null)
                action(this);
            return result;
        }
    }
}