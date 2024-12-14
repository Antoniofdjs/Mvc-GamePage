using System.Collections;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using MyApp.Data;
using MyApp.Models;
using Newtonsoft.Json.Linq;
namespace MyApp.Services.IGDB
{

    // All igdb api request endpoints will be made here
    public class Igdb
    {
        private readonly HttpClient _httpClient;
        private readonly TwitchTokenManager _twitchTokenManager;
        private readonly LocalData _localData;

        public Igdb(HttpClient httpClient, TwitchTokenManager twitchTokenManager, LocalData localData)
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

        // Fecth all game details and return the IgdbDetails Model
        public async Task<IgdbDetails> GameDetails(string slugTitle)
        {
            bool tokenSucces = await PrepareRequestAsync();
            if (!tokenSucces)
            {
                throw new InvalidOperationException("Failed to refresh token.");
            }

            var body = $"fields id, name, screenshots, videos, themes, genres, category, summary, storyline, slug, dlcs, rating; where slug=\"{slugTitle}\";";
            var content = new StringContent(body, Encoding.UTF8, "text/plain");

            // Send the POST
            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/games", content);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Error{response.StatusCode}with api.igdb.com/v4/games.");
            }

            var responseIgdbString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseIgdbString);
            var gamesJson = JArray.Parse(responseIgdbString);
            var gameJson = gamesJson[0];

            IgdbDetails igdbDetails = new IgdbDetails();

            igdbDetails.SlugTitle = (string)gameJson["slug"];
            igdbDetails.StoryLine = (string)gameJson["storyline"];
            igdbDetails.Summary = (string)gameJson["summary"];
            igdbDetails.RatingCount = (int)gameJson["rating"];
            igdbDetails.RatingLink = $"https://www.igdb.com/games/{igdbDetails.SlugTitle}#community";
            igdbDetails.GameName = (string)gameJson["name"];
            igdbDetails.GameID = (string)gameJson["id"];
            foreach (var themeID in gameJson["themes"].ToObject<List<int>>())
            {
                Console.WriteLine($"Theme ID: {themeID}");
                igdbDetails.ThemesGenres.Add(_localData.Themes.Where(u => u.ID == themeID).Select(u => u.Name).FirstOrDefault());
            }
            foreach (var genreID in gameJson["genres"].ToObject<List<int>>())
            {
                Console.WriteLine($"Genre ID: {genreID}");
                igdbDetails.ThemesGenres.Add(_localData.Genres.Where(u => u.ID == genreID).Select(u => u.Name).FirstOrDefault());
            }

            try
            {
                igdbDetails.VideosLinks = await Videos(igdbDetails.GameID);

            }
            catch (HttpRequestException ex) // http request failed
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw new InvalidOperationException("An error occurred while processing the request.");

            }
            catch (InvalidOperationException ex) // refresh token failed
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw new InvalidOperationException("An error occurred while processing refresh token");

            }

            return igdbDetails;
        }

        /* Get all videos by gameID, returns the IEnumberable(like a list) of youtube links, max of 8 */
        public async Task<List<string>> Videos(string gameID)
        {


            bool tokenSucces = await PrepareRequestAsync();
            if (!tokenSucces)
            {
                throw new InvalidOperationException("Failed to refresh token.");
            }

            var body = $"fields video_id; where game={gameID}; limit 8;";
            var content = new StringContent(body, Encoding.UTF8, "text/plain");
            var response = await _httpClient.PostAsync("https://api.igdb.com/v4/game_videos", content);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed error:{response.StatusCode} to make API call api.igdb.com/v4/game_videos");

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