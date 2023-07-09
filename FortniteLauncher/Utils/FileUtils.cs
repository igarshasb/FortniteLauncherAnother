using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using static FortniteLauncher.Globals;

namespace FortniteLauncher.Utils
{
    public static class FileUtils
    {
        public static void DeletePakFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            var sig = path.ToSig();

            if (File.Exists(sig))
                File.Delete(sig);
        }

        public static void CheckDirectory(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public static string GetUnrealEngineVersion()
        {
            var info = FileVersionInfo.GetVersionInfo(FortniteShipping());
            return $"{info.FileMajorPart}.{info.FileMinorPart}.{info.FileBuildPart}";
        }

        public static void OpenLogError(Exception ex, string errorTitle)
        {
            _ = MainWindowStatic.Dispatcher.Invoke(async () =>
            {
                var errorMsg = $"Please send your Log File to 0xkaede\nError - {ex.Message}";
                var messageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = $"Error - {errorTitle}",
                    Content = errorMsg
                };

                messageBox.ButtonLeftName = "Open Log";
                messageBox.ButtonRightName = "Close";
                messageBox.ButtonLeftClick += (s, e) => Process.Start("explorer.exe", Constants.LogFile);
                messageBox.ButtonRightClick += (s, e) => messageBox.Close();

                messageBox.Show();
            });
            
        }
    }
}
