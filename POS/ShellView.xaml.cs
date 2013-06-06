using System.Windows;
using System.Windows.Controls;
using POS.Utils;

namespace POS
{
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
            //var customerWindow = new CustomerWindow() {DataContext = this, Left = 1024, Top = 0};
            //this.Loaded += (sender, args) => customerWindow.Show();
            this.Closed += (sender, args) => PosBootstrapper._msg.Publish(new ApplicationClosingEvent());
        }
    }
}