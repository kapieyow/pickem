using Marten.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Data.Models
{
    public class SystemSettingsData
    {
        [Identity]
        public int Id { get; set; }
        public int CurrentWeekRef { get; set; }
        public string NcaaSeasonCodeRef { get; set; }
        public string SeasonCodeRef { get; set; }
    }
}
