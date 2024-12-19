namespace MyApp.Models
{

    public class Review2 // Used for storing the steam and metacritic reviews
    {
        public string Name { get; set; }
        public int RatingScore { get; set; } = 0;
        public string RatingLink { get; set; } = ""; // from api metacriticLink="/pc/bioshock-infinite"change it into https://www.metacritic.com{metacriticLink}

        public Review2() { }

    }
}