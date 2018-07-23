using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class User
    {
        public string Email { get; internal set; }
        public string UserName { get; internal set; }
    }
}
