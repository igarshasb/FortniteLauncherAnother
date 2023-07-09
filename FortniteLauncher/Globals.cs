using FortniteLauncher.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteLauncher
{
    public static class Globals
    {
        public static Builds CurrentBuild { get; set; }

        public static MainWindow MainWindowStatic { get; set; }

        public static FToken FToken { get; set; }

        public static string GetPak(string pakFile) => Path.Combine(CurrentBuild.Path, $"FortniteGame\\Content\\Paks\\{pakFile}.pak");

        public static string ToSig(this string fileName) => fileName.Replace(".pak", ".sig");

        public static string FortniteLaucher() => Path.Combine(Constants.BasePath, $"FortniteLauncher.exe");
        public static string FortniteShipping() => Path.Combine(CurrentBuild.Path, $"FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe");
        public static string FortniteShippingEAC() => Path.Combine(CurrentBuild.Path, $"FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping_Eac.exe");

        public static string FortniteArgs() => $"-epicapp=Fortnite -epicenv=Prod -EpicPortal -noeac -nobe -fltoken=fdd9g715h4i20110dd40d7d3 -AUTH_TYPE=epic " +
            $"-AUTH_LOGIN={Config.Configuration.Email} -AUTH_PASSWORD={Config.Configuration.Password}";
    }
}
