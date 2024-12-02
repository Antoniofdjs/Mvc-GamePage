namespace MyApp.Models
{

    public class ListedGame // "GameListed" when searching via a title, not the sames as searching via GameID that will be called "Game"
    {
        public int GameID { get; set; }
        public string CheapestDealID { get; set; }
        public double CheapestPrice { get; set; }
        public string Title { get; set; }
        public string Thumb { get; set; }

        public ListedGame() { }

    }
}