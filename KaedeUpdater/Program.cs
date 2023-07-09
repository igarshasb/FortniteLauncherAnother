using System;
using System.Diagnostics;

namespace KaedeUpdater
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Log("Downloading Latest Updates");

            await Task.Delay(2500);

            if (File.Exists("FortniteLauncher.exe"))
                File.Delete("FortniteLauncher.exe");

            var exeBytes = await new HttpClient().GetByteArrayAsync(new Uri("http://0xkaede.xyz:1337/api/files/Latest.exe"));

            Log("Writing Bytes to file");

            await File.WriteAllBytesAsync("FortniteLauncher.exe", exeBytes);

            Log("Update was applied successfully!");

            for (int i = 5; i > 0; i--)
            {
                Log($"Closing in {i}");

                if (i == 3)
                    Process.Start("FortniteLauncher.exe");

                await Task.Delay(1000);
            }
        }

        public static void Log(string log, LogType type = LogType.INFO)
        {
            if (type == LogType.INFO)
                Console.ForegroundColor = ConsoleColor.Green;
            else if (type == LogType.WARN)
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            else if (type == LogType.ERROR)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (type == LogType.DOWN)
                Console.ForegroundColor = ConsoleColor.Magenta;
            else if (type == LogType.Zip)
                Console.ForegroundColor = ConsoleColor.Gray;

            Console.Write(">> ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{log}\n");


        }
    }

    public enum LogType
    {
        INFO,
        WARN,
        ERROR,
        DOWN,
        Zip
    }
}