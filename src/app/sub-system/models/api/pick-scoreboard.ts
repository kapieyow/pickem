import { PickTypes, PickStates } from './enums';

export class PickScoreboard
{
    playerTag: string;
    pick: PickTypes;
    pickState: PickStates;
    pickedTeamIconFileName: string;
    pickedTeamLongName: string;
}