using System;
using Newtonsoft.Json.Linq;

namespace MyApp.Models
{
    public class TwitchTokenManager
    {
        private readonly HttpClient _httpClient;
        public  string TokenValue { get; private set; } 
        public  DateTime? TokenExpiration { get; private set;} = null;

        public TwitchTokenManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> RefreshToken()
        {
            //INITIAL DATA FOR ATTRIBUTE
            string clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            string clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");

            var tokenResponse = await _httpClient.PostAsync($"https://id.twitch.tv/oauth2/token?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials", null);
            if (!tokenResponse.IsSuccessStatusCode)
                {
                    return false;
                }
            var tokenResult = JObject.Parse(await tokenResponse.Content.ReadAsStringAsync());
            TokenValue = tokenResult["access_token"].ToString();
            TokenExpiration = DateTime.Now.AddSeconds(tokenResult["expires_in"].ToObject<int>());
            Console.WriteLine($"My Expiration is: {TokenExpiration}");
            return true;
        }
    }

}