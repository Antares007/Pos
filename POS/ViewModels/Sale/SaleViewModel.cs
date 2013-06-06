using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Caliburn.Micro;
using POS.ServerApi;
using POS.ViewModels.Sale.Printing;

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
                .Where(x => !char.IsWhiteSpace(x))
                .SkipUntil(normal)
                .Publish(o => o.Buffer(() => o.Throttle(TimeSpan.FromMilliseconds(500))))
                .Select(x => string.Join("", x))
                .TakeUntil(other)
                .Repeat()
                .Subscribe(x =>
                    {
                        AddItem["kodi"] = x;
                        AddItem.Execute(null);
                    });
        }
        public override void UpdateUi(ScreenActivationContext sac)
        {
            var cq = sac.Cqq;
            cq.IsTitle("cheki", Print);
            cq.IsTitle("gakidva", UpdateSaleViewModel);
            cq.IsTitle("gadakhdisForma", ShowPaymentForm);
        }

        private void Print(CQQ cq)
        {
            _receiptPrinter.PrintReceipt(cq);
            cq.GetForm("#gamocera").Execute(null);
        }

        private void ShowPaymentForm(CQQ cq)
        {
            PaymentForm = cq.GetForm("#gadakhdisForma");
            ViewState = "Payment";
        }

        private void UpdateSaleViewModel(CQQ cq)
        {
            PaymentForms.Clear();
            PaymentForms.AddRange(cq.All(".yvela .gadakhdisForma",
                cqq => new AmountItemViewModel()
            {
                Link = cqq.GetLink("a"),
                Name = cqq.GetText(".dasakheleba"),
                Value = cqq.GetText("a"),
                ViewState = "Plus"
            }));
            PaymentForms.Last().ViewState = "Equals";
            PaymentForms.Add(new AmountItemViewModel()
                {
                    Name = "სულ გადახდილი",
                    Value = cq.GetText(".sulMigebuliTankha"),
                    ViewState = "Minus"
                });
            PaymentForms.Add(new AmountItemViewModel()
                {
                    Name = "სულ გადასახდელი",
                    Value = cq.GetText(".misagebiTankha"),
                    ViewState = "Equals"
                });
            PaymentForms.Add(new AmountItemViewModel()
            {
                Name = "ხურდა",
                Value = cq.GetText(".gasacemiTankha"),
                ViewState = "Normal"
            });
            AddItem = cq.GetForm("#produktisDamateba");
            Submit = cq.GetLink("a[rel='cheki']");
            Submit.OnExecuted += () => IsToolVisible = false;
            Func<CQQ, ItemViewModel> createItem = (q) =>
                new ItemViewModel()
                {
                    Id = q.GetText(".id"),
                    Version = q.GetText(".versia"),
                    Name = q.GetText(".dasakheleba"),
                    Ean = q.GetText(".ean"),
                    Reference = q.GetText(".ref"),
                    Quantity = q.GetText(".raodenoba"),
                    UnitPrice = q.GetText(".fasi"),
                    TotalPrice = q.GetText(".jami"),
                    Photo = q.GetAttr("img", "src"),
                    Increase = q.GetForm(".momateba"),
                    Decrease = q.GetForm(".mokleba")
                };
            var itemViewModels = cq.All(".yvela .produkti", createItem).ToList();
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
            return sac.Cqq.IsTitle("gadakhdisForma") || sac.Cqq.IsTitle("cheki");

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
    }
}