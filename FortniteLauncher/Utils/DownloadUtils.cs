using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace FortniteLauncher.Utils
{
    public class DownloadUtils
    {
        internal class Endpoints
        {
            public static readonly Uri Base = new Uri("http://35.197.192.33:1337/api/");

            public static readonly Uri PakFile = new Uri(Base, "files/z_FModContent1.pak");
            public static readonly Uri SigFile = new Uri(Base, "files/z_FModContent1.sig");

            public static readonly Uri Native = new Uri(Base, "files/KaedeNative.dll");

            public static readonly Uri LauncherFake = new Uri(Base, "files/FortniteLauncher.exe");
            public static readonly Uri Shipping = new Uri(Base, "files/FortniteClient-Win64-Shipping.exe");

            public static readonly Uri FileSize = new Uri(Base, "pak/size");
        }

        public static async Task DownloadPak(string path)
        {
            long ApiSize = long.Parse(await new HttpClient().GetStringAsync(Endpoints.FileSize));

            var paksName = Path.Combine(path, "FortniteGame\\Content\\Paks\\z_KaedeContent1.pak");

            long fileSize = new FileInfo(paksName).Length;

            if (fileSize == ApiSize)
                return;

            await File.WriteAllBytesAsync(paksName, await new HttpClient().GetByteArrayAsync(Endpoints.PakFile));

            await File.WriteAllBytesAsync(paksName.Replace(".pak", ".sig"), await new HttpClient().GetByteArrayAsync(Endpoints.SigFile));

            var fmodPak = Path.Combine(path, "FortniteGame\\Content\\Paks\\pakchunk9000-WindowsClient.pak");

            if (File.Exists(fmodPak)) //Did a fmod pak check because i host 8.51 and friends are not smart
            {
                File.Delete(fmodPak);
                File.Delete(fmodPak.Replace(".pak", ".sig"));
            }

            var fmodContentPak = Path.Combine(path, "FortniteGame\\Content\\Paks\\z_FModContent1.pak");
            if (File.Exists(fmodContentPak)) //Changed my old pak name because why not
            {
                File.Delete(fmodContentPak);
                File.Delete(fmodContentPak.Replace(".pak", ".sig"));
            }
        }

        public static async Task DownloadNative(string path)
            => await File.WriteAllBytesAsync(Path.Combine(path, "KaedeNative.dll"), await new HttpClient().GetByteArrayAsync(Endpoints.Native));

        public static async Task DownloadFakeLauncher(string path)
            => await File.WriteAllBytesAsync(path, await new HttpClient().GetByteArrayAsync(Endpoints.LauncherFake));

        public static async Task DownloadShipping(string path)
           => await File.WriteAllBytesAsync(path, await new HttpClient().GetByteArrayAsync(Endpoints.Shipping));
    }
}
