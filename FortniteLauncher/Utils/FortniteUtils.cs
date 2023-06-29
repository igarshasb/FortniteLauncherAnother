﻿using FortniteLauncher.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.Design.AxImporter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FortniteLauncher.Utils
{
    public class FortniteUtils
    {
        internal class Endpoints
        {
            public static readonly Uri Base = new Uri("http://35.197.192.33:8080/");

            public static readonly Uri Token = new Uri(Base, "/account/api/oauth/token");
        }

        public static async Task<FToken> Login(string email, string password)
        {
            var client = new RestClient();
            var request = new RestRequest("http://35.197.192.33:8080/account/api/oauth/token", Method.Post)
                .AddHeader("authorization", "basic ZWM2ODRiOGM2ODdmNDc5ZmFkZWEzY2IyYWQ4M2Y1YzY6ZTFmMzFjMjExZjI4NDEzMTg2MjYyZDM3YTEzZmM4NGQ=")
                .AddHeader("Content-Type", "application/x-www-form-urlencoded")
                .AddParameter("grant_type", "password")
                .AddParameter("username", email)
                .AddParameter("password", password);

            var response = await client.ExecuteAsync(request);

            File.WriteAllText("test.txt", response.Content);
            Console.WriteLine(response.Content);

            return JsonConvert.DeserializeObject<FToken>(response.Content);
        }

        public static async Task<string> GetCharacter(string id, string bearer)
        {
            var client = new RestClient();
            var request = new RestRequest($"http://35.197.192.33:8080/fortnite/api/game/v2/profile/{id}/client/QueryProfile?profileId=athena&rvn=-1", Method.Post)
                .AddHeader("authorization", $"bearer {bearer}");

            var response = await client.ExecuteAsync(request);

            var athenaData = JsonConvert.DeserializeObject<FAthena>(response.Content);

            return athenaData.ProfileChanges[0].Profile.Items.LawinLoadOut.Attributes.LockerSlotsData.Slots.Character.Items[0].Replace("AthenaCharacter:", "");
        }

        public static async Task<string> GetIcon(string cid)
        {
            var CheckCID = cid.Contains("FMod") ? "CID_028_Athena_Commando_F" : cid;

            var client = new RestClient();
            var request = new RestRequest($"https://fortnite-api.com/v2/cosmetics/br/{CheckCID}", Method.Get);

            var response = await client.ExecuteAsync(request);

            var characterData = JsonConvert.DeserializeObject<FortniteAPIResponse<Cosmetic>>(response.Content).Data;

            return characterData.Images.SmallIcon;
        }
    }
}