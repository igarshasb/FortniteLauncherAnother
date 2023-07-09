using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;
using static FortniteLauncher.Globals;

namespace FortniteLauncher.Utils
{
    public class DownloadUtils
    {
        private static int counter;
        private static string totalfix;

        internal class Endpoints
        {
            public static readonly Uri Base = new Uri("http://0xkaede.xyz:1337/api/");

            public static readonly Uri PakFile = new Uri(Base, "files/z_FModContent1.pak");
            public static readonly Uri SigFile = new Uri(Base, "files/z_FModContent1.sig");

            public static readonly Uri Native = new Uri(Base, "files/KaedeNative.dll");

            public static readonly Uri LauncherFake = new Uri(Base, "files/FortniteLauncher.exe");
            public static readonly Uri Shipping = new Uri(Base, "files/FortniteClient-Win64-Shipping.exe");

            public static readonly Uri FileSize = new Uri(Base, "pak/size");
        }

        public static async Task DownloadNative()
            => await File.WriteAllBytesAsync(Constants.RunTime, await new HttpClient().GetByteArrayAsync(Endpoints.Native));

        public static async Task DownloadFakeLauncher()
            => await File.WriteAllBytesAsync(FortniteLaucher(), await new HttpClient().GetByteArrayAsync(Endpoints.LauncherFake));

        public static async Task DownloadPaks()
        {
            try
            {
                Logger.Log("Getting info for Custom Pak");
                var paksName = GetPak("z_KaedeContent1");

                if (File.Exists(paksName))
                {
                    long ApiSize = long.Parse(await new HttpClient().GetStringAsync(Endpoints.FileSize));

                    long fileSize = new FileInfo(paksName).Length;

                    if (fileSize == ApiSize)
                    {
                        Logger.Log("Latest Custom Pak is installed!");
                        return;
                    }
                }

                var webclient = new WebClient();

                webclient.Proxy = null;
                webclient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webclient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                webclient.DownloadFileAsync(Endpoints.PakFile, paksName);
                while (webclient.IsBusy)
                    await Task.Delay(1000);
                if (File.Exists(paksName))
                {
                    await Task.Delay(500);

                    await File.WriteAllBytesAsync(paksName.ToSig(), await new HttpClient().GetByteArrayAsync(Endpoints.SigFile));

                    FileUtils.DeletePakFile(GetPak("pakchunk9000-WindowsClient"));
                    FileUtils.DeletePakFile(GetPak("z_FModContent1"));
                }
                else
                {
                    Logger.Log($"File not downloaded: {paksName}", LogLevel.Error);
                    FileUtils.OpenLogError(new Exception(), "Download error");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), LogLevel.Error);
                FileUtils.OpenLogError(ex, "Download error");
            }
        }

        private static void ProgressChanged(object obj, DownloadProgressChangedEventArgs e)
        {
            var Logger = $"Downloading Custom Pak\n{e.ProgressPercentage}% of 100%, {((e.BytesReceived / 1024f) / 1024f).ToString("#0.##")}MB of {((e.TotalBytesToReceive / 1024f) / 1024f).ToString("#0.##")}MB.";

            totalfix = $"{((e.TotalBytesToReceive / 1024f) / 1024f).ToString("#0.##")}MB.";

            counter++;

            Utils.Logger.Log(Logger, LogLevel.Debug);

            if (counter % 65 == 0)
                MainWindowStatic.Dispatcher.Invoke(async () =>
                {
                    MainWindowStatic.loadingLabel.Text = Logger;
                });
        }

        private static void Completed(object obj, AsyncCompletedEventArgs e)
        {
            MainWindowStatic.Dispatcher.Invoke(async () =>
            {
                MainWindowStatic.loadingLabel.Text = $"Downloading Custom Pak\n100% of 100%, {totalfix}MB of {totalfix}MB.";
            });
        }
    }
}
