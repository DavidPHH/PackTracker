using System;
using System.Collections.Generic;
using System.Net.Http;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone;
using Newtonsoft.Json.Linq;
using PackTracker.Entity;
using System.IO;
using System.Xml;
using PackTracker.View;
using System.Linq;
using Rarity = HearthDb.Enums.Rarity;
using System.Net.Http.Headers;

namespace PackTracker.Storage {
    internal class PityTrackerUploader : IPityTrackerUploader {

        private Dictionary<string, string> PackToDict(Pack Pack) {
            Dictionary<Rarity, string> types = new Dictionary<Rarity, string>{
                {Rarity.COMMON,"commons" },
                {Rarity.RARE, "rares" },
                {Rarity.EPIC, "epics" },
                {Rarity.LEGENDARY, "legendaries" }
            };
            Dictionary<string, int> count = new Dictionary<string, int>();
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
                {"pack[set_type]", PackNameConverter.Convert(Pack.Id, "short")},
                {"commit", "Add+Pack"}
            }.Concat(PackToDict(Pack)));
            HttpResponseMessage resp = await client.SendAsync(req);
        }
    }
}
