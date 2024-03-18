using System;
using System.Collections.Generic;
using System.Net.Http;
using Hearthstone_Deck_Tracker.Hearthstone;
using PackTracker.Entity;
using System.IO;
using System.Linq;
using Rarity = HearthDb.Enums.Rarity;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace PackTracker.Storage {
    internal class PityTrackerUploader : IPityTrackerUploader {

        internal static Dictionary<int, string> shortCodes;
        static PityTrackerUploader()
        {


            using var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("PackTracker.Resources.pitytracker-shortcodes.json");
            using var sr = new StreamReader(s);
            shortCodes = JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd()).ToObject<Dictionary<int, string>>();
        }

        public static string GetShortCode(int packID)
        {
            if (shortCodes.ContainsKey(packID))
            {
                return shortCodes[packID];
            }
            return null;
        }

        private Dictionary<string, string> PackToDict(Pack Pack) {
            Dictionary<Rarity, string> types = new()
            {
                {Rarity.COMMON,"commons" },
                {Rarity.RARE, "rares" },
                {Rarity.EPIC, "epics" },
                {Rarity.LEGENDARY, "legendaries" }
            };
            Dictionary<string, int> count = [];
            foreach (var t in types.Values) {
                count[t] = 0;
                count[$"golden_{t}"] = 0;
            }
            foreach (var Card in Pack.Cards) {
                var cardType = Card.Premium ? $"golden_{types[Card.Rarity]}" : types[Card.Rarity];
                count[cardType]++;
            }

            return count.ToDictionary(e => $"pack[{e.Key}]", e => e.Value.ToString());
        }


        public async void UploadPack(string Cookie, string AuthToken, Pack Pack) {
            var shortCode = GetShortCode(Pack.Id);
            if (shortCode is null)
            {
                return;
            }
            if (Pack is null) return;
            var client = new HttpClient(new HttpClientHandler{ UseCookies = false });
            var req = new HttpRequestMessage(HttpMethod.Post, new Uri("https://pitytracker.com/packs"));
            req.Headers.Add("Cookie", Cookie);
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            client.DefaultRequestHeaders.ExpectContinue = false;
            req.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                {"authenticity_token", AuthToken},
                {"pack[region]", "Europe"},
                {"pack[reward_type]", "na"},
                {"pack[set_type]", shortCode},
                {"commit", "Add+Pack"}
            }.Concat(PackToDict(Pack)));
            await client.SendAsync(req);
        }
    }
}
