using System.Configuration;
using System.Globalization;
using Com.SharpZebra;
using Com.SharpZebra.Commands.Codes;
using Com.SharpZebra.Printing;
using POS.Utils;

namespace POS.Utils
{
    public class LabelPrinter
    {
        private readonly string _printerName;
        private readonly int _pageWidth = 292;
        private readonly int _pageStart = 40;
        private readonly int _priceCharWidth = 30;
        private readonly int _readableBarcodeCharWidth = 12;
        public LabelPrinter(string printerName)
        {
            _printerName = printerName;
            _pageWidth = int.Parse(ConfigurationManager.AppSettings["PageWidth"]);
            _pageStart = int.Parse(ConfigurationManager.AppSettings["PageStart"]);
            _priceCharWidth = int.Parse(ConfigurationManager.AppSettings["PriceCharWidth"]);
            _readableBarcodeCharWidth = int.Parse(ConfigurationManager.AppSettings["ReadableBarcodeCharWidth"]);
        }

        public void PrintPriceAndBarcode(decimal price, string barcode)
        {
            var priceString = price.ToString();
            var priceWidth = priceString.Length * _priceCharWidth;
            var priceStart = _pageStart + ((_pageWidth - priceWidth) / 2);
            var barcodeWidth = barcode.Length * _readableBarcodeCharWidth;
            var readableBarcodeStart = _pageStart + ((_pageWidth - barcodeWidth) / 2);
            var commands = new ZebraCommands();
            commands.Add(ZebraCommands.TextCommand(priceStart, 20, ElementRotation.NO_ROTATION, StandardZebraFont.LARGE, 2, 2, false, priceString));
            commands.Add(ZebraCommands.BlackLine(75, 80, 270, 1));
            commands.Add(ZebraCommands.BarCodeCommand(135, 100, ElementRotation.NO_ROTATION, 0, 1, 1, 50, false, barcode));
            commands.Add(ZebraCommands.TextCommand(readableBarcodeStart, 155, ElementRotation.NO_ROTATION, StandardZebraFont.SMALL, 1, 1, false, barcode));
            new ZebraPrinter(_printerName)
                .Print(commands);
        }

        public void PrintPrice(decimal price)
        {
            var priceString = price.ToString(CultureInfo.InvariantCulture);
            var priceWidth = priceString.Length * _priceCharWidth;
            var priceStart = _pageStart + ((_pageWidth - priceWidth) / 2);
            var commands = new ZebraCommands();
            commands.Add(ZebraCommands.TextCommand(priceStart, 60, ElementRotation.NO_ROTATION, StandardZebraFont.LARGEST, 1, 1, false, priceString));
            new ZebraPrinter(_printerName)
                .Print(commands);
        }
    }
}
public class Test
{
    public void TestMethod()
    {
        new LabelPrinter("ZDesigner LP 2824 Plus (ZPL)").PrintPrice(99.99M);
    }
}