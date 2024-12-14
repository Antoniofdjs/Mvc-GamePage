
namespace MyApp.Models.ViewModels
{
    public class IndexGameVM
    {
        public Game? Game { get; set; } = null; // cheapShark only
        public IgdbDetails? IgdbDetails { get; set; } = null;// Igdb Only

        //Game
        //IgdbDetails
        public IndexGameVM()
        {
            Game = new Game();
            IgdbDetails = new IgdbDetails();
        }
    }
}