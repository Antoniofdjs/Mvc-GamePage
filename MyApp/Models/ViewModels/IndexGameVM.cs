using MyApp.Models.Igdb;
namespace MyApp.Models.ViewModels
{
    public class IndexGameVM
    {
        public Game? Game { get; set; } = null; // cheapShark only
        public IgdbGameDetails? IgdbDetails { get; set; } = null;// Igdb Only

        public List<string> ThemesGenres { get; set; } = new List<string>();
        public List<string> Platforms { get; set; } = new List<string>();
        public List<string> GameModes { get; set; } = new List<string>();

        //Game
        //IgdbDetails
        public IndexGameVM()
        {
        }
    }
}