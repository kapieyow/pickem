import { GameStates, PickTypes, PickStates, SpreadDirections } from './enums';

export class PlayerScoreboardPick
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
    pick: PickTypes;
    pickState: PickStates
    pickToSpreadNeutral: number;
    spread: number;
    spreadDirection: SpreadDirections;
}