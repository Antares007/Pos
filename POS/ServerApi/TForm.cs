using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;

namespace POS.ServerApi
{
    public class TForm : IDictionary<string, string>, ICommand
    {
        private readonly HttpMethod _method;
        private readonly IEnumerable<string> _requiredFields;
        private readonly string _action;
        private readonly Action<HttpRequestMessage> _requestAction;

        private readonly IDictionary<string, string> _inner;

        public TForm(HttpMethod method, IEnumerable<string> requiredFields, string href,Dictionary<string,string> inner, Action<HttpRequestMessage> requestAction)
        {
            _method = method;
            _requiredFields = requiredFields;
            _action = href;
            _inner = inner;
            _requestAction = requestAction;
        }

        public bool CanExecute(object parameter)
        {
            return _requiredFields.All(s => !string.IsNullOrWhiteSpace(this[s]));
        }

        public void Execute(object parameter)
        {
            var requestUri = _action;
            var req = new HttpRequestMessage();
            if (_method == HttpMethod.Get)
                requestUri += "?" + string.Join("&", this.Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)));
            else
            {
                var content = new FormUrlEncodedContent(this.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)));
                req.Content = content;
            }
            req.Method = _method;
            req.RequestUri = new Uri(requestUri);
            _requestAction(req);
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        public void Add(KeyValuePair<string, string> item)
        {
            _inner.Add(item);
        }

        public void Clear()
        {
            _inner.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return _inner.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            _inner.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            return _inner.Remove(item);
        }

        public int Count
        {
            get { return _inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return _inner.IsReadOnly; }
        }

        public bool ContainsKey(string key)
        {
            return _inner.ContainsKey(key);
        }

        public void Add(string key, string value)
        {
            _inner.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _inner.Remove(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return _inner.TryGetValue(key, out value);
        }

        public string this[string key]
        {
            get { return _inner[key]; }
            set { _inner[key] = value;
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public ICollection<string> Keys
        {
            get { return _inner.Keys; }
        }

        public ICollection<string> Values
        {
            get { return _inner.Values; }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}