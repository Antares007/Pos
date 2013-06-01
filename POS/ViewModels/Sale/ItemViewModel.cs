using Caliburn.Micro;

namespace POS.ViewModels.Sale
{
    public class ItemViewModel:Screen
    {
        public ItemViewModel()
        {
            
        }
        public string Name { get; set; }

        public string Ean { get; set; }

        public string Reference { get; set; }

        public string Quantity { get; set; }

        public string UnitPrice { get; set; }

        public string TotalPrice { get; set; }

        public string Photo { get; set; }
    }
}