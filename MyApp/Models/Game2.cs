using Newtonsoft.Json.Linq;

namespace MyApp.Models
{
    public class Game2
    {
        //Cheap shark
        public string Title { get; set; }
        public int SteamAppID { get; set; } = 0;
        public string Thumb { get; set; } // url for image cheapShark

        public Game2() { }

        public Game2(JObject gameInfo)
        {
            Title = (string?)gameInfo["title"] ?? "";
            SteamAppID = (int?)gameInfo["steamAppID"] ?? 0;
            Thumb = (string)gameInfo["thumb"];
        }


    }
}