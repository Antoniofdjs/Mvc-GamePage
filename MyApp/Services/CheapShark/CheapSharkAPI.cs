using System.Collections;
using System.Net.Http.Headers;
using System.Text;
using MyApp.Models.Igdb;
using System.Text.Json.Nodes;
using MyApp.Data;
using MyApp.Models;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
namespace MyApp.Services.IGDB
{

    // All igdb api request endpoints will be made here
    public class CheapSharkAPI
    {
        private readonly HttpClient _httpClient;
        private readonly TwitchTokenManager _twitchTokenManager;
        private readonly LocalData _localData;

        public CheapSharkAPI(HttpClient httpClient, TwitchTokenManager twitchTokenManager, LocalData localData)
        {

            _httpClient = httpClient;
            _twitchTokenManager = twitchTokenManager;
            _localData = localData;
        }



        public async Task<HttpResponseMessage> Game(int gameID)
        {

            var response = await _httpClient.GetAsync($"https://www.cheapshark.com/api/1.0/games?id={gameID}");
            if (!response.IsSuccessStatusCode)
            {
                return response;
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var jsonGame = JObject.Parse(jsonString);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonGame.ToString(), Encoding.UTF8, "application/json")
            };
        }
    }



}