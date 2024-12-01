namespace MyApp.Models
{

    public class Deal
    {

        public string Title { get; set; } // steam 1, Amazon 4, Gamestop 5, Origin 8, Epic Games Store 25
        public string DealID { get; set; }
        public int StoreID { get; set; }
        public int GameID { get; set; }
        public double SalePrice { get; set; }
        public double NormalPrice { get; set; }
        public double Savings { get; set; }
        public double MetacriticScore { get; set; }
        public double SteamRatingPercent { get; set; }
        public string Thumb { get; set; }



        public Deal() { }

        public Deal(string title, string dealID, int storeID, int gameID,
        double salePrice, double normalPrice, double savings,
        double metacriticScore, double steamRatingPercent, string thumb)
        {
            Title = title;
            DealID = dealID;
            StoreID = storeID;
            GameID = gameID;
            SalePrice = salePrice;
            NormalPrice = normalPrice;
            Savings = savings;
            MetacriticScore = metacriticScore;
            SteamRatingPercent = steamRatingPercent;
            Thumb = thumb;

        }
    }
}