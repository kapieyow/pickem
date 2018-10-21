using Microsoft.Extensions.Logging;
using PickEmServer.Heart;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.App
{
    public class PickemDatabaseLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, PickemDatabaseLogger> _loggers = new ConcurrentDictionary<string, PickemDatabaseLogger>();
        private readonly LogService _logService;

        public PickemDatabaseLoggerProvider(LogService logService)
        {
            _logService = logService;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new PickemDatabaseLogger(name, _logService));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
