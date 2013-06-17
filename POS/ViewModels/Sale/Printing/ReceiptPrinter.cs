using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using POS.ServerApi;

namespace POS.ViewModels.Sale.Printing
{
    public class ReceiptPrinter
    {
        public async Task PrintReceipt(Jq cq)
        {

            var biSource = new BitmapImage();

            var ms = await (await new HttpClient().GetAsync(cq.GetText("logo"))).Content.ReadAsStreamAsync();
            ms.Seek(0, SeekOrigin.Begin);
            biSource.BeginInit();
            biSource.StreamSource = ms;
            biSource.EndInit();

            var receipt = new Receipt
                {
                    Phone = { Text = cq.GetText("tel") },
                    Address = { Text = cq.GetText("misamarti") },
                    Barcode = { Text = cq.GetText("chekisNomeri") },
                    Logo = { Source = biSource }
                };

            var items = cq.All("chekisKhazebi", cqq => new PartReceiptItem()
                {
                    Name = { Text = cqq.GetText("dasakheleba") },
                    Ean = { Text = cqq.GetText("kodi") },
                    UnitPrice = { Text = cqq.GetText("ertFasi") },
                    Quantity = { Text = cqq.GetText("raodenoba") },
                    Total = { Text = cqq.GetText("jami") },
                });
            foreach (var item in items)
            {
                receipt.Items.Items.Add(item);
            }
            var amountItems = cq.All("jamisKhazebi", cqq => new PartReceiptAmountItem()
                {
                    Name = { Text = cqq.GetText("dasakheleba") },
                    Amount = { Text = cqq.GetText("tankha") }
                });
            foreach (var aItem in amountItems)
            {
                receipt.Amounts.Items.Add(aItem);
            }
            var printer = new Printer(ConfigurationManager.AppSettings["ReceiptPrinter"]);
            printer.PrintReceipt(receipt);
        }
    }
}