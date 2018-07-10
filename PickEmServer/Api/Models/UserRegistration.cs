using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class UserRegistration
    {
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
