using Caliburn.Micro;
using POS.ServerApi;

namespace POS.ViewModels
{
    public class ProductItemViewModel
    {
        public string Photo { get; set; }
        public string Name { get; set; }

        public string Eans { get; set; }

        public TLink Open { get; set; }
    }
}