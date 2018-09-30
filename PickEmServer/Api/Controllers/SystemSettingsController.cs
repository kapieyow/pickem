using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickEmServer.Api.Models;
using PickEmServer.Heart;

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    public class SystemSettingsController : Controller
    {
        private readonly SystemSettingsService _systemSettingsService;

        public SystemSettingsController(SystemSettingsService systemSettingsService)
        {
            _systemSettingsService = systemSettingsService;
        }

        [Authorize]
        [HttpGet]
        [Route("api/settings")]
        public async Task<SystemSettings> ReadSystemSettings()
        {
            return await _systemSettingsService.ReadSystemSettings();
        }
    
        [Authorize]
        [HttpPut]
        [Route("api/settings")]
        public async Task<SystemSettings> UpdateSystemSettings([FromBody] SystemSettingsUpdate systemSettingsUpdate)
        {
            return await _systemSettingsService.UpdateSystemSettings(systemSettingsUpdate);
        }
     
    }
}