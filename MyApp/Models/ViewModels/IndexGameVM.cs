
namespace MyApp.Models.ViewModels
{
    public class IndexGameVM
    {
        public Game Game { get; set; } // cheapShark only

        // IdgbDetails
        public List<string> ThemesGenres { get; set; }
        public string StoryLine { get; set; }
        public string Summary { get; set; }
        public int Rating { get; set; }
        public string RatingLink { get; set; }
        public string Slug { get; set; }

        //Game
        //IgdbDetails
        public IndexGameVM() { }
    }
}