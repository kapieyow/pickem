using Marten;
using Microsoft.Extensions.Logging;
using PickEmServer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Heart
{
    public class ReferenceService
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger<ReferenceService> _logger;

        public ReferenceService(IDocumentStore documentStore, ILogger<ReferenceService> logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        public async void ThrowIfNonexistantSeason(string seasonCode)
        {
            using (var dbSession = _documentStore.LightweightSession())
            {
                // verify season exists
                var season = await dbSession
                    .Query<SeasonData>()
                    .Where(s => s.SeasonCode == seasonCode)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (season == null)
                {
                    throw new ArgumentException($"Input season code ({seasonCode}) does not exist");
                }
            }
        }
    }
}
