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
            var body = $"fields *; where id=170; limit 50;";

            var content = new StringContent(body, Encoding.UTF8, "text/plain");

            // Send the POST
            // var response = await _httpClient.PostAsync("https://api.igdb.com/v4/game_videos", content);
            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/platforms", content);
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

            IndexGameVM indexGameVM = new IndexGameVM();

            indexGameVM.Game = new Game2((JObject)jsonGame["info"]);
            foreach (var jsonDeal in (JArray)jsonGame["deals"])
            {
                Deal deal = new Deal((JObject)jsonDeal);
                deal.StoreName = _localData.Stores.Where(u => u.Id == deal.StoreID).Select(u => u.Name).FirstOrDefault();
                indexGameVM.Deals.Add(deal); // oficial deal for the view
            }

            // Retrieve metacritic and steam ratings from cheapShark
            string dealID = indexGameVM.Deals[0].DealID;
            var responseDealSearch = await _httpClient.GetAsync($"https://www.cheapshark.com/api/1.0/deals?id={dealID}");
            if (responseDealSearch.IsSuccessStatusCode)
            {
                Review2 reviewMetaCritic = new Review2();

                var jsonStringDealSearch = await responseDealSearch.Content.ReadAsStringAsync();
                var jsonDealSearch = JObject.Parse(jsonStringDealSearch);

                reviewMetaCritic.Name = "MetaCritic";
                reviewMetaCritic.RatingScore = (int)jsonDealSearch["gameInfo"]["metacriticScore"];
                var linkMetaCritic = (string)jsonDealSearch["gameInfo"]["metacriticLink"];
                reviewMetaCritic.RatingLink = $"https://www.metacritic.com{linkMetaCritic}";

                if (reviewMetaCritic.RatingScore > 0)
                {
                    indexGameVM.Reviews.Add(reviewMetaCritic);
                }

                if (indexGameVM.Game.SteamAppID != 0)
                {
                    Review2 reviewSteam = new Review2();
                    reviewSteam.Name = "Steam";
                    reviewSteam.RatingScore = (int)jsonDealSearch["gameInfo"]["steamRatingPercent"];
                    var steamAppID = indexGameVM.Game.SteamAppID;
                    reviewSteam.RatingLink = $"https://store.steampowered.com/app/{steamAppID}/#app_reviews_hash";
                    indexGameVM.Reviews.Add(reviewSteam);
                }
            }

            // Fetch igdb details
            string slugTitle = SanatizeTitleSlug(indexGameVM.Game.Title);
            Console.WriteLine($"My slug title is: {slugTitle}");

            var igdbResponse = await _igdbAPI.GameDetails(slugTitle);
            if (!igdbResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"No results found for igdb slug {slugTitle}");
                TempData["Found"] = "Game Details Not Found";
                return View(indexGameVM);
            }
            var igdbDetails = await igdbResponse.Content.ReadAsStringAsync();
            var jsonGameDetails = JObject.Parse(igdbDetails);

            // Prepare ViewModel IgdbDetails and set review for igdb
            indexGameVM.IgdbDetails = new IgdbGameDetails(jsonGameDetails);
            indexGameVM.ThemesGenres.AddRange(GetThemeNames(indexGameVM.IgdbDetails.ThemesIDs));
            indexGameVM.ThemesGenres.AddRange(GetGenreNames(indexGameVM.IgdbDetails.GenresIDs));
            indexGameVM.Platforms.AddRange(GetPlatformNames(indexGameVM.IgdbDetails.PlatformsIDs));
            indexGameVM.GameModes.AddRange(GetGameModeNames(indexGameVM.IgdbDetails.GameModesIDs));
            if (indexGameVM.IgdbDetails.RatingCount > 0)
            {
                Review2 igdbReview = new Review2()
                {

                    RatingScore = indexGameVM.IgdbDetails.RatingCount,
                    RatingLink = indexGameVM.IgdbDetails.RatingLink,
                    Name = "IGDB",

                };
                indexGameVM.Reviews.Add(igdbReview);
            }

            // Fetch igdb multiplayer modes
            var igdbMultiplayerResponse = await _igdbAPI.MultiPlayerModes(indexGameVM.IgdbDetails.GameID);
            if (!igdbMultiplayerResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error awaiting /multiplayer_modes {igdbMultiplayerResponse.StatusCode}");
                return View(indexGameVM);
            }
            var jsonMultiplayerString = await igdbMultiplayerResponse.Content.ReadAsStringAsync();
            var jsonMultiplayerModes = JObject.Parse(jsonMultiplayerString);
            IgdbMultiplayerMode igdbMultiplayerModes = new IgdbMultiplayerMode(jsonMultiplayerModes);
            indexGameVM.MultiPlayerModes.AddRange(GetMultiplayerModes(igdbMultiplayerModes));

            Console.WriteLine("Found igdbDetails");
            return View(indexGameVM);
        }


        // public async Task<IActionResult> Index(int gameID, string searchTitle, string returnTo = "Home")
        // {
        //     ViewBag.returnTo = returnTo;
        //     ViewBag.searchTitle = searchTitle;

        //     var response = await _httpClient.GetAsync($"https://www.cheapshark.com/api/1.0/games?id={gameID}");
        //     if (!response.IsSuccessStatusCode)
        //     {
        //         return NotFound();
        //     }

        //     var jsonString = await response.Content.ReadAsStringAsync();
        //     var jsonGame = JObject.Parse(jsonString);
        //     var jsonDeals = jsonGame["deals"];

        //     Game game = new Game(); // cheapShark
        //     {
        //         game.Title = (string)jsonGame["info"]["title"];
        //         var appID = jsonGame["info"]["steamAppID"];
        //         if (appID != null && appID.Type != JTokenType.Null)
        //         {
        //             game.SteamAppID = (int)appID;
        //         }
        //         game.Thumb = (string)jsonGame["info"]["thumb"];
        //         game.Deals = new List<MainDeal>();
        //         game.Review = new Review();

        //         foreach (var jsondeal in jsonDeals)
        //         {
        //             int storeID = (int)jsondeal["storeID"];
        //             var store = _localData.Stores.FirstOrDefault(u => u.Id == storeID);
        //             string storeName = store.Name;

        //             var deal = new MainDeal
        //             {
        //                 StoreID = storeID,
        //                 StoreName = storeName,
        //                 DealID = (string)jsondeal["dealID"],
        //                 SalePrice = (double)jsondeal["price"],
        //                 NormalPrice = (double)jsondeal["retailPrice"],
        //                 Savings = (double)jsondeal["savings"]
        //             };
        //             game.Deals.Add(deal);
        //         }

        //     }
        //     // Populate ViewModel
        //     IndexGameVM IndexGameVM = new IndexGameVM();
        //     IndexGameVM.Game = game;

        //     // Retrieve metacritic and steam ratings from cheapShark
        //     string dealID = game.Deals[0].DealID;
        //     var responseDealSearch = await _httpClient.GetAsync($"https://www.cheapshark.com/api/1.0/deals?id={dealID}");
        //     if (responseDealSearch.IsSuccessStatusCode)
        //     {
        //         var jsonStringDealSearch = await responseDealSearch.Content.ReadAsStringAsync();
        //         var jsonDealSearch = JObject.Parse(jsonStringDealSearch);

        //         game.Review.MetacriticScore = (int)jsonDealSearch["gameInfo"]["metacriticScore"];
        //         var linkMetaCritic = (string)jsonDealSearch["gameInfo"]["metacriticLink"];
        //         game.Review.MetacriticLink = $"https://www.metacritic.com{linkMetaCritic}";

        //         if (game.SteamAppID != 0)
        //         {
        //             game.Review.SteamRating = (int)jsonDealSearch["gameInfo"]["steamRatingPercent"];
        //             game.Review.SteamLink = $"https://store.steampowered.com/app/{game.SteamAppID}/#app_reviews_hash";
        //         }
        //     }

        //     // Fetch igdb details
        //     string slugTitle = SanatizeTitleSlug(game.Title);
        //     Console.WriteLine($"My slug title is: {slugTitle}");

        //     var igdbResponse = await _igdbAPI.GameDetails(slugTitle);
        //     if (!igdbResponse.IsSuccessStatusCode)
        //     {
        //         Console.WriteLine($"No results found for igdb slug {slugTitle}");
        //         TempData["Found"] = "Game Details Not Found";
        //         return View(IndexGameVM);
        //     }
        //     var igdbDetails = await igdbResponse.Content.ReadAsStringAsync();
        //     var jsonGameDetails = JObject.Parse(igdbDetails);

        //     // Prepare ViewModel IgdbDetails
        //     IndexGameVM.IgdbDetails = new IgdbGameDetails(jsonGameDetails);
        //     IndexGameVM.ThemesGenres.AddRange(GetThemeNames(IndexGameVM.IgdbDetails.ThemesIDs));
        //     IndexGameVM.ThemesGenres.AddRange(GetGenreNames(IndexGameVM.IgdbDetails.GenresIDs));
        //     IndexGameVM.Platforms.AddRange(GetPlatformNames(IndexGameVM.IgdbDetails.PlatformsIDs));
        //     IndexGameVM.GameModes.AddRange(GetGameModeNames(IndexGameVM.IgdbDetails.GameModesIDs));

        //     // Fetch igdb multiplayer modes
        //     var igdbMultiplayerResponse = await _igdbAPI.MultiPlayerModes(IndexGameVM.IgdbDetails.GameID);
        //     if (!igdbMultiplayerResponse.IsSuccessStatusCode)
        //     {
        //         Console.WriteLine($"Error awaiting /multiplayer_modes {igdbMultiplayerResponse.StatusCode}");
        //         return View(IndexGameVM);
        //     }
        //     var jsonMultiplayerString = await igdbMultiplayerResponse.Content.ReadAsStringAsync();
        //     var jsonMultiplayerModes = JObject.Parse(jsonMultiplayerString);
        //     IgdbMultiplayerMode igdbMultiplayerModes = new IgdbMultiplayerMode(jsonMultiplayerModes);
        //     IndexGameVM.MultiPlayerModes.AddRange(GetMultiplayerModes(igdbMultiplayerModes));

        //     Console.WriteLine("Found igdbDetails");
        //     return View(IndexGameVM);
        // }

        // HELPER FUNCTIONS ---------------------------------------------------------------------------------
        private string SanatizeTitleSlug(string gameTitle)
        {

            string slugTitle = gameTitle.Replace(" -", "");
            slugTitle = slugTitle.ToLower();
            slugTitle = slugTitle.Replace(".", "");
            slugTitle = slugTitle.Replace(' ', '-');
            slugTitle = slugTitle.Replace("-(2000)", "");
            slugTitle = slugTitle.Replace(":", "");
            slugTitle = slugTitle.Replace("'", "-");
            return slugTitle;
        }

        private List<string> GetMultiplayerModes(IgdbMultiplayerMode multiplayerModes)
        {
            Console.WriteLine("Preparing multiplayer modes list");
            var modes = new List<(bool condition, string description)>
            {
                (multiplayerModes.OnlineMax > 0, $"Online Max: {multiplayerModes.OnlineMax}"),
                (multiplayerModes.OnlineCoop, $"Online Co-op Max: {multiplayerModes.OnlinecoopMax}"),
                (multiplayerModes.SplitsScreenOnline, "Split Screen Online"),
                (multiplayerModes.SplitScreen, "Split Screen"),
                (multiplayerModes.CampaingCoop, "Campaign Co-op"),
                (multiplayerModes.LanCoop, "LAN Co-op"),
                (multiplayerModes.OfflineCoop, $"Offline Co-op Max: {multiplayerModes.OfflineCoopmax}"),
                (multiplayerModes.OfflineMax > 0, $"Offline Max: {multiplayerModes.OfflineMax}"),
            };

            return modes
                .Where(mode => mode.condition)
                .OrderByDescending(mode => mode.description)
                .Select(mode => mode.description)
                .ToList();
        }


        private List<string> GetThemeNames(List<int> themesIds)
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

        private List<string> GetGameModeNames(List<int> gameModesIDs)
        {
            Console.WriteLine("Populating GameMode Names");
            List<string> gameModeNames = new List<string>();

            foreach (var gameModeID in gameModesIDs)
            {
                Console.WriteLine($"GameMode ID: {gameModeID}");
                gameModeNames.Add(_localData.GameModes
                    .Where(u => u.ID == gameModeID)
                    .Select(u => u.Name)
                    .FirstOrDefault());
            }

            return gameModeNames;
        }

        private List<string> GetGenreNames(List<int> genreIds)
        {
            Console.WriteLine("Populating Genre Names");
            List<string> genreNames = new List<string>();

            foreach (var genreID in genreIds)
            {
                Console.WriteLine($"Genre ID: {genreID}");
                genreNames.Add(_localData.Genres
                    .Where(u => u.ID == genreID)
                    .Select(u => u.Name)
                    .FirstOrDefault());
            }

            return genreNames;
        }

        private List<string> GetPlatformNames(List<int> platformsIDS)
        {
            Console.WriteLine("Populating Platform Names");
            List<string> platformNames = new List<string>();

            foreach (var platformID in platformsIDS)
            {
                Console.WriteLine($"Platform ID: {platformID}");
                platformNames.Add(_localData.Platforms
                    .Where(u => u.ID == platformID)
                    .Select(u => u.Name)
                    .FirstOrDefault());
            }
            return platformNames;
        }
    }
}