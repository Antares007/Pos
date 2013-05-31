using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using POS.Utils;

namespace POS.ViewModels
{
    public class MainViewModel : Screen
    {
        public string Barcode { get; set; }
        public string Price { get; set; }
        public void Print()
        {
            new LabelPrinter("ZDesigner LP 2824 Plus (ZPL)")
            .PrintPriceAndBarcode(decimal.Parse(Price), Barcode);
        }
    }
}