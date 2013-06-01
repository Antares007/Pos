using System.Windows;
using System.Windows.Controls;

namespace POS
{
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
            this.Loaded += (sender, args) =>
                {
                    new CustomerWindow() {DataContext = this,Left = 1024,Top = 0}.Show();
                };
        }
    }
}