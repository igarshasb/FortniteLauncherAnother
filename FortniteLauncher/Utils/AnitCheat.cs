using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FortniteLauncher.Utils
{
    public class AnitCheat
    {
        public static bool waiting = true;

        public static async Task Init(int currentProc, int fortniteProc)
        {
            while (waiting)
            {
                await Task.Delay(1000);

                var process = Process.GetProcessesByName("FortniteLauncher");

                foreach(var proc in process)
                {
                    var procId = proc.Id;
                    if (procId == currentProc || procId == fortniteProc)
                        continue;

                    proc.Kill();
                }
            }
        }

        public static void Stop() => waiting = false;
    }
}
