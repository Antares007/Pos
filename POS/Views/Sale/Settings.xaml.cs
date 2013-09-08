using System;
using System.Collections.Generic;
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

namespace POS.Views.Sale
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
            var culture = App.CurrentCulture.Name;
            var currentLangRadio = Languages.Children.OfType<RadioButton>().FirstOrDefault(x => x.Tag.ToString() == culture);
            if (currentLangRadio != null)
                currentLangRadio.IsChecked = true;
            Languages.Children.OfType<RadioButton>().ToList().ForEach(button =>
                button.Checked += (s, args) =>
                {
                    var radio = (RadioButton)s;
                    if (radio != null)
                    {
                        App.SelectCulture(radio.Tag.ToString());
                    }
                });
        }
    }
}
