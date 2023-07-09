using FortniteLauncher.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace FortniteLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() { }

        protected override void OnStartup(StartupEventArgs e)
        {
            FileUtils.CheckDirectory(Constants.BasePath);
            FileUtils.CheckDirectory(Constants.LogPath);

            if (File.Exists("KaedeUpdater.exe"))
                File.Delete("KaedeUpdater.exe");

            Logger.Start();

            var window = new MainWindow();
            window.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Logger.Log("Application ended");
            base.OnExit(e);
        }
    }
}
