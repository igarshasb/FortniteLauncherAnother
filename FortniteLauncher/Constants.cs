using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteLauncher
{
    public static class Constants
    {
        public static readonly string BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KaedeServer\\");

        public static readonly string LogPath = Path.Combine(BasePath, "Logs");

        public static readonly string LogFile = Path.Combine(LogPath, "KaedeLauncher.log");
        public static readonly string ConfigFile = Path.Combine(BasePath, "config.kaede");

        public static readonly string RunTime = Path.Combine(BasePath, "KaedeNative.dll");
        public static readonly string Updater = Path.Combine(Directory.GetCurrentDirectory(), "KaedeUpdater.exe");

        public static readonly string Version = "1.0.5";
    }
}
