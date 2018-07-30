using Marten.Schema;

namespace PickEmServer.Data.Models
{
    public class PlayerData
    {
        [Identity]
        public string PlayerTag { get; set; }
        public string UserNameRef { get; set; }
    }
}
