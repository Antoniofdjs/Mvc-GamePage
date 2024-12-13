
namespace MyApp.Models.ViewModels
{
    public class IndexGameVM
    {
        public Game Game { get; set; } // cheapShark only
        public IgdbDetails IgdbDetails { get; set; }// Igdb Only

        //Game
        //IgdbDetails
        public IndexGameVM()
        {
            Game = new Game();
            IgdbDetails = new IgdbDetails();
        }
    }
}