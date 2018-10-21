import { League } from './league';

export class UserLoggedIn
{
    defaultLeagueCode: string;
    email: string;
    token: string;
    userName: string;
    leagues: League[];
}