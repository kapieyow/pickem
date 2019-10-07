
using Marten;
using Marten.Services;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Linq;
using System.Text;

namespace PickEmServer.Data
{
    public class MartenLogger: IMartenLogger, IMartenSessionLogger
    {
        private readonly ILogger<MartenLogger> _logger;

        public MartenLogger(ILogger<MartenLogger> logger)
        {
            _logger = logger;
        }

        public IMartenSessionLogger StartSession(IQuerySession session)
        {
            return this;
        }

        public void SchemaChange(string sql)
        {
            _logger.LogDebug($"Marten Schema Change: SQL ({sql})");
        }

        public void LogSuccess(NpgsqlCommand command)
        {
            StringBuilder logOutput = new StringBuilder();

            logOutput.Append($"Marten LogSuccess: CommandText ({command.CommandText})");

            foreach (var p in command.Parameters.OfType<NpgsqlParameter>())
            {
                logOutput.Append($" Param> ({p.ParameterName}):({p.Value})");
            }

            _logger.LogDebug(logOutput.ToString());
        }

        public void LogFailure(NpgsqlCommand command, Exception ex)
        {
            StringBuilder logOutput = new StringBuilder();

            logOutput.Append($"Marten LogFAILURE: CommandText ({command.CommandText})");

            foreach (var p in command.Parameters.OfType<NpgsqlParameter>())
            {
                logOutput.Append($" Param> ({p.ParameterName}):({p.Value})");
            }

            _logger.LogDebug(logOutput.ToString());
        }

        public void RecordSavedChanges(IDocumentSession session, IChangeSet commit)
        {
            _logger.LogDebug($"Marten RecordSavedChanges: ({commit.Updated.Count()}) updates, ({commit.Inserted.Count()}) inserts, and ({commit.Deleted.Count()}) deletions)");
        }
    }
}
