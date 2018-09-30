using Marten;
using PickEmServer.Api.Models;
using PickEmServer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Heart
{
    public class SystemSettingsService
    {
        private readonly IDocumentStore _documentStore;
        private const int SYSTEM_SETTINGS_ID = 0;

        public SystemSettingsService(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task<SystemSettings> ReadSystemSettings()
        {
            using (var dbSession = _documentStore.QuerySession())
            {
                var systemSettingsData = await this.GetSystemSettings(dbSession);
                return this.MapSystemSettingsData(systemSettingsData);
            }
        }

        public async Task<SystemSettings> UpdateSystemSettings(SystemSettingsUpdate systemSettingsUpdates)
        {
            if (systemSettingsUpdates == null)
            {
                throw new ArgumentNullException("systemSettingsUpdates");
            }

            using (var dbSession = _documentStore.LightweightSession())
            {
                var systemSettingsData = await this.GetSystemSettings(dbSession);

                systemSettingsData.CurrentWeekRef = systemSettingsUpdates.CurrentWeekRef;

                dbSession.Store(systemSettingsData);
                dbSession.SaveChanges();

                return this.MapSystemSettingsData(systemSettingsData);
            }
        }

        private async Task<SystemSettingsData> GetSystemSettings(IQuerySession runningDocumentSession)
        {
            var systemSettingsData = await runningDocumentSession
                .Query<SystemSettingsData>()
                .Where(ssd => ssd.Id == SYSTEM_SETTINGS_ID)
                .ToListAsync()
                .ConfigureAwait(false);

            if ( systemSettingsData.Count != 1 )
            {
                throw new Exception($"Expecting exactly 1 row for Id = ({SYSTEM_SETTINGS_ID}) but received ({systemSettingsData.Count})");
            }

            return systemSettingsData.First();
        }

        private SystemSettings MapSystemSettingsData(SystemSettingsData systemSettingsData)
        {
            return new SystemSettings
            {
                CurrentWeekRef = systemSettingsData.CurrentWeekRef,
                NcaaSeasonCodeRef = systemSettingsData.NcaaSeasonCodeRef,
                SeasonCodeRef = systemSettingsData.SeasonCodeRef
            };
        }
    }
}
