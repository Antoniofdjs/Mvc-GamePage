using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyApp.Data;
using MyApp.Models;
using MyApp.Models.ViewModels;
using Newtonsoft.Json.Linq;

namespace MyApp.Controllers
{
    public class GameController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly LocalData _localData;

        private readonly TwitchTokenManager _twitchTokenManager;
        private IMemoryCache _cache;

        public GameController(ILogger<HomeController> logger, HttpClient httpClient, LocalData localData, TwitchTokenManager twitchTokenManager, IMemoryCache cache)
        {
            _logger = logger;
            _httpClient = httpClient;
            _localData = localData;
            _cache = cache;
            _twitchTokenManager = twitchTokenManager;
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
            // var body = $"fields name, screenshots, videos, themes, genres, category, summary, storyline, slug, dlcs, rating; where slug=\"{gameTitle}\";";
            // var body = "fields name, id, slug; where id<=80; limit 50;";
            var body = "fields *; where id=30864;";

            var content = new StringContent(body, Encoding.UTF8, "text/plain");

            // Send the POST
            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/game_videos", content);
            // var response = await _httpClient.PostAsync("https://api.igdb.com/v4/themes", content);
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

            // IndexGameVM IndexGameVM = new IndexGameVM();
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
            // Client-ID: Client ID
            string clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            string clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");

            //expiration check
            if (_twitchTokenManager.TokenExpiration == null || DateTime.Now >= _twitchTokenManager.TokenExpiration)
            {
                await _twitchTokenManager.RefreshToken();
            }
            _httpClient.DefaultRequestHeaders.Add("Client-ID", clientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _twitchTokenManager.TokenValue);

            // var body = "search \"Sid Meiers Civilization VI\"; fields name, slug; limit 50;";
            string gameTitle = game.Title; // Testing with this, static value
            string slugTitle = gameTitle.Replace(' ', '-');
            slugTitle = slugTitle.Replace(":", ""); // Remove colons
            slugTitle = slugTitle.Replace("'", "-"); // Remove colons
            slugTitle = slugTitle.ToLower();
            Console.WriteLine($"My slug title is: {slugTitle}");

            var body = $"fields id, name, screenshots, videos, themes, genres, category, summary, storyline, slug, dlcs, rating; where slug=\"{slugTitle}\";";
            // var body = "fields name, id, slug; where id<=80; limit 50;";
            // var body = "fields *; where id=337510;";

            var content = new StringContent(body, Encoding.UTF8, "text/plain");

            // Send the POST
            var responseIgdb = await _httpClient.PostAsync("https://api.igdb.com/v4/games", content);
            // var response = await _httpClient.PostAsync("https://api.igdb.com/v4/screenshots", content);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var responseIgdbString = await responseIgdb.Content.ReadAsStringAsync();
            Console.WriteLine(responseIgdbString);
            var gamesJson = JArray.Parse(responseIgdbString);
            var gameJson = gamesJson[0];

            var name = (string)gameJson["name"];
            var gameId = (string)gameJson["id"];
            var rating = (int)gameJson["rating"];
            var slug = (string)gameJson["slug"];
            List<int> videosIDs = gameJson["videos"].ToObject<List<int>>();
            string summary = (string)gameJson["summary"];
            string storyLine = (string)gameJson["storyline"];
            List<int> themesIDs = gameJson["themes"].ToObject<List<int>>();
            List<int> genresIDs = gameJson["genres"].ToObject<List<int>>();


            List<string> themesGenres = new List<string>();
            List<string> videoURLs = new List<string>();
            Console.WriteLine($"MY SLUG IS: {slug}");
            // Prepare themes, genres ,video links for youtbe, and screenshot links
            foreach (var themeID in themesIDs)
            {
                Console.WriteLine($"Theme ID: {themeID}");
                themesGenres.Add(_localData.Themes.Where(u => u.ID == themeID).Select(u => u.Name).FirstOrDefault());
            }
            foreach (var genreID in genresIDs)
            {
                Console.WriteLine($"Genre ID: {genreID}");
                themesGenres.Add(_localData.Genres.Where(u => u.ID == genreID).Select(u => u.Name).FirstOrDefault());
            }

            var body2 = $"fields video_id; where game={gameId}; limit 8;";
            var content2 = new StringContent(body2, Encoding.UTF8, "text/plain");
            var responseVideosIgdb = await _httpClient.PostAsync("https://api.igdb.com/v4/game_videos", content2);

            var jsonStringVideos = await responseVideosIgdb.Content.ReadAsStringAsync();
            var jsonVideosLinksIds = JArray.Parse(jsonStringVideos);

            List<string> youtubeURLs = new List<string>();
            foreach (var videoJson in jsonVideosLinksIds)
            {
                var videoId = videoJson["video_id"];
                var videoLink = $"https://www.youtube.com/embed/{videoId}";
                youtubeURLs.Add(videoLink);
            }

            // Populate ViewModel
            IndexGameVM IndexGameVM = new IndexGameVM();

            IndexGameVM.Game = game;
            IndexGameVM.IgdbDetails.SlugTitle = slug;
            IndexGameVM.IgdbDetails.StoryLine = storyLine;
            IndexGameVM.IgdbDetails.Summary = summary;
            IndexGameVM.IgdbDetails.ThemesGenres = themesGenres;
            IndexGameVM.IgdbDetails.RatingCount = rating;
            IndexGameVM.IgdbDetails.RatingLink = $"https://www.igdb.com/games/{IndexGameVM.IgdbDetails.SlugTitle}#community";
            IndexGameVM.IgdbDetails.VideosLinks = youtubeURLs;

            return View(IndexGameVM); // devolver IndexGameVM
            //IndexGameVM.Game
            //IndexGameVM.IgdbDetails
        }
    }
}