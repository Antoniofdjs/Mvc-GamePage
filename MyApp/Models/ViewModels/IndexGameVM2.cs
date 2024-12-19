using MyApp.Models.Igdb;
namespace MyApp.Models.ViewModels
{
    public class IndexGameVM
    {
        public Game2? Game { get; set; } = null; // cheapShark only
        public IgdbGameDetails? IgdbDetails { get; set; } = new IgdbGameDetails();// Igdb Only


        public List<Deal> Deals { get; set; } = new List<Deal>();
        public List<Review2> Reviews { get; set; } = new List<Review2>();
        public List<string> ThemesGenres { get; set; } = new List<string>();
        public List<string> Platforms { get; set; } = new List<string>();
        public List<string> GameModes { get; set; } = new List<string>();
        public List<string> MultiPlayerModes { get; set; } = new List<string>();

        //Game
        //IgdbDetails
        public IndexGameVM()
        {
        }
    }
}