using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Data.Models
{
    public class LogData
    {
        public int Id { get; set; }
        public string Component { get; set; }
        public string LogMessage { get; set; }
        public string LogLevel { get; set; }
    }
}
