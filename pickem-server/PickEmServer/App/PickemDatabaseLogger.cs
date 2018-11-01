using Microsoft.Extensions.Logging;
using PickEmServer.Api.Models;
using PickEmServer.Heart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.App
{
    public class PickemDatabaseLogger : ILogger
    {
        private readonly LogService _logService;
        private readonly string _name;

        public PickemDatabaseLogger(string name, LogService logService)
        {
            _logService = logService;
            _name = name;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // NOTE: The filters in appsettings override this. For example if you have 
            //      "PickemDatabaseLogger": {
            //          "LogLevel": {
            //              "Default": "None"
            //          }
            //      }
            //
            // this will not be called and Log will not either
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

            LogAdd newLog = new LogAdd
            {
                // This is a internal service log. The API logs from others will not be run throuhg the Core logger
                Component = _name,
                LogLevel = logLevel.ToString(),
                LogMessage = formatter(state, exception)
            };

            _logService.AddLog(newLog);
        }
    }
}
