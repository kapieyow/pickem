using Marten;
using PickEmServer.Api.Models;
using PickEmServer.App;
using PickEmServer.Data.Models;
using System;
using System.Collections.Generic;

namespace PickEmServer.Heart
{
    public class GameChanger
    {
        private GameData _game;
        private IDocumentSession _dbSession;

        public GameChanger(GameData game, IDocumentSession dbSession)
        {
            _game = game;
            _dbSession = dbSession;
        }

        public void ApplyChanges(GameUpdate gameUpdates)
        {
            _game.LastUpdated = gameUpdates.LastUpdated;
            _game.CurrentPeriod = gameUpdates.CurrentPeriod;
            _game.TimeClock = gameUpdates.TimeClock;
            _game.GameStart = gameUpdates.GameStart;
            this.UpdateAwayTeamScore(gameUpdates.AwayTeamScore);
            this.UpdateHomeTeamScore(gameUpdates.HomeTeamScore);

            this.UpdateGameState(gameUpdates.GameState);

            _dbSession.Store(_game);
        }

        public void ApplySpread(SpreadUpdate spreadUpdates)
        {
            if ( _game.GameState != GameStates.SpreadNotSet )
            {
                throw new InvalidOperationException($"Cannot update game id: {_game.GameId} spread because the game state is: {_game.GameState}");
            }

            _game.Spread.PointSpread = spreadUpdates.PointSpread;
            _game.Spread.SpreadDirection = spreadUpdates.SpreadDirection;

            this.SynchScoresAfterSpread();

            _dbSession.Store(_game);
        }

        public void SaveChanges()
        {
            _dbSession.SaveChanges();
        }

        private void UpdateGameState(GameStates newGameState)
        {
            if ( _game.GameState != newGameState )
            {
                _game.GameState = newGameState;

                switch (newGameState)
                {
                    case GameStates.Cancelled:
                        // TODO: more here to cancel? no scores?
                        break;

                    case GameStates.Final:
                        //  - game: set winner
                        _game.AwayTeam.Winner = (_game.AwayTeam.Score > _game.HomeTeam.Score);
                        _game.HomeTeam.Winner = (_game.AwayTeam.Score < _game.HomeTeam.Score);

                        // TODO: //  - player picks: update pick status(es) for related games
                        // TODO: //  - player picks: update week points
                        // TODO: //  - week scoreboard: update this weeks player points (in leagues with games)
                        // TODO: //  - league board: update this weeks player points (in leagues with games)
                        // TODO: //  - league board: update this seasons player points (in leagues with games)

                        break;

                    case GameStates.InGame:
                        // TODO: //  - picks: lock picks
                        break;

                    case GameStates.SpreadNotSet:
                        break;

                    case GameStates.SpreadSet:
                        // TODO: // open picks
                        break;
                    default:
                        throw new ArgumentException($"Unknown new game state: {newGameState}");
                }

            }
        }

        private void UpdateAwayTeamScore(int newScore)
        {
            if ( _game.AwayTeam.Score != newScore )
            {
                //  - game: update score
                _game.AwayTeam.Score = newScore;

                //  - game: update score after spread
                this.SynchScoresAfterSpread();

                // TODO: //  - player picks: update pick status(es) for related games
            }
        }

        private void UpdateHomeTeamScore(int newScore)
        {
            if (_game.HomeTeam.Score != newScore)
            {
                //  - game: update score
                _game.HomeTeam.Score = newScore;

                //  - game: update score after spread
                this.SynchScoresAfterSpread();

                // TODO: //  - player picks: update pick status(es) for related games
            }
        }

        private void SynchScoresAfterSpread()
        {
            //  - game: update score after spread
            switch (_game.Spread.SpreadDirection)
            {
                case SpreadDirections.None:
                    _game.AwayTeam.ScoreAfterSpread = _game.AwayTeam.Score;
                    _game.HomeTeam.ScoreAfterSpread = _game.HomeTeam.Score;
                    break;

                case SpreadDirections.ToAway:
                    _game.AwayTeam.ScoreAfterSpread = _game.AwayTeam.Score + _game.Spread.PointSpread;
                    _game.HomeTeam.ScoreAfterSpread = _game.HomeTeam.Score;
                    break;

                case SpreadDirections.ToHome:
                    _game.AwayTeam.ScoreAfterSpread = _game.AwayTeam.Score;
                    _game.HomeTeam.ScoreAfterSpread = _game.HomeTeam.Score + _game.Spread.PointSpread;
                    break;
            }
        }
    }
}
