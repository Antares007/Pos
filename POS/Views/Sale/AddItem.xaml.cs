using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace POS.Views.Sale
{
    public partial class AddItem : UserControl
    {
        public AddItem()
        {
            InitializeComponent();
            this.IsVisibleChanged += (sender, args) =>
            {
                if ((bool)args.NewValue)
                {
                    ItemCode.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(delegate()
                    {
                        ItemCode.Focus();
                    }));
                }
            };
        }
    }
}