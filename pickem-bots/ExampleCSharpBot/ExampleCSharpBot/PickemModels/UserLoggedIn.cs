using System.Collections.Generic;

namespace ExampleCSharpBot.PickemModels
{
    public class UserLoggedIn
    {
        public string DefaultLeagueCode { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }

        public List<League> Leagues { get; set; }
    }
}
