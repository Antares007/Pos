using Caliburn.Micro;

namespace POS.ViewModels
{
    public class PriceListItemViewModel : Screen
    {
        public string ItemName { get; set; }

        public string Photo { get; set; }

        public string Skus { get; set; }

        public string Eans { get; set; }

        public string Price { get; set; }
    }
}