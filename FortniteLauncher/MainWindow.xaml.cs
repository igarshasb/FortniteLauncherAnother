using FortniteLauncher.Controller;
using FortniteLauncher.Fortnite;
using FortniteLauncher.Utils;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using static FortniteLauncher.Globals;

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
        private BackgroundWorker worker;

        #region Main

        private async void InvokeSplashScreen()
        {
            if (_initialized)
                return;

            _initialized = true;

            RootMainGrid.Visibility = Visibility.Collapsed;
            RootLoadingGrid.Visibility = Visibility.Collapsed;
            RootLoginGrid.Visibility = Visibility.Collapsed;

            loadingLabel.Text = "Loading Configs";

            await Config.Load();

            if (Config.Configuration.Email != "unused")
            {
                EmailTxt.Text = Config.Configuration.Email;
                PasswordTxt.Text = Config.Configuration.Password;
            }

            loadingLabel.Text = "Loading Paths";

            await RefreshBuilds();

            MainWindowStatic = this;

            RootLoadingGrid.Visibility = Visibility.Hidden;
            RootMainGrid.Visibility = Visibility.Hidden;
            RootLoginGrid.Visibility = Visibility.Visible;
        }

        private async void LaunchBtn_Click(object sender, RoutedEventArgs e)
        {
            worker = new BackgroundWorker();
            worker.DoWork += LaunchFortnite;
            worker.RunWorkerAsync();
        } 

        private async void LaunchFortnite(object sender, DoWorkEventArgs e)
        {
            try
            {
                _ = Dispatcher.Invoke(async () =>
                {
                    RootMainGrid.Visibility = Visibility.Collapsed;
                    RootLoadingGrid.Visibility = Visibility.Visible;
                });

                if (!System.IO.File.Exists(FortniteLaucher()))
                {
                    _ = Dispatcher.Invoke(async () =>
                    {
                        loadingLabel.Text = "Downloading FortniteLauncher.exe...";
                    });

                    Logger.Log($"Downloading Fake FortniteLauncher");
                    await DownloadUtils.DownloadFakeLauncher();
                }

                
                if (FileUtils.GetUnrealEngineVersion() is "4.23.0")
                {
                    _ = Dispatcher.Invoke(async () =>
                    {
                        loadingLabel.Text = "Downloading Custom Paks...";
                    });

                    await DownloadUtils.DownloadPaks();
                }

                _ = Dispatcher.Invoke(async () =>
                {
                    loadingLabel.Text = "Launching Fortnite...";
                });

                await Launcher.LaunchFortniteGame();

                _ = Dispatcher.Invoke(async () =>
                {
                    var fTokenData = await FortniteUtils.Login(EmailTxt.Text, PasswordTxt.Password);
                    if (fTokenData.ErrorMessage is null)
                    {
                        var cid = await FortniteUtils.GetCharacter(fTokenData.AccountId, fTokenData.AccessToken);

                        var characterData = await FortniteUtils.GetIcon(cid);

                        SkinIcon.ImageSource = new BitmapImage(new Uri(characterData.Image));

                        var icon = characterData.Series is "" ?
                        $"http://0xkaede.xyz:1337/api/files/{characterData.Rarity}.png" :
                        $"http://0xkaede.xyz:1337/api/files/{characterData.Series}.png";

                        RarityIcon.ImageSource = new BitmapImage(new Uri(icon));
                    }
                });

                _ = Dispatcher.Invoke(async () =>
                {
                    RootMainGrid.Visibility = Visibility.Visible;
                    RootLoadingGrid.Visibility = Visibility.Collapsed;
                });
            }
            catch (Exception ex) 
            {
                Logger.Log(ex.ToString(), LogLevel.Error);
                FileUtils.OpenLogError(ex, "Fortnite Laucher");
                return;
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Logger.Log("Checking For Updates");
                var version = await new HttpClient().GetStringAsync("http://0xkaede.xyz:1337/api/version");
                if (version != Constants.Version)
                {
                    Logger.Log("Version is outdated");
                    var messageBox = new Wpf.Ui.Controls.MessageBox()
                    {
                        Title = "Version Error",
                        Content = $"Your current version is outdated.\nWe suggest you to turn off your anti virus to prevent errors\nWould you like to update to {version}?",
                        ButtonLeftName = "Update",
                        ButtonRightName= "Close",
                    };
                    messageBox.ButtonLeftClick += async (s, e) =>
                    {
                        Logger.Log("Starting Updated");
                        if (!System.IO.File.Exists(Constants.Updater))
                        {
                            var updaterBytes = await new HttpClient().GetByteArrayAsync("http://0xkaede.xyz:1337/api/files/KaedeUpdater.exe");
                            await System.IO.File.WriteAllBytesAsync(Constants.Updater, updaterBytes);

                            await Task.Delay(500);
                        }

                        var updater = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = Constants.Updater,
                            }
                        };

                        updater.Start();

                        await Task.Delay(500);
                        Application.Current.Shutdown();
                    };
                    messageBox.ButtonRightClick += (s, e) => messageBox.Close();

                    messageBox.Show();
                    return;
                }

                Logger.Log("Version is Up-To Date");

                FToken = await FortniteUtils.Login(EmailTxt.Text, PasswordTxt.Password);

                if (FToken.ErrorMessage != null)
                {
                    snakeBarNot.Show("Login Error", FToken.ErrorMessage, Wpf.Ui.Common.SymbolRegular.ShieldError24, Wpf.Ui.Common.ControlAppearance.Danger);
                    return; //stop
                }

                SignedInAsLable.Content = $"Signed in as {FToken.DisplayName}";

                Config.Configuration.Email = EmailTxt.Text;
                Config.Configuration.Password = PasswordTxt.Password;
                await Config.Save();

                var cid = await FortniteUtils.GetCharacter(FToken.AccountId, FToken.AccessToken);

                cid = cid is null ? cid : "CID_001_Athena_Commando_F_Default";

                var characterData = await FortniteUtils.GetIcon(cid);

                SkinIcon.ImageSource = new BitmapImage(new Uri(characterData.Image));

                var icon = characterData.Series is "" ?
                        $"http://0xkaede.xyz:1337/api/files/{characterData.Rarity}.png" :
                        $"http://0xkaede.xyz:1337/api/files/{characterData.Series}.png";

                RarityIcon.ImageSource = new BitmapImage(new Uri(icon));

                RootLoadingGrid.Visibility = Visibility.Hidden;
                RootMainGrid.Visibility = Visibility.Hidden;
                RootVersionGrid.Visibility = Visibility.Visible;
                RootLoginGrid.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), LogLevel.Error);
                FileUtils.OpenLogError(ex, "Login");
                return;
            }
        }

        #endregion

        #region Builds

        private async Task RefreshBuilds()
        {
            try
            {
                Versions.Children.Clear();

                var builds = Config.Configuration.Builds;

                foreach (var build in builds)
                {
                    var controller = new VersionControl(build.Name, build.Path);
                    controller.MouseLeftButtonDown += delegate
                    {
                        CurrentBuild = build;
                        CurrentPathTxt.Text = build.Path;
                        RootVersionGrid.Visibility = Visibility.Hidden;
                        RootMainGrid.Visibility = Visibility.Visible;
                    };
                    Versions.Children.Add(controller);
                    Logger.Log($"Added {build.Name} to build list");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), LogLevel.Error);
                FileUtils.OpenLogError(ex, "Login Error");
                return;
            }
        }

        private void AddBuildBtn_Click(object sender, RoutedEventArgs e)
        {
            RootVersionGrid.Visibility = Visibility.Hidden;
            AddBuildGrid.Visibility = Visibility.Visible;
        }

        private async void SaveBuildBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(InstallPath.Text))
                {
                    snakeBarNot.Show("Installation is null or empty!", "Please enter a path.", Wpf.Ui.Common.SymbolRegular.Folder16, Wpf.Ui.Common.ControlAppearance.Danger);
                    return;
                }

                if (string.IsNullOrEmpty(BuildName.Text))
                {
                    snakeBarNot.Show("Build name is null or empty!", "Please enter a build name!", Wpf.Ui.Common.SymbolRegular.Box16, Wpf.Ui.Common.ControlAppearance.Danger);
                    return;
                }

                if (!Directory.Exists(Path.Combine(InstallPath.Text, "FortniteGame")))
                {
                    snakeBarNot.Show("Installation error!", "Please make sure the directory you selected contains a folder\ncalled \"FortniteGame\" and \"Engine\"!",
                        Wpf.Ui.Common.SymbolRegular.Folder16, Wpf.Ui.Common.ControlAppearance.Danger);
                    return;
                }

                var build = new Builds
                {
                    Name = BuildName.Text,
                    Path = InstallPath.Text,
                };

                var addinfo = await Config.AddBuild(build);
                if (addinfo == $"We already found a build that is called {build.Name}!")
                {
                    snakeBarNot.Show("Config Error!", $"We already found a build that is called {build.Name}!", Wpf.Ui.Common.SymbolRegular.Folder16, Wpf.Ui.Common.ControlAppearance.Danger);
                    return;
                }

                snakeBarNot.Show("Build Added!", $"The build {build.Name} was added to your list!", Wpf.Ui.Common.SymbolRegular.Folder16, Wpf.Ui.Common.ControlAppearance.Success);

                await RefreshBuilds();

                RootVersionGrid.Visibility = Visibility.Visible;
                AddBuildGrid.Visibility = Visibility.Hidden;
            }
            catch (Exception ex) 
            {
                Logger.Log(ex.ToString(), LogLevel.Error);
                FileUtils.OpenLogError(ex, "Login Error");
                return;
            }
        }

        private void AddBuildBtn_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
                if (dialog.ShowDialog(this).GetValueOrDefault())
                {
                    var selectedPath = dialog.SelectedPath;
                    if (!Directory.Exists(selectedPath + "\\FortniteGame"))
                    {
                        snakeBarNot.Show("Path Error", "Please select a folder witch contains", Wpf.Ui.Common.SymbolRegular.ShieldError24, Wpf.Ui.Common.ControlAppearance.Danger);
                        return;
                    }

                    InstallPath.Text = selectedPath;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), LogLevel.Error);
                FileUtils.OpenLogError(ex, "Login Error");
                return;
            }
        }

        private void BackBuildBtn_Click(object sender, RoutedEventArgs e)
        {
            RootVersionGrid.Visibility = Visibility.Visible;
            AddBuildGrid.Visibility = Visibility.Hidden;
        }

        #endregion
    }
}
