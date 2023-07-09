using FortniteLauncher.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using static FortniteLauncher.Globals;

namespace FortniteLauncher.Fortnite
{
    public static class Launcher
    {
        public static async Task LaunchFortniteGame()
        {
            Logger.Log("Starting to launch fortnite");

            await DownloadUtils.DownloadNative();

            var fortniteLauncher = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = FortniteLaucher(),
                    CreateNoWindow = true,
                }
            };

            fortniteLauncher.Start();

            Logger.Log($"Started Process {Path.GetFileName(FortniteLaucher())} ({fortniteLauncher.Id})");

            SuspendProcess(fortniteLauncher);

            var fortniteShippingEac = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = FortniteShippingEAC(),
                    Arguments = FortniteArgs(),
                    CreateNoWindow = true,
                }
            };

            fortniteShippingEac.Start();

            Logger.Log($"Started Process {Path.GetFileName(FortniteLaucher())} ({fortniteShippingEac.Id})");

            SuspendProcess(fortniteShippingEac);

            var fortniteShipping = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = FortniteShipping(),
                    Arguments = FortniteArgs(),
                    RedirectStandardOutput = true,
                }
            };

            fortniteShipping.Start();

            Logger.Log($"Started Process {Path.GetFileName(FortniteShipping())} ({fortniteShipping.Id})");

            Injector.InjectDll(fortniteShipping.Id, Constants.RunTime);

            _ = MainWindowStatic.Dispatcher.Invoke(async () =>
            {
                MainWindowStatic.loadingLabel.Text = $"Waiting For Fortnite to be closed...";
            });

            fortniteShipping.WaitForExit();

            fortniteLauncher.Kill();
            fortniteShippingEac.Kill();

            File.Delete(Constants.RunTime);
        }

        private static void SuspendProcess(Process proc)
        {
            foreach (ProcessThread thread in proc.Threads)
                Win32.SuspendThread(Win32.OpenThread(0x0002, false, thread.Id));

            Logger.Log($"Supspend Process\"{proc.ProcessName}\" ({proc.Id})");
        }
    }
}
