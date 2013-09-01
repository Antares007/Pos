using System.Threading;
using System.Windows;
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
                    FocusTextBox();
                }
            };
        }

        private void FocusTextBox()
        {
            ItemCode.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(delegate()
            {
                ItemCode.Focus();
            }));
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            var text = ItemCode.Text;
            if (text.Length > 0)
            {
                ItemCode.SetValue(TextBox.TextProperty, text.Remove(text.Length - 1, 1));
                FocusTextBox();
                ItemCode.CaretIndex = ItemCode.Text.Length;
            }
        }
    }
}