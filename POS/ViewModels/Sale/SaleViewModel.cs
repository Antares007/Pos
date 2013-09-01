using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using Caliburn.Micro;
using POS.ServerApi;
using POS.Utils;
using POS.ViewModels.Sale.Printing;
using UserControl = System.Windows.Controls.UserControl;

namespace POS.ViewModels.Sale
{
    [Title("gakidva")]
    public class SaleViewModel : HyperMediaViewModel
    {
        private string _total;
        private string _change;
        private TForm _addItem;
        private TLink _submit;
        private string _viewState;
        private TForm _paymentForm;
        private readonly ReceiptPrinter _receiptPrinter = new ReceiptPrinter();
        private TForm _increaseQuantity;
        private TForm _decreaseQuantity;
        private bool _isToolVisible;
        private TForm _reset;

        public TForm IncreaseQuantity
        {
            get { return _increaseQuantity; }
            set { _increaseQuantity = value; NotifyOfPropertyChange(() => IncreaseQuantity); }
        }

        public TForm DecreaseQuantity
        {
            get { return _decreaseQuantity; }
            set { _decreaseQuantity = value; NotifyOfPropertyChange(() => DecreaseQuantity); }
        }

        public string ViewState
        {
            get { return _viewState; }
            set { _viewState = value; NotifyOfPropertyChange(() => ViewState); }
        }
        public BindableCollection<AmountItemViewModel> PaymentForms { get; set; }
        public BindableCollection<ItemViewModel> Items { get; set; }
        public TForm PaymentForm
        {
            get { return _paymentForm; }
            set { _paymentForm = value; NotifyOfPropertyChange(() => PaymentForm); }
        }

        public string Total
        {
            get { return _total; }
            set { _total = value; NotifyOfPropertyChange(() => Total); }
        }

        public string Change
        {
            get { return _change; }
            set { _change = value; NotifyOfPropertyChange(() => Change); }
        }

        public TForm AddItem
        {
            get { return _addItem; }
            set { _addItem = value; NotifyOfPropertyChange(() => AddItem); }
        }

        public TLink Submit
        {
            get { return _submit; }
            set { _submit = value; NotifyOfPropertyChange(() => Submit); }
        }

        public SaleViewModel()
        {
            Items = new BindableCollection<ItemViewModel>();
            IsToolVisible = false;
            PaymentForms = new BindableCollection<AmountItemViewModel>();
            var pco = Observable.FromEventPattern<PropertyChangedEventArgs>(this, "PropertyChanged")
                                .Select(x => x.EventArgs)
                                .Where(x => x.PropertyName == "ViewState")
                                .Select(x => this.ViewState);
            var normal = pco.Where(x => x == "Normal").Select(x => Unit.Default);
            var other = pco.Where(x => x != "Normal").Select(x => Unit.Default);
            Scanner.ObservableKeys
                   .SkipUntil(pco.Where(x => x == "Payment"))
                   .Where(c => char.IsNumber(c) || (int)c == 27)
                   .TakeUntil(pco.Where(x => x != "Payment"))
                   .Scan("", (s, c) => c == 27 ? "" : s + c)
                   .Repeat()
                   .ObserveOnDispatcher()
                   .Subscribe(x =>
                       {
                           if (string.IsNullOrWhiteSpace(x))
                               PaymentForm["mnishvneloba"] = "0.00";
                           else
                               PaymentForm["mnishvneloba"] = String.Format("{0:0.00}", (decimal.Parse(x) / 100));
                           NotifyOfPropertyChange(() => PaymentForm);
                       });

            Scanner.ObservableKeys
                .Where(x => !char.IsWhiteSpace(x))
                .SkipUntil(normal)
                .Publish(o => o.Buffer(() => o.Throttle(TimeSpan.FromMilliseconds(500))))
                .Select(x => string.Join("", x))
                .TakeUntil(other)
                .Repeat()
                .ObserveOnDispatcher()
                .Subscribe(x =>
                    {
                        AddItem["kodi"] = x;
                        AddItem.Execute(null);
                    });
        }
        public override void UpdateUi(ScreenActivationContext sac)
        {
            var jq = sac.Jq;
            jq.IsTitle("cheki", Print);
            jq.IsTitle("gakidva", UpdateSaleViewModel);
            jq.IsTitle("gadakhdisForma", ShowPaymentForm);
        }

        private async void Print(Jq cq)
        {
            await _receiptPrinter.PrintReceipt(cq);
            cq.GetForm("gamocera").Execute(null);
        }

