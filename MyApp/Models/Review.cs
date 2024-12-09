namespace MyApp.Models
{

    public class Review // Used for storing the steam and metacritic reviews
    {
        public int MetacriticScore { get; set; } = 0;
        public string MetacriticLink { get; set; } = ""; // from api metacriticLink="/pc/bioshock-infinite"change it into https://www.metacritic.com{metacriticLink}
        public int SteamRating { get; set; } = 0;
        public string SteamLink { get; set; } = "";

        public Review() { }

    }
}