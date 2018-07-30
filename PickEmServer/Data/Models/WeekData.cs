using Marten.Schema;

namespace PickEmServer.Data.Models
{
    public class WeekData
    {
        [Identity]
        public int WeekNumber { get; set; }
    }
}