        private void ShowPaymentForm(Jq cq)
        {
            PaymentForm = cq.GetForm("gadakhdisForma");
            ViewState = "Payment";
        }
        public void ShowAddItem()
        {
            ViewState = "AddItem";
        }
        public void ShowSettings()
        {
            ViewState = "SettingsShown";
        }
        private void UpdateSaleViewModel(Jq cq)
        {
            PaymentForms.Clear();
            PaymentForms.AddRange(cq.All("gadakhdisFormebi",
                cqq => new AmountItemViewModel()
            {
                Link = cqq.GetLink("gadakhdisForma"),
                StringResourceKey = PaymentName2ResKeyConverter.Convert(cqq.GetText("dasakheleba")),
                Value = cqq.GetText("tankha"),
                ViewState = "Plus"
            }));
            PaymentForms.Last().ViewState = "Equals";
            PaymentForms.Add(new AmountItemViewModel()
                {
                    StringResourceKey = PaymentName2ResKeyConverter.Convert("სულ გადახდილი"),
                    Value = cq.GetText("sulMigebuliTankha"),
                    ViewState = "Minus"
                });
            PaymentForms.Add(new AmountItemViewModel()
                {
                    StringResourceKey = PaymentName2ResKeyConverter.Convert("სულ გადასახდელი"),
                    Value = cq.GetText("misagebiTankha"),
                    Value2 = cq.GetText("fasdaklebuliMisagebiTankha"),
                    ViewState = "Equals"
                });
            PaymentForms.Add(new AmountItemViewModel()
            {
                StringResourceKey = PaymentName2ResKeyConverter.Convert("ხურდა"),
                Value = cq.GetText("gasacemiTankha"),
                ViewState = "Normal"
            });
            AddItem = cq.GetForm("produktisDamateba");
            Reset = cq.GetForm("gaukmeba");
            Submit = cq.GetLink("cheki");
            Submit.OnExecuted += () => IsToolVisible = false;
            Func<Jq, ItemViewModel> createItem = (q) =>
                new ItemViewModel()
                {
                    Id = q.GetText("id"),
                    Version = q.GetText("versia"),
                    Name = q.GetText("ref") + " " + q.GetText("dasakheleba"),
                    Ean = q.GetText("ean"),
                    Reference = q.GetText("ref"),
                    Quantity = q.GetText("raodenoba"),
                    UnitPrice = q.GetText("fasdaklebuliFasi"),
                    TotalPrice = q.GetText("jami"),
                    DiscountedTotalPrice = q.GetText("fasdaklebuliJami"),
                    Photo = q.GetText("img"),
                    Increase = q.GetForm("momateba"),
                    Decrease = q.GetForm("mokleba")
                };
            var itemViewModels = cq.All("produktebi", createItem).ToList();
            var newItems = (from first in itemViewModels
                            from second in Items.Where(x => x.Id == first.Id).DefaultIfEmpty()
                            where second == null
                            select first).ToList();
            var changedItems = (from first in itemViewModels
                                from second in Items.Where(x => x.Id == first.Id)
                                where second.Version != first.Version
                                select new { First = first, Second = second }).ToList();
            var deletedItems = (from first in Items
                                from second in itemViewModels.Where(x => x.Id == first.Id).DefaultIfEmpty()
                                where second == null
                                select first).ToList();
            deletedItems.ForEach(x => Items.Remove(Items.First(e => e.Id == x.Id)));
            changedItems.ForEach(x => x.Second.Update(x.First));
            newItems.ForEach(x => Items.Insert(0, x));
            ViewState = "Normal";
        }
        protected override bool DoCanHandle(ScreenActivationContext sac)
        {
            return sac.Jq.IsTitle("gadakhdisForma") || sac.Jq.IsTitle("cheki");

        }

        public void ItemMouseDown(object item)
        {
            var itemViewModel = (ItemViewModel)item;
            var view = (UserControl)itemViewModel.GetView();
            var saleView = ((UserControl)this.GetView());
            var translatePoint = view.TranslatePoint(new Point(0, 0), saleView);
            IsToolVisible = true;
            ToolX = translatePoint.X;
            ToolY = translatePoint.Y;
            NotifyOfPropertyChange(() => ToolX);
            NotifyOfPropertyChange(() => ToolY);
            var storyboard = saleView.TryFindResource("MoveTool") as Storyboard;
            IncreaseQuantity = itemViewModel.Increase;
            DecreaseQuantity = itemViewModel.Decrease;
            if (storyboard != null)
                storyboard.Begin();
        }

        public double ToolX { get; set; }
        public double ToolY { get; set; }
        public bool IsToolVisible
        {
            get { return _isToolVisible; }
            set { _isToolVisible = value; NotifyOfPropertyChange(() => IsToolVisible); }
        }
        public void KeyboardOverlayMouseDown(RoutedEventArgs args)
        {
            var grid = args.OriginalSource as Grid;
            if (grid == null || grid.Name != "KeyboardOverlay") return;
            if (ViewState == "AddItem")
            {
                AddItem["kodi"] = string.Empty;
                NotifyOfPropertyChange(() => AddItem);
            }
            ViewState = "Normal";
        }

        public TForm Reset
        {
            get { return _reset; }
            set { _reset = value; NotifyOfPropertyChange(() => Reset); }
        }

        #region Temp
        public void PrintPrice(ItemViewModel item)
        {
            new LabelPrinter(ConfigurationManager.AppSettings["zebra"])
                    .PrintPrice(decimal.Parse(item.UnitPrice));
        }
        #endregion
    }
}