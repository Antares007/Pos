using System.Configuration;
using Caliburn.Micro;
using POS.ServerApi;
using POS.Utils;
using System.Linq;

namespace POS.ViewModels
{
    public class ProductViewModel:Screen
    {
        
        public string Barcode { get; set; }
        public string Name { get; set; }
        public TForm Price { get; set; }
        public string Eans { get; set; }
        public BindableCollection<ReferenceViewModel> References { get; set; }
        public BindableCollection<string> Photos { get; set; }
        private string _activePhoto;
        public string ActivePhoto
        {
            get
            {
                return string.IsNullOrEmpty(_activePhoto) ? Photos.FirstOrDefault() : _activePhoto;
            }
            set { _activePhoto = value; }
        }

        public void PrintPrice()
        {
            new LabelPrinter(ConfigurationManager.AppSettings["zebra"])
                    .PrintPrice(decimal.Parse(Price["fasi"]));
        }

        public string Id { get; set; }

        public string ExcelFileName { get; set; }
    }
}