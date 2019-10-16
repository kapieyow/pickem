using ExampleCSharpBot.PickemModels;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ExampleCSharpBot
{
    class Program
    {
        const string PickemUserName = "SET_YOUR_USER_NAME";
        const string PickemPassword = "SET_YOUR_PASSWORD";
        const string PickemBotLeagueCode = "SET_LEAGUE_CODE";
        const string PickemBaseUrl = "SET_BASE_PICKEM_URL";

        static void Main(string[] args)
        {
            RunBotAsync().GetAwaiter().GetResult();
        }

        static async Task RunBotAsync()
        {
            // authenticate
            var userCredentials = new UserCredentials { UserName = PickemUserName, Password = PickemPassword };
            var userLoggedIn = await PostToApi<UserLoggedIn>($"{PickemBaseUrl}/auth/login", userCredentials);
            var jwt = userLoggedIn.Token;

            // get player tag for this user in the league
            var player = await GetFromApi<Player>($"{PickemBaseUrl}/{PickemBotLeagueCode}/players/{PickemUserName}", jwt);

            // get pickem games for current week
            var botLeague = userLoggedIn.Leagues.Single(l => l.LeagueCode == PickemBotLeagueCode);
            var playerScoreboard = await GetFromApi<PlayerScoreboard>(
                $"{PickemBaseUrl}/{PickemBotLeagueCode}/{botLeague.CurrentWeekRef}/{player.PlayerTag}/scoreboard", 
                jwt);

            foreach ( var gamePickScoreboard in playerScoreboard.GamePickScoreboards )
            {
                // THIS IS WHERE YOUR MAGIC PICK LOGIC GOES. 
                var pick = PickTypes.Home;

                // pick your pick
                var pickUpdate = new PlayerPickUpdate { Pick = pick };
                var playerPickResult = await PutToApi<PlayerPick>(
                    $"{PickemBaseUrl}/{PickemBotLeagueCode}/{botLeague.CurrentWeekRef}/{player.PlayerTag}/scoreboard/{gamePickScoreboard.GameId}/pick", 
                    pickUpdate,
                    jwt);
            }

        }


        static async Task<T> GetFromApi<T>(string Url, string Jwt)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + Jwt);

            var responseString = await httpClient.GetStringAsync(Url);

            return JsonConvert.DeserializeObject<T>(responseString);
        }

        static async Task<T> PostToApi<T>(string Url, object postObject)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var httpResponse = await httpClient.PostAsync(Url, new StringContent(JsonConvert.SerializeObject(postObject), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            var responseString = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseString);
        }

        static async Task<T> PutToApi<T>(string Url, object postObject, string Jwt)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("authorization", "Bearer " + Jwt);

            var httpResponse = await httpClient.PutAsync(Url, new StringContent(JsonConvert.SerializeObject(postObject), Encoding.UTF8, "application/json"));
            httpResponse.EnsureSuccessStatusCode();
            var responseString = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(responseString);
        }
    }
}
