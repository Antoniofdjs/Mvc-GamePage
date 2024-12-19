using Newtonsoft.Json.Linq;

namespace MyApp.Models.Igdb
{

    // Data from the igdb /games api
    public class IgdbGameDetails
    {
        public string GameName { get; set; } = "";
        public string GameID { get; set; } = "";
        public string SlugTitle { get; set; } = "";
        public string StoryLine { get; set; } = "";
        public string Summary { get; set; } = "";
        public int RatingCount { get; set; } = 0;
        public string RatingLink { get; set; } = ""; // Complete igdb review link community #
        public List<int> ThemesIDs { get; set; }
        public List<int> GenresIDs { get; set; }
        public List<int> PlatformsIDs { get; set; }
        public List<int> GameModesIDs { get; set; }
        public IgdbMultiplayerMode MultiplayerMode { get; set; } = null;

        public IgdbGameDetails() { }

        //Jobject constructor
        public IgdbGameDetails(JObject gameDetails)
        {
            if (!gameDetails.ContainsKey("id") || !gameDetails.ContainsKey("name"))
            {
                Console.WriteLine("Error: Missing required keys 'id' or 'name' in the JSON data.");
                return;
            }

            GameID = (string)gameDetails["id"] ?? "";
            GameName = (string)gameDetails["name"] ?? "";
            SlugTitle = (string)gameDetails["slug"] ?? "";
            StoryLine = (string)gameDetails["storyline"] ?? "";
            Summary = (string)gameDetails["summary"] ?? "";
            RatingCount = (int?)gameDetails["rating"] ?? 0;
            RatingLink = $"https://www.igdb.com/games/{SlugTitle}#community";

            ThemesIDs = gameDetails["themes"]?.ToObject<List<int>>() ?? new List<int>();
            GenresIDs = gameDetails["genres"]?.ToObject<List<int>>() ?? new List<int>();
            PlatformsIDs = gameDetails["platforms"]?.ToObject<List<int>>() ?? new List<int>();
            GameModesIDs = gameDetails["game_modes"]?.ToObject<List<int>>() ?? new List<int>();

        }
    }
}