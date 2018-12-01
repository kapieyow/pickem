import { GameScoreboard } from './game-scoreboard';

export class PlayerScoreboard
{
    games: number;
    gamesPicked: number;
    gamesWon: number;
    gamesLost: number;
    gamesPending: number;
    points: number;

    gamePickScoreboards: GameScoreboard[];

    constructor() { this.gamePickScoreboards = []; }
}