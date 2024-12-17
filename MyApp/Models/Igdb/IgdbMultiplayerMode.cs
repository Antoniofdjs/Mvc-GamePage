
namespace MyApp.Models.Igdb
{

    public class IgdbMultiplayerMode
    {
        public bool CampaingCoop { get; set; }
        public bool LanCoop { get; set; }
        public bool OfflineCoop { get; set; }
        public int OfflineCoopmax { get; set; } //players total
        public int OfflineMax { get; set; } // players total

        public bool SplitScreen { get; set; }
        public bool SplitsScreenOnline { get; set; }
        public bool OnlineCoop { get; set; }
        public int OnlinecoopMax { get; set; }
        public int OnlineMax { get; set; }

        public IgdbMultiplayerMode() { }
    }
}
