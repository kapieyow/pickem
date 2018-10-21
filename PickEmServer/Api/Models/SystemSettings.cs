using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class SystemSettings
    {
        public int CurrentWeekRef { get; internal set; }
        public string NcaaSeasonCodeRef { get; internal set; }
        public string SeasonCodeRef { get; internal set; }
    }
}
