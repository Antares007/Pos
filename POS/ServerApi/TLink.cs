using System;
using System.Net.Http;
using System.Windows.Input;

namespace POS.ServerApi
{
    public class TLink : ICommand
    {
        private readonly string _url;
        private readonly Action<HttpRequestMessage> _transitLink;

        public TLink(string url, Action<HttpRequestMessage> transitLink)
        {
            _url = url;
            _transitLink = transitLink;
        }

        public bool CanExecute(object parameter)
        {
            return !string.IsNullOrEmpty(_url);
        }

        public async void Execute(object parameter)
        {
            _transitLink(new HttpRequestMessage(HttpMethod.Get, new Uri(_url)));
            OnExecuted.Invoke();
        }

        public event EventHandler CanExecuteChanged;
        public event Action OnExecuted = delegate {};
    }
}