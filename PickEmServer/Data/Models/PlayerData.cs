using Marten.Schema;

namespace PickEmServer.Data.Models
{
    public class PlayerData
    {
        [Identity]
        public string PlayerTag { get; internal set; }
        public string UserNameRef { get; internal set; }
    }
}
