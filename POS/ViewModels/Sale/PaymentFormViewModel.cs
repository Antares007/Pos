using POS.ServerApi;

namespace POS.ViewModels.Sale
{
    public class PaymentFormViewModel
    {
        public TLink Link { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}