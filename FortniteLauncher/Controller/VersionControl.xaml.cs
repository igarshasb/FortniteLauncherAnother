using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FortniteLauncher.Controller
{
    /// <summary>
    /// Interaction logic for VersionControl.xaml
    /// </summary>
    public partial class VersionControl : UserControl
    {
        private readonly string Name;
        public VersionControl(string name, string path)
        {
            InitializeComponent();
            Name = name;
            BuildName.Content = name;
            SeasonImage.Source = new BitmapImage(new Uri($"{path}\\FortniteGame\\Content\\Splash\\Splash.bmp"));
            BuildVersion.Content = path;
        }

        private async void DeleteBuild_Click(object sender, RoutedEventArgs e)
        {
            await Config.DeleteBuildByName(Name);
            Visibility = Visibility.Collapsed;
        }
    }
}
