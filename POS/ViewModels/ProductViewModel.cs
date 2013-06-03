using System.Configuration;
using Caliburn.Micro;
using POS.ServerApi;
using POS.Utils;
using System.Linq;

namespace POS.ViewModels
{
    [Title("produkti")]
    public class ProductViewModel : HyperMediaViewModel
    {
        private string _activePhoto;
        private TForm _price;
        private string _eans;
        private string _name;
        private string _barcode;
        private string _id;
        private string _excelFileName;

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public string Barcode
        {
            get { return _barcode; }
            set
            {
                _barcode = value;
                NotifyOfPropertyChange(() => Barcode);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public TForm Price
        {
            get { return _price; }
            set
            {
                _price = value;
                NotifyOfPropertyChange(() => Price);
            }
        }

        public string Eans
        {
            get { return _eans; }
            set
            {
                _eans = value;
                NotifyOfPropertyChange(() => Eans);
            }
        }

        public string ActivePhoto
        {
            get
            {
                return string.IsNullOrEmpty(_activePhoto) ? Photos.FirstOrDefault() : _activePhoto;
            }
            set { _activePhoto = value; }
        }

        public string ExcelFileName
        {
            get { return _excelFileName; }
            set
            {
                _excelFileName = value;
                NotifyOfPropertyChange(() => ExcelFileName);
            }
        }

        public BindableCollection<ReferenceViewModel> References { get; set; }
        public BindableCollection<string> Photos { get; set; }

        public ProductViewModel()
        {
            References = new BindableCollection<ReferenceViewModel>();
            Photos = new BindableCollection<string>();
        }


        public void PrintPrice()
        {
            new LabelPrinter(ConfigurationManager.AppSettings["zebra"])
                    .PrintPrice(decimal.Parse(Price["fasi"]));
        }

        public override void UpdateUi(ScreenActivationContext sac)
        {
            var cqq = sac.Cqq;
            Id = cqq.GetText(".partiebi .partia .id");
            Barcode = cqq.GetText("a[rel='self']");
            Name = cqq.GetText(".dasakheleba");
            ExcelFileName = cqq.GetText(".partiebi .partia .shenishvna");
            Price = cqq.GetForm("fasisShecvla");
            Eans = cqq.GetText(".eans .ean");
            Photos.Clear();
            Photos.AddRange(cqq.All("img", cqq1 => cqq1.GetAttr("src")));
            NotifyOfPropertyChange(() => ActivePhoto);
            References.Clear();
            References.AddRange(cqq.All(".eans .ref", cqq1 => new ReferenceViewModel()
                {
                    Barcode = cqq1.GetText(),
                    Open = cqq1.GetLink(),
                }));
        }
    }
}