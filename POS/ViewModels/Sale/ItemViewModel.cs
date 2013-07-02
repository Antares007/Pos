using Caliburn.Micro;
using POS.ServerApi;

namespace POS.ViewModels.Sale
{
    public class ItemViewModel : Screen
    {
        private string _quantity;
        private string _unitPrice;
        private string _totalPrice;
        private TForm _increase;
        private TForm _decrease;
        private string _version;
        private string _discountedTotalPrice;
        private bool _hasDiscount;

        public ItemViewModel()
        {

        }
        public string Name { get; set; }

        public string Ean { get; set; }

        public string Reference { get; set; }

        public string Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value; NotifyOfPropertyChange(() => Quantity);
            }
        }

        public string UnitPrice
        {
            get { return _unitPrice; }
            set { _unitPrice = value; NotifyOfPropertyChange(() => UnitPrice); }
        }

        public string TotalPrice
        {
            get { return _totalPrice; }
            set { _totalPrice = value; NotifyOfPropertyChange(() => TotalPrice); }
        }

        public string Photo { get; set; }

        public TForm Increase
        {
            get { return _increase; }
            set { _increase = value; NotifyOfPropertyChange(() => Increase); }
        }

        public TForm Decrease
        {
            get { return _decrease; }
            set { _decrease = value; NotifyOfPropertyChange(() => Decrease); }
        }

        public string Id { get; set; }

        public string Version
        {
            get { return _version; }
            set { _version = value; NotifyOfPropertyChange(() => Version); }
        }

        public string DiscountedTotalPrice
        {
            get { return _discountedTotalPrice; }
            set { _discountedTotalPrice = value; NotifyOfPropertyChange(() => DiscountedTotalPrice); }
        }
        public bool HasDiscount
        {
            get { return TotalPrice != DiscountedTotalPrice; }
        }

        public void Update(ItemViewModel n)
        {
            Quantity = n.Quantity;
            UnitPrice = n.UnitPrice;
            TotalPrice = n.TotalPrice;
            Increase = n.Increase;
            Decrease = n.Decrease;
            Version = n.Version;
            DiscountedTotalPrice = n.DiscountedTotalPrice;
            NotifyOfPropertyChange(() => HasDiscount);
        }
    }
}