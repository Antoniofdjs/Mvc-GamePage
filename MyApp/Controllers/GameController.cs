using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyApp.Data;
using MyApp.Services.IGDB;
using MyApp.Models;
using MyApp.Models.ViewModels;
using Newtonsoft.Json.Linq;
using OpenAI;
using MyApp.Models.Igdb;

namespace MyApp.Controllers
{
    public class GameController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly LocalData _localData;
        private readonly IgdbAPI _igdbAPI;
        private readonly TwitchTokenManager _twitchTokenManager;
        private IMemoryCache _cache;

        public GameController(ILogger<HomeController> logger, HttpClient httpClient, LocalData localData, TwitchTokenManager twitchTokenManager, IMemoryCache cache, IgdbAPI igdbApi)
        {
            _logger = logger;
            _httpClient = httpClient;
            _localData = localData;
            _cache = cache;
            _twitchTokenManager = twitchTokenManager;
            _igdbAPI = igdbApi;
        }

        public async Task<IActionResult> Summary(string gameTitle = "diablo-iv")
        {

            // Client-ID: Client ID
            string clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            string clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");

            //expiration check
            if (_twitchTokenManager.TokenExpiration == null || DateTime.Now >= _twitchTokenManager.TokenExpiration)
            {
                await _twitchTokenManager.RefreshToken();
            }
            // Authorization: Bearer access_token
            // Needs to be post method
            _httpClient.DefaultRequestHeaders.Add("Client-ID", clientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _twitchTokenManager.TokenValue);

            // var body = $"search \"{gameTitle}\"; fields name, slug; limit 50;";
            // var body = $"fields game_modes, name; where slug=\"{gameTitle}\";";
            // var body = "fields name, id, slug; where id<=80; limit 50;";
            var body = "fields *; limit 50;";

            var content = new StringContent(body, Encoding.UTF8, "text/plain");

            // Send the POST
            // var response = await _httpClient.PostAsync("https://api.igdb.com/v4/game_videos", content);
            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/game_modes", content);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseString);
            var gamesJson = JArray.Parse(responseString);
            // var gameJson = gamesJson[0];
            // var name = (string)gameJson["name"];
            // List<int> themes = gameJson["themes"].ToObject<List<int>>();
            // List<int> genres = gameJson["genres"].ToObject<List<int>>();
            // string summary = (string)gameJson["summary"];
            // string storyLine = (string)gameJson["storyline"];

            return Ok(responseString);
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

            Game game = new Game(); // cheapShark
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
            // Populate ViewModel
            IndexGameVM IndexGameVM = new IndexGameVM();
            IndexGameVM.Game = game;

            // Retrieve metacritic and steam ratings from cheapShark
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

            //Get IgdbDetails -------------------------------------------------------------------------------------------------------------
            string clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            string clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");

            // var body = "search \"Sid Meiers Civilization VI\"; fields name, slug; limit 50;";
            string gameTitle = game.Title; // Testing with this, static value
            string slugTitle = gameTitle.Replace(" -", "");
            slugTitle = slugTitle.Replace(' ', '-');
            slugTitle = slugTitle.Replace("-(2000)", "");
            slugTitle = slugTitle.Replace(":", "");
            slugTitle = slugTitle.Replace("'", "-");
            slugTitle = slugTitle.ToLower();
            Console.WriteLine($"My slug title is: {slugTitle}");

            var igdbResponse = await _igdbAPI.GameDetails(slugTitle); // Will fecth all details and video links
            if (!igdbResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"No results found for igdb slug {slugTitle}");
                TempData["Found"] = "Game Details Not Found";
                return View(IndexGameVM);
            }
            var igdbDetails = await igdbResponse.Content.ReadAsStringAsync();
            var jsonGameDetails = JObject.Parse(igdbDetails);

            IndexGameVM.IgdbDetails = new IgdbGameDetails(jsonGameDetails);

            IndexGameVM.ThemesGenres.AddRange(GetThemeNames(IndexGameVM.IgdbDetails.ThemesIDs));
            IndexGameVM.ThemesGenres.AddRange(GetGenreNames(IndexGameVM.IgdbDetails.GenresIDs));
            IndexGameVM.Platforms.AddRange(GetPlatformNames(IndexGameVM.IgdbDetails.PlatformsIDs));
            IndexGameVM.GameModes.AddRange(GetGameModeNames(IndexGameVM.IgdbDetails.GameModesIDs));

            Console.WriteLine("Found igdbDetails");
            return View(IndexGameVM);
        }

        // Helper functions
        public List<string> GetThemeNames(List<int> themesIds)
        {
            Console.WriteLine("Populating Theme Names");
            List<string> themeNames = new List<string>();

            foreach (var themeID in themesIds)
            {
                Console.WriteLine($"Theme ID populated: {themeID}");
                themeNames.Add(_localData.Themes
                    .Where(u => u.ID == themeID)
                    .Select(u => u.Name)
                    .FirstOrDefault());
            }
            return themeNames;
        }

        public List<string> GetGameModeNames(List<int> gameModesIDs)
        {
            Console.WriteLine("Populating GameMode Names");
            List<string> gameModeNames = new List<string>();

            foreach (var gameModeID in gameModesIDs)
            {
                Console.WriteLine($"Theme ID: {gameModeID}");
                gameModeNames.Add(_localData.GameModes
                    .Where(u => u.ID == gameModeID)
                    .Select(u => u.Name)
                    .FirstOrDefault());
            }

            return gameModeNames;
        }

        public List<string> GetGenreNames(List<int> genreIds)
        {
            Console.WriteLine("Populating Genre Names");
            List<string> genreNames = new List<string>();

            foreach (var genreID in genreIds)
            {
                Console.WriteLine($"Theme ID: {genreID}");
                genreNames.Add(_localData.Genres
                    .Where(u => u.ID == genreID)
                    .Select(u => u.Name)
                    .FirstOrDefault());
            }

            return genreNames;
        }

        public List<string> GetPlatformNames(List<int> platformsIDS)
        {
            Console.WriteLine("Populating Platform Names");
            List<string> platformNames = new List<string>();

            foreach (var platformID in platformsIDS)
            {
                Console.WriteLine($"Theme ID: {platformID}");
                platformNames.Add(_localData.Platforms
                    .Where(u => u.ID == platformID)
                    .Select(u => u.Name)
                    .FirstOrDefault());
            }
            return platformNames;
        }
    }
}