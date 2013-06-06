using System;
using System.Windows.Controls;
using POS.Utils;

namespace POS.Views.Sale
{
    public partial class SaleView : UserControl
    {
        public SaleView()
        {
            InitializeComponent();
            var customerWindow = new CustomerWindow() { DataContext = ItemsGrid, Left = 1024, Top = 0 };
            this.Loaded += (sender, args) =>
                {
                    PosBootstrapper._msg.GetStream<ApplicationClosingEvent>()
                                   .Subscribe(x => customerWindow.Close());
                    customerWindow.Show();
                };
        }
    }
}