using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyApp.Data;
using MyApp.Models;
using Newtonsoft.Json.Linq;

namespace MyApp.Controllers
{
    public class GameController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly LocalData _localData;
        private IMemoryCache _cache;

        public GameController(ILogger<HomeController> logger, HttpClient httpClient, LocalData localData, IMemoryCache cache)
        {
            _logger = logger;
            _httpClient = httpClient;
            _localData = localData;
            _cache = cache;
        }

        public async Task<IActionResult> Index(int gameID, string searchTitle, string returnTo = "Home")
        {
            ViewBag.returnTo = returnTo;
            ViewBag.searchTitle = searchTitle;

            var response = await _httpClient.GetAsync($"https://www.cheapshark.com/api/1.0/games?id={gameID}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JObject.Parse(jsonString);
            var jsonDeals = jsonGame["deals"];

            Game game = new Game();
            {
                game.Title = (string)jsonGame["info"]["title"];
                game.Thumb = (string)jsonGame["info"]["thumb"];
                game.Deals = new List<Deal>();
                game.Review = new Review();

                foreach (var jsondeal in jsonDeals)
                {
                    int storeID = (int)jsondeal["storeID"];
                    var store = _localData.Stores.FirstOrDefault(u => u.Id == storeID);
                    string storeName = store.Name;

                    var deal = new Deal
                    {
                        StoreID = storeID,
                        StoreName = storeName,
                        DealID = (string)jsondeal["dealID"],
                        SalePrice = (double)jsondeal["price"],
                        NormalPrice = (double)jsondeal["retailPrice"],
                        Savings = (double)jsondeal["savings"]
                    };
                    game.Deals.Add(deal);
                }

            }

            // Retrieve metacritic and steam ratings
            string dealID = game.Deals[0].DealID;
            var responseDealSearch = await _httpClient.GetAsync($"https://www.cheapshark.com/api/1.0/deals?id={dealID}");
            if (responseDealSearch.IsSuccessStatusCode)
            {
                var jsonStringDealSearch = await responseDealSearch.Content.ReadAsStringAsync();
                var jsonDealSearch = JObject.Parse(jsonStringDealSearch);

                game.Review.MetacriticScore = (int)jsonDealSearch["gameInfo"]["metacriticScore"];
                game.Review.MetaCriticLink = (string)jsonDealSearch["gameInfo"]["metacriticLink"];
                game.Review.SteamRating = (int)jsonDealSearch["gameInfo"]["steamRatingPercent"];
            }

            return View(game);
        }
    }
}