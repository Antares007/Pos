using Caliburn.Micro;
using POS.ServerApi;

namespace POS.ViewModels.Sale
{
    public class AmountItemViewModel : Screen
    {
        private string _viewState;
        public TLink Link { get; set; }
        //public string Name { get; set; }
        public string StringResourceKey { get; set; }
        public string Value { get; set; }
        public string Value2 { get; set; }
        public string ViewState
        {
            get { return _viewState; }
            set { _viewState = value; NotifyOfPropertyChange(() => ViewState); }
        }
        public bool HasLink { get { return Link != null; } }
        public bool HasValue2
        {
            get { return Value != Value2 && Value2 != null; }
        }
    }
}