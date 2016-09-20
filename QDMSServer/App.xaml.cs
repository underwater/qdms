using QDMSServer.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QDMSServer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            new AppBootstrapper();
        }
        
    }

    public class AppBootstrapper
    {
        public AppBootstrapper()
        {
            Locator.CurrentMutable.Register(() => new FredViewModel(), typeof(FredViewModel));
            Locator.CurrentMutable.Register(() => new QuandlViewModel(), typeof(QuandlViewModel));
        }
    }
}
