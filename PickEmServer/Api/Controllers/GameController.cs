using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickEmServer.Api.Models;
using PickEmServer.Heart;

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    public class GameController : Controller
    {
        private readonly GameService _gameService;

        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost]
        [Route("api/private/{SeasonCode}/{WeekNumber}/games")]
        public async Task<Game> AddGame(string SeasonCode, int WeekNumber, [FromBody] GameAdd newGame)
        {
            // TODO: handle exceptions
            return await _gameService.AddGame(SeasonCode, WeekNumber, newGame);
        }

        [HttpPut]
        [Route("api/private/{SeasonCode}/{WeekNumber}/games/{GameId}")]
        public async Task<Game> UpdateGame(string SeasonCode, int WeekNumber, int GameId, [FromBody] GameUpdate gameUpdates)
        {
            // TODO: handle exceptions
            return await _gameService.UpdateGame(SeasonCode, WeekNumber, GameId, gameUpdates);
        }

        [HttpPut]
        [Route("api/private/{SeasonCode}/{WeekNumber}/games/{GameId}/spread")]
        public async Task<Game> UpdateSpread(string SeasonCode, int WeekNumber, int GameId, [FromBody] SpreadUpdate spreadUpdates)
        {
            // TODO: handle exceptions
            return await _gameService.UpdateSpread(SeasonCode, WeekNumber, GameId, spreadUpdates);
        }
    }
}