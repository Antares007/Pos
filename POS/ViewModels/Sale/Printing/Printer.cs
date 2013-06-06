using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;

namespace POS.ViewModels.Sale.Printing
{
    public class Printer
    {
        private readonly string _printerName;

        public Printer(string printerName)
        {
            _printerName = printerName;
        }
        public void PrintReceipt(UserControl uc)
        {
            var dialog = new PrintDialog();
            var printer = new PrintServer().GetPrintQueues()
                            .FirstOrDefault(x => x.Name == _printerName);
            if (printer != null)
                dialog.PrintQueue = printer;
            uc.Measure(new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
            uc.Arrange(new Rect(new Point(0, 0), uc.DesiredSize));
            dialog.PrintVisual(uc, "receipt");
        }
    }
}