using Caliburn.Micro;
using POS.ServerApi;

namespace POS.ViewModels
{
    [Title("produktebi")]
    public class ProductsViewModel : Screen, IUpdatableScreen
    {
        public BindableCollection<ProductItemViewModel> Items { get; set; }

        public TForm Search { get; set; }

        public ProductsViewModel()
        {
            Items = new BindableCollection<ProductItemViewModel>();
        }
        public void UpdateUi(ScreenActivationContext sac)
        {
            Search = sac.Cqq.GetForm("dzebna");
            Items.Clear();
            Items.AddRange(sac.Cqq.All("#produktebi .yvela .produkti",
                cqq => new ProductItemViewModel()
                    {
                        Name = cqq.GetText(".dasakheleba"),
                        Eans = cqq.GetText(".eans"),
                        Photo = cqq.GetAttr(".image","src"),
                        Open = cqq.GetLink("a")
                    }));
        }
    }
}