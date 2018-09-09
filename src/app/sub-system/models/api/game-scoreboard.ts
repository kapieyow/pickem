import { GameLeaderTypes, GameStates, SpreadDirections } from './enums';
import { PickScoreboard } from './pick-scoreboard';

export class GameScoreboard
{
    awayTeamIconFileName: string;
    awayTeamLosses: number;
    awayTeamLongName: string;
    awayTeamRank: number;
    awayTeamScore: number;
    awayTeamWins: number;
    gameStatusDescription: string;
    gameState: GameStates;
    homeTeamIconFileName: string;
    homeTeamLosses: number;
    homeTeamLongName: string;
    homeTeamRank: number;
    homeTeamScore: number;
    homeTeamWins: number;
    gameId: number;
    spread: number;
    spreadDirection: SpreadDirections;
    leader: GameLeaderTypes;
    leaderAfterSpread: GameLeaderTypes;

    pickScoreboards: PickScoreboard[];
}