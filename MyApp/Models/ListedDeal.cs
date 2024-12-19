using Newtonsoft.Json.Linq;

namespace MyApp.Models
{

    // deal data from accesing https://www.cheapshark.com/api/1.0/games?id={}
    // /games provides  "info", "cheapestPriceEver" and array "deals"
    // These deals are shorter than the hot deals at the home page, 
    // we are naming them "ListedDeals"
    public class ListedDeal
    {

        public int StoreID { get; set; }
        public string DealID { get; set; }
        public double Price { get; set; }
        public double RetailPrice { get; set; }
        public double Savings { get; set; }
        public string StoreName { get; set; }


        public ListedDeal(JObject listedDeal) // api doesnt give store names, we populate that in the controller using localData

        {
            StoreID = (int)listedDeal["storeID"];
            DealID = (string)listedDeal["dealID"];
            Price = (double)listedDeal["price"];
            RetailPrice = (double)listedDeal["retailPrice"];
            Savings = (double)listedDeal["savings"];
            // StoreName will be selected from the localData
        }
    }
}