using System.Net.Http;

namespace POS
{
    public interface INavigator
    {
        void Navigate(HttpRequestMessage context);
        void GoBack();
        bool CanGoBack { get;}
    }
}