using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace POS
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        Mutex _myMutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            bool aIsNewInstance = false;
            _myMutex = new Mutex(true, "MyWPFApplication", out aIsNewInstance);
            if (!aIsNewInstance)
            {
                MessageBox.Show("Already an instance is running...");
                App.Current.Shutdown();
            }
            SelectCulture("ka-GE");
        }
        public static void SelectCulture(string culture)
        {
            var dictionaryList = Application.Current.Resources.MergedDictionaries.ToList();
            var requestedDictionaryName = string.Format("/Resources/Localization/StringResources_{0}.xaml", culture);
            var resourceDictionary = dictionaryList.FirstOrDefault(d =>
                {
                    if (d.Source != null)
                        return d.Source.OriginalString == requestedDictionaryName;
                    else
                        return false;
                });
            if (resourceDictionary == null)
            {
                requestedDictionaryName = "StringResources_ka-GE.xaml";
                resourceDictionary = dictionaryList.FirstOrDefault(d =>
                {
                    if (d.Source != null)
                        return d.Source.OriginalString == requestedDictionaryName;
                    else
                        return false;
                });
            }
            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }
    }
}
