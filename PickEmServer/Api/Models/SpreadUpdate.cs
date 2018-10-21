using PickEmServer.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class SpreadUpdate
    {
        public SpreadDirections SpreadDirection { get; set; }
        public decimal PointSpread { get; set; }
    }
}
