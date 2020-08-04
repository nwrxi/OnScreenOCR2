using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace OnScreenOCR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(AppSettings.Default.GOOGLE_APPLICATION_CREDENTIALS))
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", AppSettings.Default.GOOGLE_APPLICATION_CREDENTIALS);
            }
        }
    }
}
