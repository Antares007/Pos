using System;
using System.Collections.Generic;
using System.Configuration;
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
        }

    }
}
