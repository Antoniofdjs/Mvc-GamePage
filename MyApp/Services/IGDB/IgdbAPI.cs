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
    public class IgdbAPI
    {
        private readonly HttpClient _httpClient;
        private readonly TwitchTokenManager _twitchTokenManager;
        private readonly LocalData _localData;

        public IgdbAPI(HttpClient httpClient, TwitchTokenManager twitchTokenManager, LocalData localData)
        {

            _httpClient = httpClient;
            _twitchTokenManager = twitchTokenManager;
            _localData = localData;
        }


        // Helper method to refresh the token and set headers
        private async Task<bool> PrepareRequestAsync()
        {
            string clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            string clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
            if (_twitchTokenManager.TokenExpiration == null || DateTime.Now >= _twitchTokenManager.TokenExpiration)
            {
                bool ok = await _twitchTokenManager.RefreshToken();
                if (!ok)
                {
                    return false;
                }
            }

            // Clear existing headers to avoid duplication
            _httpClient.DefaultRequestHeaders.Clear();

            // Set the headers for the request
            _httpClient.DefaultRequestHeaders.Add("Client-ID", clientId);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _twitchTokenManager.TokenValue);
            return true;
        }

        public async Task<HttpResponseMessage> GameDetails<T>(T slugOrGameID)
        {
            if (!(typeof(T) == typeof(string) || typeof(T) == typeof(int)))
            {
                throw new ArgumentException("Invalid type. Only string or int is allowed.");
            }

            bool tokenSucces = await PrepareRequestAsync();
            if (!tokenSucces)
            {
                throw new InvalidOperationException("Failed to refresh token.");
            }

            string body = "";
            if (typeof(T) == typeof(string)) // using slugTitle
            {
                body = $"fields *; where slug=\"{slugOrGameID}\";";
            }
            if (typeof(T) == typeof(int)) // using gameID
            {
                body = $"fields *; where id={slugOrGameID};";
            }
            var content = new StringContent(body, Encoding.UTF8, "text/plain");

            // Send the POST
            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/games", content);
            if (!response.IsSuccessStatusCode)
            {
                return response;
            }

            var responseIgdbString = await response.Content.ReadAsStringAsync();
            var gamesJson = JArray.Parse(responseIgdbString);
            if (gamesJson.Count <= 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("{\"Message\": \"Error\"}", Encoding.UTF8, "application/json")
                };

            }
            var gameJson = gamesJson[0];

            // Lists/arrays for the JObject
            List<int> platformsIds = gameJson["platforms"]?.ToObject<List<int>>() ?? new List<int>();
            List<int> themesIds = gameJson["themes"]?.ToObject<List<int>>() ?? new List<int>();
            List<int> genresIds = gameJson["genres"]?.ToObject<List<int>>() ?? new List<int>();
            List<int> gameModesIds = gameJson["game_modes"]?.ToObject<List<int>>() ?? new List<int>();

            JObject result = new JObject
            {
                ["id"] = gameJson.Value<string>("id") ?? "",
                ["slug"] = gameJson.Value<string>("slug") ?? "",
                ["name"] = gameJson.Value<string>("name") ?? "",
                ["storyline"] = gameJson.Value<string>("storyline") ?? "",
                ["summary"] = gameJson.Value<string>("summary") ?? "",
                ["rating"] = gameJson.Value<int?>("rating") ?? 0,
                ["platforms"] = JArray.FromObject(platformsIds),
                ["themes"] = JArray.FromObject(themesIds),
                ["genres"] = JArray.FromObject(genresIds),
                ["game_modes"] = JArray.FromObject(gameModesIds)
            };

            var myResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(result.ToString(), Encoding.UTF8, "application/json")
            };

            Console.WriteLine("Response succes, json fetched from api/games");
            return myResponse;
        }


        public async Task<HttpResponseMessage> MultiPlayerModes(string gameID)
        {
            bool tokenSucces = await PrepareRequestAsync();
            if (!tokenSucces)
            {
                throw new InvalidOperationException("Failed to refresh token.");
            }

            var body = $"fields *; where id={gameID};";
            var content = new StringContent(body, Encoding.UTF8, "text/plain");

            // Send the POST
            Console.WriteLine($"Sending post to /multiplayer_modes for game id {gameID}");
            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/multiplayer_modes", content);
            if (!response.IsSuccessStatusCode)
            {
                return response;
            }
            Console.WriteLine($"Succes, response received from /multiplayer_modes");
            var jsonString = await response.Content.ReadAsStringAsync();
            var jsonMultiplayers = JArray.Parse(jsonString) ?? null;
            if (jsonMultiplayers.Count <= 0)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("{\"Message\": \"Error\"}", Encoding.UTF8, "application/json")
                };
            }

            var jsonMultiplayer = jsonMultiplayers[0];
            JObject multiplayerModesJObject = new JObject
            {
                ["campaingcoop"] = jsonMultiplayer.Value<bool>("campaigncoop"),
                ["lancoop"] = jsonMultiplayer.Value<bool>("lancoop"),
                ["offlinecoop"] = jsonMultiplayer.Value<bool>("offlinecoop"),
                ["offlinemax"] = jsonMultiplayer.Value<int>("offlinemax"),
                ["onlinecoop"] = jsonMultiplayer.Value<bool>("onlinecoop"),
                ["onlinecoopmax"] = jsonMultiplayer.Value<int>("onlinecoopmax"),
                ["splitscreen"] = jsonMultiplayer.Value<bool>("splitscreen"),
                ["splitscreenonline"] = jsonMultiplayer.Value<bool>("splitscreenonline"),
            };

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(multiplayerModesJObject.ToString(), Encoding.UTF8, "application/json")
            };

        }

        /* Get all videos by gameID, returns the of youtube links max of 8, FIX THIS need to return json and httpResponse */
        public async Task<List<string>> Videos(int gameID)
        {

            bool tokenSucces = await PrepareRequestAsync();
            if (!tokenSucces)
            {
                throw new InvalidOperationException("Failed to refresh token.");
            }

            var body = $"fields video_id; where game={gameID};";
            var content = new StringContent(body, Encoding.UTF8, "text/plain");
            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/game_videos", content);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed error:{response.StatusCode} call to api.igdb.com/v4/game_videos");

            }

            var jsonStringVideos = await response.Content.ReadAsStringAsync();
            var jsonVideosLinksIds = JArray.Parse(jsonStringVideos);

            List<string> youtubeURLs = jsonVideosLinksIds
                .Where(videoJson => videoJson["video_id"] != null && !string.IsNullOrEmpty((string)videoJson["video_id"]))
                .Select(videoJson => $"https://www.youtube.com/embed/{videoJson["video_id"]}")
                .ToList();
            return youtubeURLs;
        }

    }
}