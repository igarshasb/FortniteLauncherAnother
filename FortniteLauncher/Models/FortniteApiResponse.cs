using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteLauncher.Models
{
    public class FortniteAPIResponse<T>
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }
    }

    public class Cosmetic
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("images")]
        public Images Images { get; set; }

        [JsonProperty("rarity")]
        public Type Rarity { get; set; }
    }

    public class Type
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("displayValue")]
        public string DisplayValue { get; set; }

        [JsonProperty("backendValue")]
        public string BackendValue { get; set; }
    }

    public class Images
    {
        [JsonProperty("smallIcon")]
        public string SmallIcon { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("featured")]
        public string Featured { get; set; }
    }

}
