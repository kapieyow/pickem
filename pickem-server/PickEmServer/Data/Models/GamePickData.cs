using PickEmServer.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Data.Models
{
    public class GamePickData
    {
        public int GameIdRef { get; set; }
        public PickTypes Pick { get; set; }
        public PickStates PickStatus { get; set; }
    }
}
