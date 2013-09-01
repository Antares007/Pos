using System.Collections.Generic;

namespace POS.Utils
{
    public static class PaymentName2ResKeyConverter
    {
         private static readonly Dictionary<string,string> _keys = new Dictionary<string, string>()
             {
                 {"ქეში","Nagdi"},
                 {"საკრედიტო ბარათი","SakreditoBarati"},
                 {"სასაჩუქრე ბარათი","SasachukreBarati"},
                 {"სულ გადახდილი","SulGadakhdili"},
                 {"სულ გადასახდელი","SulGadasakhdeli"},
                 {"ხურდა","Khurda"},
             }; 
         public static string Convert(string name)
         {
             return _keys[name];
         }
    }
}