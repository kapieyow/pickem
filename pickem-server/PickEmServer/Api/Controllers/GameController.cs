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

        [Authorize(Policy = "IsAGod")]
        [HttpGet]
        [Route("api/{SeasonCode}/{WeekNumber}/games_in_any_league")]
        public async Task<List<Game>> ReadGamesInAnyLeague(string SeasonCode, int WeekNumber)
        {
            return await _gameService.ReadGamesInAnyLeague(SeasonCode, WeekNumber);
        }

        [Authorize(Policy = "IsAGod")]
        [HttpGet]
        [Route("api/games/{GameId}")]
        public async Task<Game> ReadGame(int GameId)
        {
            return await _gameService.ReadGame(GameId);
        }

        [Authorize(Policy = "IsAGod")]
        [HttpGet]
        [Route("api/games/{SeasonCode}/{WeekNumber}")]
        public async Task<List<Game>> ReadGames(string SeasonCode, int WeekNumber)
        {
            return await _gameService.ReadGames(SeasonCode, WeekNumber);
        }

        [Authorize(Policy = "IsAGod")]
        [HttpPost]
        [Route("api/games/{SeasonCode}/{WeekNumber}")]
        public async Task<Game> AddGame(string SeasonCode, int WeekNumber, [FromBody] GameAdd newGame)
        {
            return await _gameService.AddGame(SeasonCode, WeekNumber, newGame);
        }

        [Authorize(Policy = "IsAGod")]
        [HttpPut]
        [Route("api/games/{GameId}")]
        public async Task<Game> UpdateGame(int GameId, [FromBody] GameUpdate gameUpdates)
        {
            return await _gameService.UpdateGame(GameId, gameUpdates);
        }

        [Authorize(Policy = "IsAGod")]
        [HttpPut]
        [Route("api/games/{GameId}/spread")]
        public async Task<Game> UpdateSpread(int GameId, [FromBody] SpreadUpdate spreadUpdates)
        {
            return await _gameService.UpdateSpread(GameId, spreadUpdates);
        }

        [Authorize(Policy = "IsAGod")]
        [HttpPut]
        [Route("api/games/{GameId}/spread/lock")]
        public async Task<Game> UpdateSpread(int GameId)
        {
            return await _gameService.LockSpread(GameId);
        }
    }
}