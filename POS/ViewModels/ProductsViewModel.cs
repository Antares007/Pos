using Caliburn.Micro;
using POS.ServerApi;

namespace POS.ViewModels
{
    [Title("produktebi")]
    public class ProductsViewModel : HyperMediaViewModel
    {
        public BindableCollection<ProductItemViewModel> Items { get; set; }

        public TForm Search { get; set; }

        public ProductsViewModel()
        {
            Items = new BindableCollection<ProductItemViewModel>();
        }
        public override void UpdateUi(ScreenActivationContext sac)
        {
            Search = sac.Jq.GetForm("dzebna");
            Items.Clear();
            Items.AddRange(sac.Jq.All("produktebi",
                cqq => new ProductItemViewModel()
                    {
                        Name = cqq.GetText("dasakheleba"),
                        Eans = cqq.GetText("eans"),
                        Photo = cqq.GetText("img"),
                        Open = cqq.GetLink("self")
                    }));
        }
    }
}