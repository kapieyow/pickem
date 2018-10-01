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
    public class GameController : Controller
    {
        private readonly GameService _gameService;

        public GameController(GameService gameService)
        {
            _gameService = gameService;
        }

        [Authorize]
        [HttpGet]
        [Route("api/{SeasonCode}/{WeekNumber}/games_in_any_league")]
        public async Task<List<Game>> ReadGamesInAnyLeague(string SeasonCode, int WeekNumber)
        {
            return await _gameService.ReadGamesInAnyLeague(SeasonCode, WeekNumber);
        }

        [Authorize]
        [HttpGet]
        [Route("api/{SeasonCode}/{WeekNumber}/games")]
        public async Task<List<Game>> ReadGames(string SeasonCode, int WeekNumber)
        {
            return await _gameService.ReadGames(SeasonCode, WeekNumber);
        }

        [Authorize]
        [HttpPost]
        [Route("api/{SeasonCode}/{WeekNumber}/games")]
        public async Task<Game> AddGame(string SeasonCode, int WeekNumber, [FromBody] GameAdd newGame)
        {
            return await _gameService.AddGame(SeasonCode, WeekNumber, newGame);
        }

        [Authorize]
        [HttpPut]
        [Route("api/{SeasonCode}/{WeekNumber}/games/{GameId}")]
        public async Task<Game> UpdateGame(string SeasonCode, int WeekNumber, int GameId, [FromBody] GameUpdate gameUpdates)
        {
            return await _gameService.UpdateGame(SeasonCode, WeekNumber, GameId, gameUpdates);
        }

        [Authorize]
        [HttpPut]
        [Route("api/{SeasonCode}/{WeekNumber}/games/{GameId}/spread")]
        public async Task<Game> UpdateSpread(string SeasonCode, int WeekNumber, int GameId, [FromBody] SpreadUpdate spreadUpdates)
        {
            return await _gameService.UpdateSpread(SeasonCode, WeekNumber, GameId, spreadUpdates);
        }

        [Authorize]
        [HttpPut]
        [Route("api/{SeasonCode}/{WeekNumber}/games/{GameId}/spread/lock")]
        public async Task<Game> UpdateSpread(string SeasonCode, int WeekNumber, int GameId)
        {
            return await _gameService.LockSpread(SeasonCode, WeekNumber, GameId);
        }
    }
}