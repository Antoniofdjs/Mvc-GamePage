using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyApp.Models;
using MyApp.Models.ViewModels;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MyApp.Data;
using Microsoft.Extensions.Caching.Memory;

namespace MyApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient; // Declare HttpClient
        private readonly LocalData _localData;
        private IMemoryCache _cache;


        // Modify the constructor to accept HttpClient
        public HomeController(ILogger<HomeController> logger, HttpClient httpClient, LocalData localData, IMemoryCache cache)
        {
            _logger = logger;
            _httpClient = httpClient;
            _localData = localData;
            _cache = cache;
        }

        public async Task<IActionResult> Index(int storeId = 1)
        {

            string cacheKey = $"indexVM{storeId}"; // Key for storing cached data
            if (_cache.TryGetValue(cacheKey, out IndexVM indexVMCache))
            {
                Console.WriteLine($"CACHE ACTIVATED USING CACHE FOR STORE {storeId}");
                return View(indexVMCache);
            }

            var response = await _httpClient.GetAsync($"https://www.cheapshark.com/api/1.0/deals?storeID={storeId}&upperPrice=15");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var jsonDeals = JArray.Parse(jsonString);

            List<MainDeal> deals = new List<MainDeal>();
            foreach (var jsonDeal in jsonDeals)
            {
                var deal = new MainDeal
                {
                    Title = (string)jsonDeal["title"],
                    GameID = (int)jsonDeal["gameID"],
                    DealID = (string)jsonDeal["dealID"],
                    StoreID = (int)jsonDeal["storeID"],
                    SalePrice = (double)jsonDeal["salePrice"],
                    NormalPrice = (double)jsonDeal["normalPrice"],
                    Thumb = (string)jsonDeal["thumb"],
                    MetacriticScore = (double)jsonDeal["metacriticScore"],
                    SteamRatingPercent = (double)jsonDeal["steamRatingPercent"],
                    Savings = (double)jsonDeal["savings"]
                };

                deals.Add(deal);
            }

            IndexVM indexVM = new IndexVM();
            indexVM.Deals = deals.OrderByDescending(u => u.Savings).ToList();
            indexVM.StoreId = storeId;// id from https request

            _cache.Set(cacheKey, indexVM, TimeSpan.FromMinutes(10));
            return View(indexVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
