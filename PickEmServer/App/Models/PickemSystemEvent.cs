using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.App.Models
{
    public class PickemSystemEvent
    {

        public PickemSystemEvent(PickemSystemEventTypes type)
        {
            this.Type = type;
            this.LeagueCodesAffected = new List<string>();
            this.DynamicKeys = new ExpandoObject();
        }

        public PickemSystemEvent(PickemSystemEventTypes type, string seasonCode, int weekNumber) : this(type)
        {
            this.DynamicKeys.seasonCode = seasonCode;
            this.DynamicKeys.weekNumber = weekNumber;
        }

        public PickemSystemEventTypes Type { get; internal set; }
        public List<string> LeagueCodesAffected { get; internal set; }
        public dynamic DynamicKeys { get; internal set; }

    }
}
