using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class LogAdd
    {
        public string Component { get; set; }
        public string LogMessage { get; set; }
        public string LogLevel { get; set; }
    }
}
