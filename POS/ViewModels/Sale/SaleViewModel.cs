using System;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using CsQuery;
using POS.ServerApi;
using POS.Utils;

namespace POS.ViewModels.Sale
{
    [Title("gakidva")]
    public class SaleViewModel : Screen, IUpdatableScreen
    {
        public SaleViewModel()
        {
            Items = new BindableCollection<ItemViewModel>();
        }

        public BindableCollection<ItemViewModel> Items { get; set; }

        public string Total { get; set; }

        public string Change { get; set; }

        public TForm PaymentForm { get; set; }

        public TForm AddItem { get; set; }

        public TForm Submit { get; set; }

        public void BarcodeTextBoxKeyDown(KeyEventArgs args)
        {
            if (args.Key == Key.Return || args.Key == Key.Enter)
                AddItem.Execute(this);
        }

        public void UpdateUi(ScreenActivationContext sac)
        {
            var cq = sac.Cqq;
            Total = cq.GetText(".misagebiTankha"); 
            Change = cq.GetText(".gasacemiTankha");
            PaymentForm = cq.GetForm("gadakhda");
            AddItem = cq.GetForm("produktisDamateba");
            Submit = cq.GetForm("dasruleba");
            Items.Clear();
            Func<CQQ, ItemViewModel> createItem = (q) => 
                new ItemViewModel()
                {
                    Name = q.GetText(".dasakheleba"),
                    Ean = q.GetText(".ean"),
                    Reference = q.GetText(".ref"),
                    Quantity = q.GetText(".raodenoba"),
                    UnitPrice = q.GetText(".fasi"), 
                    TotalPrice = q.GetText(".jami"), 
                    Photo = q.GetAttr("img", "src")
                };
            Items.AddRange(cq.All(".yvela .produkti", createItem));
        }
    }
}