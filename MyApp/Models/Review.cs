namespace MyApp.Models
{

    public class Review // Used for storing the steam and metacritic reviews
    {
        public int MetacriticScore { get; set; }
        public string MetaCriticLink { get; set; }
        public int SteamRating { get; set; }

        public Review() { }

    }
}