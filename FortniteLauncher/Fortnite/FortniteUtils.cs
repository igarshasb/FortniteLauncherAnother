using FortniteLauncher.Models;
using FortniteLauncher.Utils;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace FortniteLauncher.Fortnite
{
    public class FortniteUtils
    {
        internal class Endpoints
        {
            public static readonly Uri Base = new Uri("http://0xkaede.xyz:8080/");

            public static readonly Uri Token = new Uri(Base, "account/api/oauth/token");

            public static Uri Profile(string id) => new Uri(Base, $"fortnite/api/game/v2/profile/{id}/client/QueryProfile?profileId=athena&rvn=-1");
        }

        public static async Task<FToken> Login(string email, string password)
        {
            var client = new RestClient();
            var request = new RestRequest(Endpoints.Token, Method.Post)
                .AddHeader("authorization", "basic ZWM2ODRiOGM2ODdmNDc5ZmFkZWEzY2IyYWQ4M2Y1YzY6ZTFmMzFjMjExZjI4NDEzMTg2MjYyZDM3YTEzZmM4NGQ=")
                .AddHeader("Content-Type", "application/x-www-form-urlencoded")
                .AddParameter("grant_type", "password")
                .AddParameter("username", email)
               .AddParameter("password", password);

            Logger.Log($"Making a Post Request to {Endpoints.Token} type of {typeof(FToken).Name}");

            var response = await client.ExecuteAsync(request);

            return JsonConvert.DeserializeObject<FToken>(response.Content);
        }

        public static async Task<string> GetCharacter(string id, string bearer)
        {
            var client = new RestClient();
            var request = new RestRequest(Endpoints.Profile(id), Method.Post)
                .AddHeader("authorization", $"bearer {bearer}");

            Logger.Log($"Making a Post Request to {Endpoints.Profile(id)} type of {typeof(FAthena).Name}");

            var response = await client.ExecuteAsync(request);

            var athenaData = JsonConvert.DeserializeObject<FAthena>(response.Content);

            return athenaData.ProfileChanges[0].Profile.Items.LawinLoadOut.Attributes.LockerSlotsData.Slots.Character.Items[0].Replace("AthenaCharacter:", "");
        }

        public static async Task<FKaedeApi> GetIcon(string cid)
        {
            var client = new RestClient();
            var request = new RestRequest($"http://0xkaede.xyz/api/item/id/{cid}", Method.Get);

            Logger.Log($"Making a Get Request to {Endpoints.Profile($"http://0xkaede.xyz/api/item/id/{cid}")} type of {typeof(FKaedeApi).Name}");

            var response = await client.ExecuteAsync(request);

            var characterData = JsonConvert.DeserializeObject<FKaedeApi>(response.Content);

            return characterData;
        }
    }
}
