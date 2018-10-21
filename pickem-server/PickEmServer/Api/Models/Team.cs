using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class Team
    {
        public string TeamCode { get; internal set; }
        public string ShortName { get; internal set; }
        public string LongName { get; internal set; }
        public string NcaaNameSeo { get; internal set; }
        public string theSpreadName { get; internal set; }
        public string icon24FileName { get; internal set; }
    }
}
