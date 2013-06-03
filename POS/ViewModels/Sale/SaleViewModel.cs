using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Caliburn.Micro;
using CsQuery;
using POS.ServerApi;
using POS.Utils;

namespace POS.ViewModels.Sale
{
    [Title("gakidva")]
    public class SaleViewModel : HyperMediaViewModel
    {
        private string _total;
        private string _change;
        private TForm _addItem;
        private TForm _submit;
        private string _viewState;
        private TForm _paymentForm;

        public string ViewState
        {
            get { return _viewState; }
            set { _viewState = value; NotifyOfPropertyChange(() => ViewState); }
        }
        public BindableCollection<PaymentFormViewModel> PaymentForms { get; set; }
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

        public TForm Submit
        {
            get { return _submit; }
            set { _submit = value; NotifyOfPropertyChange(() => Submit); }
        }

        public SaleViewModel()
        {
            Items = new BindableCollection<ItemViewModel>();
            PaymentForms = new BindableCollection<PaymentFormViewModel>();
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
            cq.IsTitle("gakidva", UpdateSaleViewModel);
            cq.IsTitle("gadakhdisForma", ShowPaymentForm);
        }

        private void ShowPaymentForm(CQQ cq)
        {
            PaymentForm = cq.GetForm("gadakhdisForma");
            ViewState = "Payment";
        }

        private void UpdateSaleViewModel(CQQ cq)
        {
            Total = cq.GetText(".misagebiTankha");
            Change = cq.GetText(".gasacemiTankha");
            PaymentForms.Clear();
            PaymentForms.AddRange(cq.All(".yvela .gadakhdisForma", cqq => new PaymentFormViewModel()
            {
                Link = cqq.GetLink("a"),
                Name = cqq.GetText(".dasakheleba"),
                Value = cqq.GetText("a")
            }));
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
            ViewState = "Normal";
        }

        protected override bool DoCanHandle(ScreenActivationContext sac)
        {
            return sac.Cqq.IsTitle("gadakhdisForma");
        }

    }
}