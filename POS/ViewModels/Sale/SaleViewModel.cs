using System.Windows.Input;
using Caliburn.Micro;
using POS.ServerApi;

namespace POS.ViewModels.Sale
{
    public class SaleViewModel : Screen
    {
        public BindableCollection<ItemViewModel> Items { get; set; }

        public string Total { get; set; }

        public string Change { get; set; }

        public TForm PaymentForm { get; set; }

        public TForm AddItem { get; set; }

        public TForm Submit { get; set; }

        public void BarcodeTextBoxKeyDown(KeyEventArgs args)
        {
            if (args.Key == Key.Return || args.Key == Key.Enter)
                AddItem.Execute(this);
        }
    }
}