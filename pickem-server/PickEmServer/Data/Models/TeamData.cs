using Marten.Schema;
using System.Collections.Generic;

namespace PickEmServer.Data.Models
{
    public class TeamData
    {
        [Identity]
        public string TeamCode { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string EspnAbbreviation { get; set; }
        public string EspnDisplayName { get; set; }
        public string NcaaNameSeo { get; set; }
        public string theSpreadName { get; set; }
        public string icon24FileName { get; set; }
        public string YahooCode { get; set; }

        public List<TeamSeasonStats> Seasons { get; set; }
    }
}
