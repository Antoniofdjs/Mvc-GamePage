namespace MyApp.Models
{
    public class Game
    {
        public string Title { get; set; }
        public List<Deal> Deals { get; set; }
        public string Thumb { get; set; }
        public Game() { }
    }
}