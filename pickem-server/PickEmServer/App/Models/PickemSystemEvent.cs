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
            : this(type, null, null, null, null)
        { }

        public PickemSystemEvent(PickemSystemEventTypes type, string seasonCode, int weekNumber)
            : this(type, seasonCode, null, weekNumber, null)
        { }

        public PickemSystemEvent(PickemSystemEventTypes type, string seasonCode, int weekNumber, int gameId)
            : this(type, seasonCode, null, weekNumber, gameId)
        { }

        public PickemSystemEvent(PickemSystemEventTypes type, string leagueCode)
           : this(type, null, leagueCode, null, null)
        { }

        public PickemSystemEvent(PickemSystemEventTypes type, string seasonCode, string leagueCode, int? weekNumber, int? gameId)
        {
            this.Type = type;
            this.LeagueCodesAffected = new List<string>();
            this.DynamicKeys = new ExpandoObject();
            this.DynamicInformation = new ExpandoObject();

            if ( seasonCode != null )
                this.DynamicKeys.seasonCode = seasonCode;

            if ( leagueCode != null )
                this.DynamicKeys.leagueCode = leagueCode;

            if ( weekNumber != null )
                this.DynamicKeys.weekNumber = weekNumber.Value;

            if ( gameId != null )
                this.DynamicKeys.gameId = gameId.Value;
        }

        public PickemSystemEventTypes Type { get; internal set; }
        public List<string> LeagueCodesAffected { get; internal set; }
        public dynamic DynamicKeys { get; internal set; }
        public dynamic DynamicInformation { get; internal set; }

    }
}
