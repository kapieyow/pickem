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
    gameCurrentPeriod: string;
    gameStart: Date;
    gameStatusDescription: string;
    gameState: GameStates;
    gameTimeClock: string;
    gameTitle: string;
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
    winPoints: number;

    pickScoreboards: PickScoreboard[];
}