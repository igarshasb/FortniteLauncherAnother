using FortniteLauncher.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Appearance;
using static System.Net.Mime.MediaTypeNames;
using MessageBox = Wpf.Ui.Controls.MessageBox;

namespace FortniteLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Accent.ApplySystemAccent();
            Wpf.Ui.Appearance.Background.Apply(this, BackgroundType.Mica);

            Loaded += (_, _) => InvokeSplashScreen();
        }

        private bool _initialized = false;
        BackgroundWorker worker;

        private static string installLocation;
        private static readonly string _appdata = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\KaedeServer";

        private Process _fnProcess;
        private Process _fnLauncher;

        private async void InvokeSplashScreen()
        {
            if (_initialized)
                return;

            _initialized = true;

            RootMainGrid.Visibility = Visibility.Collapsed;
            RootLoadingGrid.Visibility = Visibility.Collapsed;
            RootLoginGrid.Visibility = Visibility.Collapsed;

            loadingLabel.Text = "Loading...";

            if(!Directory.Exists(_appdata))
                Directory.CreateDirectory(_appdata);

            loadingLabel.Text = "Loading Configs";

            await Config.Load();

            if(Config.Configuration.Email != "unused")
            {
                EmailTxt.Text = Config.Configuration.Email;
                PasswordTxt.Text = Config.Configuration.Password;
            }

            loadingLabel.Text = "Loading Paths";

            FModTxt.Text = Config.Configuration.Path;

            installLocation = FModTxt.Text;

            RootLoadingGrid.Visibility = Visibility.Hidden;
            RootMainGrid.Visibility = Visibility.Hidden;
            RootLoginGrid.Visibility = Visibility.Visible;
        }

        private async void LaunchBtn_Click(object sender, RoutedEventArgs e)
        {
            FModTxt.Text = Config.Configuration.Path;

            worker = new BackgroundWorker();
            worker.DoWork += LaunchFortnite;
            worker.RunWorkerAsync();
        }

        private async void LaunchFortnite(object sender, DoWorkEventArgs e)
        {
            Process _fnProcess;
            Process _fnLauncher;

            var win64Location = $"{installLocation}\\FortniteGame\\Binaries\\Win64";
            var launcherExe = $"{_appdata}\\FortniteLauncher.exe";
            var shippingExe = $"{win64Location}\\FortniteClient-Win64-Shipping.exe";
            string fileName3 = win64Location + "\\FortniteClient-Win64-Shipping_EAC.exe";

            _ = Dispatcher.Invoke(async () =>
             {
                 RootMainGrid.Visibility = Visibility.Collapsed;
                 RootLoadingGrid.Visibility = Visibility.Visible;
             });

            if (!File.Exists(launcherExe))
            {
                _ = Dispatcher.Invoke(async () =>
                {
                    loadingLabel.Text = "Downloading FortniteLauncher.exe...";
                });

                await DownloadUtils.DownloadFakeLauncher(launcherExe);
            }

            if (!File.Exists(shippingExe))
            {
                _ = Dispatcher.Invoke(async () =>
                {
                    loadingLabel.Text = "Downloading FortniteClient-Win64-Shipping.exe...";
                });

                await DownloadUtils.DownloadShipping(shippingExe);
            }

            _ = Dispatcher.Invoke(async () =>
            {
                loadingLabel.Text = "Downloading Custom Paks...";
            });

            await DownloadUtils.DownloadPak(installLocation);

            _ = Dispatcher.Invoke(async () =>
            {
                loadingLabel.Text = "Downloading Dll's...";
            });

            await DownloadUtils.DownloadNative(_appdata);

            _ = Dispatcher.Invoke(async () =>
            {
                loadingLabel.Text = "Launching Fortnite...";
            });

            var launchArgs = $"-skippatchcheck -epicapp=Fortnite -epicenv=Prod -EpicPortal -useallavaliblecores -HTTP=WinINet -steamimportavailable " +
                $"-epiclocale=en -epicsandboxid=fn -noeac -nobe -fltoken=none -AUTH_TYPE=epic -AUTH_LOGIN={Config.Configuration.Email} -AUTH_PASSWORD={Config.Configuration.Password}";

            _fnLauncher = new Process
            {
                StartInfo =
                {
                    FileName = launcherExe,
                    Arguments = launchArgs,
                    CreateNoWindow = true
                }
            };
            _fnLauncher.Start();

            foreach (ProcessThread thread in _fnLauncher.Threads)
                Win32.SuspendThread(Win32.OpenThread(0x0002, false, thread.Id));

            _fnProcess = new Process
            {
                StartInfo =
                {
                    FileName = shippingExe,
                    Arguments = launchArgs,
                    RedirectStandardOutput = true
                }
            };

            _fnProcess.Start();

            _ = Dispatcher.Invoke(async () =>
            {
                loadingLabel.Text = "Injecting native...";
            });

            Injector.InjectDll(_fnProcess.Id, System.IO.Path.Combine(_appdata, "KaedeNative.dll"));

            _ = Dispatcher.Invoke(async () =>
            {
                loadingLabel.Text = $"Waiting For Fortnite to be closed...";
            });

            _fnProcess.WaitForExit();
            _fnLauncher.Kill();

            _ = Dispatcher.Invoke(async () =>
            {
                var fTokenData = await FortniteUtils.Login(EmailTxt.Text, PasswordTxt.Password);

                if (fTokenData.ErrorMessage is null)
                {
                    var cid = await FortniteUtils.GetCharacter(fTokenData.AccountId, fTokenData.AccessToken);

                    var characterData = await FortniteUtils.GetIcon(cid);

                    SkinIcon.ImageSource = new BitmapImage(new Uri(characterData.Images.SmallIcon));
                    RarityIcon.ImageSource = new BitmapImage(new Uri($"http://0xkaede.xyz:1337/api/files/{characterData.Rarity.Value}.png"));
                }
            });

            _ = Dispatcher.Invoke(async () =>
            {
                RootMainGrid.Visibility = Visibility.Visible;
                RootLoadingGrid.Visibility = Visibility.Collapsed;
            });
        }

        private void RevertModsBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void SelectPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                var selectedPath = dialog.SelectedPath;

                if(!Directory.Exists(selectedPath + "\\FortniteGame"))
                {
                    snakeBarNot.Show("Path Error", "Please select a folder witch contains", Wpf.Ui.Common.SymbolRegular.ShieldError24, Wpf.Ui.Common.ControlAppearance.Danger);
                    return;
                }

                installLocation = selectedPath;
                FModTxt.Text = selectedPath;
                Config.Configuration.Path = selectedPath;
                await Config.Save();
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var fTokenData = await FortniteUtils.Login(EmailTxt.Text, PasswordTxt.Password);

            if(fTokenData.ErrorMessage != null)
            {
                snakeBarNot.Show("Login Error", fTokenData.ErrorMessage, Wpf.Ui.Common.SymbolRegular.ShieldError24, Wpf.Ui.Common.ControlAppearance.Danger);
                return; //stop
            }

            SignedInAsLable.Content = $"Signed in as {fTokenData.DisplayName}";

            Config.Configuration.Email = EmailTxt.Text;
            Config.Configuration.Password = PasswordTxt.Password;
            await Config.Save();

            var cid = await FortniteUtils.GetCharacter(fTokenData.AccountId, fTokenData.AccessToken);

            var characterData = await FortniteUtils.GetIcon(cid);

            SkinIcon.ImageSource = new BitmapImage(new Uri(characterData.Images.SmallIcon));
            RarityIcon.ImageSource = new BitmapImage(new Uri($"http://0xkaede.xyz:1337/api/files/{characterData.Rarity.Value}.png"));

            RootLoadingGrid.Visibility = Visibility.Hidden;
            RootMainGrid.Visibility = Visibility.Visible;
            RootLoginGrid.Visibility = Visibility.Hidden;
        }
    }
}
