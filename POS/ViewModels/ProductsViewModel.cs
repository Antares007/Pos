using Caliburn.Micro;
using POS.ServerApi;

namespace POS.ViewModels
{
    public class ProductsViewModel : Screen
    {
        public BindableCollection<ProductItemViewModel> Items { get; set; }

        public TForm Search { get; set; }
    }
}