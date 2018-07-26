using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class JobResult
    {
        public bool Success { get; internal set; }
        public List<string> Messages { get; internal set; }
    }
}
