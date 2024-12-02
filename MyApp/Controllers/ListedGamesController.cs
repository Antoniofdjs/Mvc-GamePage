using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyApp.Data;
using MyApp.Models;
using Newtonsoft.Json.Linq;

namespace MyApp.Controllers
{
    public class ListedGamesController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient; // Declare HttpClient
        private readonly LocalData _localData;
        private IMemoryCache _cache;

        public ListedGamesController(ILogger<HomeController> logger, HttpClient httpClient, LocalData localData, IMemoryCache cache)
        {
            _logger = logger;
            _httpClient = httpClient;
            _localData = localData;
            _cache = cache;
        }

        public async Task<IActionResult> Index(string title)
        {
            var response = await _httpClient.GetAsync($"https://www.cheapshark.com/api/1.0/games?title={title}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var jsonListedGames = JArray.Parse(jsonString);
            List<ListedGame> listedGames = new List<ListedGame>();

            foreach (var jsonGame in jsonListedGames)
            {
                ListedGame listedGame = new ListedGame();
                listedGame.Title = (string)jsonGame["external"];
                listedGame.GameID = (int)jsonGame["gameID"];
                listedGame.CheapestDealID = (string)jsonGame["cheapestDealID"];
                listedGame.CheapestPrice = (double)jsonGame["cheapest"];
                listedGame.Thumb = (string)jsonGame["thumb"];

                listedGames.Add(listedGame);
            }

            return View(listedGames);
        }
    }
}