using Marten;
using PickEmServer.Api.Models;
using PickEmServer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Heart
{
    public class LogService
    {
        private readonly IDocumentStore _documentStore;

        public LogService(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task<Log> AddLog (LogAdd logAdd)
        {

            if ( logAdd == null )
            {
                throw new ArgumentException("No LogAdd parameter input for AddLog (is null)");
            }
            // TODO : better mapping e.g. auto mapper or something 
            LogData logData = new LogData
            {
                Component = logAdd.Component,
                LogLevel = logAdd.LogLevel,
                LogMessage = logAdd.LogMessage
            };

            using (var dbSession = _documentStore.LightweightSession())
            {
                dbSession.Store(logData);
                await dbSession.SaveChangesAsync();

                // success return as API "read" object
                // TODO: better mapping e.g.auto mapper or something
                Log newLog = new Log
                {
                    Id = logData.Id,
                    Component = logData.Component,
                    LogLevel = logData.LogLevel,
                    LogMessage = logData.LogMessage
                };

                return newLog;
            }

        }

        public async Task<List<Log>> ReadLogs()
        {
            using (var dbSession = _documentStore.LightweightSession())
            {

                // verify the game codes exist
                var logs = await dbSession
                    .Query<LogData>()
                    .ToListAsync()
                    .ConfigureAwait(false);

                return logs.Select(logData =>
                    new Log
                    {
                        Component = logData.Component,
                        Id = logData.Id,
                        LogLevel = logData.LogLevel,
                        LogMessage = logData.LogMessage
                    }
                ).ToList();
            }
        }
    }
}
