using Newtonsoft.Json.Linq;
namespace MyApp.Models
{

    public class Deal
    {

        public string StoreName { get; set; }
        public string DealID { get; set; }
        public int StoreID { get; set; }
        public double Price { get; set; }
        public double RetailPrice { get; set; }
        public double Savings { get; set; }


        public Deal() { }

        public Deal(JObject listedDeal) // api doesnt give store names, we populate that in the controller using localData

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