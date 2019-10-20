using ExampleCSharpBot.PickemModels;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ExampleCSharpBot
{
    class Program
    {
        const string PickemUserName = "gumanchew";
        const string PickemPassword = "password1";
        const string PickemBotLeagueCode = "BOTS-NCAAF-19";
        const string PickemBaseUrl = "https://streamhead.duckdns.org/p-api/api";

        static async Task Main(string[] args)
        {
            await RunBotAsync();
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

            var myMagic = new AdamsMagicService();

            var wins = 0;
            var losses = 0;

            foreach ( var gamePickScoreboard in playerScoreboard.GamePickScoreboards/*.Where(s => AdamsMagicService.BeforeGameStates.Contains(s.GameState)) */)
            {
                var pick = myMagic.ScoreIt(gamePickScoreboard);

                // pick your pick
                var pickUpdate = new PlayerPickUpdate { Pick = pick };

                Console.WriteLine($"{pick.ToString()} {gamePickScoreboard.LeaderAfterSpread} {gamePickScoreboard.LeaderAfterSpread.ToString() == pick.ToString()} {gamePickScoreboard.HomeTeamLongName} {gamePickScoreboard.HomeTeamWins} {gamePickScoreboard.HomeTeamLosses} {gamePickScoreboard.HomeTeamRank} {gamePickScoreboard.AwayTeamLongName} {gamePickScoreboard.AwayTeamWins} {gamePickScoreboard.AwayTeamLosses} {gamePickScoreboard.AwayTeamRank} {gamePickScoreboard.LeaderAfterSpread} {gamePickScoreboard.Spread} {gamePickScoreboard.SpreadDirection}");

                if (gamePickScoreboard.LeaderAfterSpread.ToString() == pick.ToString())
                    wins++;
                else
                    losses++;
                    
                try {
                    var playerPickResult = await PutToApi<PlayerPick>(
                        $"{PickemBaseUrl}/{PickemBotLeagueCode}/{botLeague.CurrentWeekRef}/{player.PlayerTag}/scoreboard/{gamePickScoreboard.GameId}/pick", 
                        pickUpdate,
                        jwt);
                } catch {}
                
            }

            Console.WriteLine($"Wins {wins} Losses {losses}");
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
