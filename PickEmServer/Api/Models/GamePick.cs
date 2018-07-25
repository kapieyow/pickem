using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class GamePick
    {
        public Game Game { get; internal set; }
        public int PickId { get; internal set; }
        public PickTypes Pick { get; internal set; }
        public PickStatuses PickStatus { get; internal set; }
    }
}
