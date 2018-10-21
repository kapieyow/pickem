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
        private readonly bool _neverLog = false;


        public PickemDatabaseLogger(string name, LogService logService)
        {
            _logService = logService;
            _name = name;

            // TODO this should be done through filters the RIGHT way
            _neverLog = !name.Contains("PickEmServer");
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // TODO: configure this to NOT log everything 
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // TODO: never log is TERRIBLE. Is used to avoid logging other stuff besides this app. Should be a filter or something
            if (_neverLog || !IsEnabled(logLevel))
            { 
                return;
            }

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
