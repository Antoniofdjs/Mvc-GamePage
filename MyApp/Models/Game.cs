namespace MyApp.Models
{
    public class Game
    {
        public string Title { get; set; }
        public int SteamAppID { get; set; } = 0;
        public List<Deal> Deals { get; set; }
        public string Thumb { get; set; }
        public Review Review { get; set; }
        public Game() { }
    }
}