using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FortniteLauncher.Utils
{
    public class Config
    {
        private static readonly string FILE = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "KaedeServer\\config.kaede");
        public static Root Configuration { get; set; }

        public async static Task Load()
        {
            if (!File.Exists(FILE))
            {
                await Task.Delay(1000);

                Configuration = new Root();

                await Save();
            }
            else
                Configuration = JsonConvert.DeserializeObject<Root>(File.ReadAllText(FILE));
        }

        public async static Task Save()
            => await File.WriteAllTextAsync(FILE, JsonConvert.SerializeObject(Configuration));
    }

    public class Root
    {
        public string Email { get; set; } = "unused";
        public string Password { get; set; }
        public string Path { get; set; }
    }
}
