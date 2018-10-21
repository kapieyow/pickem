using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class LeagueAdd
    {
        public string LeagueCode { get; set; }
        public string LeagueTitle { get; set; }

        public List<int> WeekNumbers { get; set; }
    }
}
