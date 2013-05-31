using Com.SharpZebra;
using Com.SharpZebra.Commands.Codes;
using Com.SharpZebra.Printing;
using POS.Utils;

namespace POS.Utils
{
    public class LabelPrinter
    {
        private readonly string _printerName;
        private const int PageWidth = 292;
        private const int PageStart = 65;
        private const int PriceCharWidth = 30;
        private const int ReadableBarcodeCharWidth = 12;
        public LabelPrinter(string printerName)
        {
            _printerName = printerName;
        }

        public void PrintPriceAndBarcode(decimal price, string barcode)
        {
            var priceString = price.ToString();
            var priceWidth = priceString.Length * PriceCharWidth;
            var priceStart = PageStart + ((PageWidth - priceWidth) / 2);
            var barcodeWidth = barcode.Length * ReadableBarcodeCharWidth;
            var readableBarcodeStart = PageStart + ((PageWidth - barcodeWidth) / 2);
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
            var priceString = price.ToString();
            var priceWidth = priceString.Length * PriceCharWidth;
            var priceStart = PageStart + ((PageWidth - priceWidth) / 2);
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