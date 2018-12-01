import { PickemScoringTypes } from "./enums";

export class League
{
    currentWeekRef: number;
    leagueCode: string;
    leagueTitle: string;
    ncaaSeasonCodeRef: string;
    pickemScoringType: PickemScoringTypes;
    seasonCodeRef: string;
}