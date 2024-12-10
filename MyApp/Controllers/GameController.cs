using System.Net.Http.Headers;
using System.Text;
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

        public async Task<IActionResult> Summary()
        {

            // Client-ID: Client ID
            string clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            string clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
            Console.WriteLine("My client id is: ");
            Console.WriteLine(clientId);

            // Authorization: Bearer access_token

            // Needs to be post method
            _httpClient.DefaultRequestHeaders.Add("Client-ID", clientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "AccesTokenHEREEEEEEEEEE");
            var body = "fields video_id;";
            var content = new StringContent(body, Encoding.UTF8, "text/plain");

            // Send the POST
            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/game_videos", content);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var responseData = await response.Content.ReadAsStringAsync();
            return View();
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
                var appID = jsonGame["info"]["steamAppID"];
                if (appID != null && appID.Type != JTokenType.Null)
                {
                    game.SteamAppID = (int)appID;
                }
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
                var linkMetaCritic = (string)jsonDealSearch["gameInfo"]["metacriticLink"];
                game.Review.MetacriticLink = $"https://www.metacritic.com{linkMetaCritic}";

                if (game.SteamAppID != 0)
                {
                    game.Review.SteamRating = (int)jsonDealSearch["gameInfo"]["steamRatingPercent"];
                    game.Review.SteamLink = $"https://store.steampowered.com/app/{game.SteamAppID}/#app_reviews_hash";
                }
            }

            return View(game);
        }
    }
}