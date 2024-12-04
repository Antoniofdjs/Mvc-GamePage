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

        public async Task<IActionResult> Index(int gameID)
        {
            var response = await _httpClient.GetAsync($"https://www.cheapshark.com/api/1.0/games?id={gameID}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JArray.Parse(jsonString);
            var jsonDeals = jsonGame["deals"];

            Game game = new Game();
            {
                game.Title = (string)jsonGame["info"]["title"];
                game.Deals = new List<Deal>();
                foreach (var jsondeal in jsonDeals)
                {
                    var deal = new Deal
                    {
                        StoreID = (int)jsondeal["storeID"],
                        DealID = (string)jsondeal["dealID"],
                        SalePrice = (double)jsondeal["salePrice"],
                        NormalPrice = (double)jsondeal["normalPrice"],
                        Savings = (double)jsondeal["savings"]
                    };
                    game.Deals.Add(deal);
                }

            }
            return View(game);
        }
    }
}