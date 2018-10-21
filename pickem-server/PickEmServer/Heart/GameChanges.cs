using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Heart
{
    internal class GameChanges
    {
        internal bool AncillaryMetaDataChanged { get; set; } // stuff that does not effect scoring or pick statuses e.g. time clock and current period.
        internal bool GameStateChanged { get; set; }
        internal bool ScoreChanged { get; set; }

        internal bool GameChanged
        {
            get
            {
                return this.AncillaryMetaDataChanged || this.GameStateChanged || this.ScoreChanged;
            }
        }
    }
}
