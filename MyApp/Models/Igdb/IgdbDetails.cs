//toda la data de IGDB
//  ScreenShots foreing id
//         Videos   youtube id https://www.youtube.com/embed/{videoID} using iframe
//         Themes Themes + Genres foreing id array
//         Summary string
//         StoryLine string
//         Rating Double


// Create view Model for IndexGameVM
//Game "CheapShark"
//IgdbDetails


// IndexGameVM.IgdbDetails.Videos   "Aqui saco los videos "
using Microsoft.AspNetCore.SignalR;

namespace MyApp.Models.Igdb
{

    public class IgdbGameDetails
    {
        public string GameName { get; set; } = "";
        public string GameID { get; set; } = "";
        public string SlugTitle { get; set; } = "";
        public string StoryLine { get; set; } = "";
        public string Summary { get; set; } = "";
        public int RatingCount { get; set; } = 0;
        public string RatingLink { get; set; } = ""; // Complete igdb review link community #
        public List<string> Platforms { get; set; } = new List<string>(); // Names of platforms
        public List<string>? ThemesGenres { get; set; } = new List<string>();
        public List<string>? VideosLinks { get; set; } = new List<string>(); // Complete youtube Video Link EMBED "https://www.youtube.com/embed/{videoId}"

    }

}