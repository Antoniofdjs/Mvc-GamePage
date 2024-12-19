namespace MyApp.Models
{

    public class MainDeal
    {

        public string Title { get; set; }
        public string DealID { get; set; }
        public string StoreName { get; set; }
        public int StoreID { get; set; }
        public int GameID { get; set; }
        public double SalePrice { get; set; }
        public double NormalPrice { get; set; }
        public double Savings { get; set; }
        public double MetacriticScore { get; set; }
        public double SteamRatingPercent { get; set; }
        public string Thumb { get; set; }



        public MainDeal() { }

        public MainDeal(string title, string dealID, int storeID, int gameID,
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