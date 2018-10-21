using PickEmServer.App;

namespace PickEmServer.Api.Models
{
    public class Spread
    {
        public SpreadDirections SpreadDirection { get; internal set; }
        public decimal PointSpread { get; internal set; }
    }
}
