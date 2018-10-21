import { PlayerScoreboard } from './player-scoreboard';
import { GameScoreboard } from './game-scoreboard';
import { PlayerWeekWins } from './player-week-wins';

export class WeekScoreboard
{
    playerTags: string[];
    gamePickScoreboards: GameScoreboard[];
    playerWins: PlayerWeekWins[];
}