using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace POS.Views.Sale
{
    /// <summary>
    /// Interaction logic for PaymentInput.xaml
    /// </summary>
    public partial class PaymentInput : UserControl
    {
        public PaymentInput()
        {
            InitializeComponent();
            this.IsVisibleChanged += (sender, args) =>
                {
                    if ((bool)args.NewValue)
                    {
                        Input.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(delegate()
                        {
                            Input.Focus();
                        }));
                    }
                };
        }
    }
}
