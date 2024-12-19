using Newtonsoft.Json.Linq;

namespace MyApp.Models
{
    public class Game
    {
        //Cheap shark
        public string Title { get; set; }
        public int SteamAppID { get; set; } = 0;
        public string Thumb { get; set; } // url for image cheapShark

        public List<MainDeal> Deals { get; set; }
        public List<int> DealsIDs { get; set; }
        public Review Review { get; set; }

        //IGDB
        /*
        If you search by title in games:
        
        ScreenShots foreing id
        Videos---youtube id https://www.youtube.com/embed/{videoID} using iframe
        Themes= Themes + Genres foreing id array
        Summary string
        StoryLine string
        Rating Double

        */

        public Game() { }

        public Game(JObject gameInfo)
        {
            Title = (string?)gameInfo["title"] ?? "";
            SteamAppID = (int?)gameInfo["steamAppID"] ?? 0;
            Thumb = (string)gameInfo["thumb"];
        }


    }
}