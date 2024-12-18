
using Newtonsoft.Json.Linq;

namespace MyApp.Models.Igdb
{

    public class IgdbMultiplayerMode
    {
        public int OnlineMax { get; set; }
        public bool OnlineCoop { get; set; }
        public int OnlinecoopMax { get; set; }

        public bool SplitsScreenOnline { get; set; }
        public bool SplitScreen { get; set; }


        public bool CampaingCoop { get; set; }
        public bool LanCoop { get; set; }
        public bool OfflineCoop { get; set; }
        public int OfflineCoopmax { get; set; } //players total
        public int OfflineMax { get; set; } // players total


        public IgdbMultiplayerMode(JObject multiplayerModes) // igdbApi json constructor
        {
            List<string> requiredKeys = new List<string>(){
                "campaigncoop",
                "lancoop",
                "offlinecoop",
                "offlinecoopmax",
                "offlinemax",
                "onlinecoop",
                "onlinecoopmax",
                "splitscreen",
                "splitscreenonline",
                "onlinemax",
            };
            // foreach (var key in requiredKeys)
            // {
            //     if (!multiplayerModes.ContainsKey(key))
            //     {
            //         Console.WriteLine($"Error: Missing required key '{key}' in multiplayer modes JSON.");
            //         return;
            //     }
            // }

            OnlineMax = (int?)multiplayerModes["onlinemax"] ?? 0;
            OnlineCoop = (bool?)multiplayerModes["onlinecoop"] ?? false;
            OnlinecoopMax = (int?)multiplayerModes["onlinecoopmax"] ?? 0;
            SplitsScreenOnline = (bool?)multiplayerModes["splitscreenonline"] ?? false;
            SplitScreen = (bool?)multiplayerModes["splitscreen"] ?? false;
            CampaingCoop = (bool?)multiplayerModes["campaigncoop"] ?? false;
            LanCoop = (bool?)multiplayerModes["onlinecoop"] ?? false;
            OfflineCoop = (bool?)multiplayerModes["offlinecoop"] ?? false;
            OfflineCoopmax = (int?)multiplayerModes["onlinecoopmax"] ?? 0;
            OfflineMax = (int?)multiplayerModes["offlinemax"] ?? 0;

            if (OfflineCoop == true && OfflineCoopmax == 0)
            {
                OfflineCoop = false;
            }

            Console.WriteLine("values for my  modes: ");
            Console.WriteLine(OnlineMax);
            Console.WriteLine(OnlineCoop);
            Console.WriteLine(OnlinecoopMax);
            Console.WriteLine(SplitsScreenOnline);
            Console.WriteLine(SplitScreen);
            Console.WriteLine(CampaingCoop);
            Console.WriteLine(LanCoop);
            Console.WriteLine(OfflineCoop);
            Console.WriteLine(OfflineCoopmax);
            Console.WriteLine(OfflineMax);
        }
    }
}
