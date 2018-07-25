using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class Spread
    {
        public SpreadDirections SpreadDirection { get; internal set; }
        public decimal PointSpread { get; internal set; }
    }
}
