#!/usr/bin/env python3

#=================================================
# Pickem CLI - Handles command line input 
# parsing then hands of to core and/or handlers
# Not intended to have any logic.
#================================================= 
import argparse
import time
import pickemAddYahooGames
import pickemCore
import pickemSynchGames
import pickemUpdateSpreads
import pickemUpdateTeams

# "configs"
VERSION = "2.2.38"

#=====================================
# sub command methods
#=====================================

def extractGames(args):
    core.extractPickemGames(args.pickem_season_code, args.week)

def setLeagueGame(args):
    core.setLeagueGame(args.league_codes, args.week, args.game_id, args.pickem_team_code, args.yahoo_team_code, args.game_win_points)

def setLeagueGames(args):
    core.setLeagueGames(args.league_codes, args.week, args.game_ids)

def setLeagueGamesFromYahoo(args):
    addYahooGamesHandler = pickemAddYahooGames.PickemAddYahooGamesHandler(core)
    addYahooGamesHandler.Run(args.league_codes, args.week)

def setupLeagueWeek(args):
    core.setupLeagueWeek(args.league_codes, args.week)

def synchGames(args):
    synchGamesHandler = pickemSynchGames.PickemSynchGamesHandler(core.apiClient, core.logger)

    if ( args.loop_every_sec == None ):
        synchGamesHandler.Run(args.action, args.espn_season_code, args.pickem_season_code, args.week, args.dump_json)
    else:
        runCount = 0
        failCount = 0
        while(True): # for eva .. eva?
            try:
                synchGamesHandler.Run(args.action, args.espn_season_code, args.pickem_season_code, args.week, args.dump_json)
            except:
                failCount = failCount + 1
           
            runCount = runCount + 1

            core.logger.info("-- Run #" + str(runCount) + " complete. Fail count - " + str(failCount) + ". Snoozing " + str(args.loop_every_sec) + " seconds")
            time.sleep(args.loop_every_sec)

def updateGame(args):
    # read current data. Then update ONLY the title and re-send the rest of the current state
    # TODO: better way to do this?
    gameData = core.apiClient.readGame(args.game_id)
    core.apiClient.updateGame(args.game_id, gameData['gameStart'], gameData['lastUpdated'], gameData['gameState'], gameData['currentPeriod'], gameData['timeClock'], gameData['awayTeam']['score'], gameData['homeTeam']['score'], args.game_title)

def updateSpreads(args):
    synchGamesHandler = pickemUpdateSpreads.PickemUpdateSpreadsHandler(core.apiClient, core.logger)
    synchGamesHandler.Run(args.action, args.pickem_season_code, args.week)

def updateTeams(args):
    updateTeamsHandler = pickemUpdateTeams.PickemUpdateSpreadsHandler(core.apiClient, core.logger)
    updateTeamsHandler.Run(args.pickem_season_code, args.week, args.rankings_source)


#=====================================
# Helper functions
#=====================================
def setupArgumentParsers():
    # arg setup
    argParser = argparse.ArgumentParser()
    subArgParsers = argParser.add_subparsers()

   

    # -- extract_games sub-command
    subParser = subArgParsers.add_parser('extract_games')
    subParser.add_argument('-psc', '--pickem_season_code', type=str, required=True, help='Pickem Season Code')
    subParser.add_argument('-w', '--week', type=int, required=True, help='Week number')
    subParser.set_defaults(func=extractGames)

    # -- set_league_game sub-command (SINGLE Game)
    subParser = subArgParsers.add_parser('set_league_game')
    subParser.add_argument('-w', '--week', type=int, required=True, help='Week number')
    subParser.add_argument('-lcs', '--league_codes', required=True, nargs='+', type=str, help='League codes')

    mutexParams = subParser.add_mutually_exclusive_group(required=True)
    mutexParams.add_argument('-gid', '--game_id', type=int, help='Game Id')
    mutexParams.add_argument('-ptc', '--pickem_team_code', type=str, help='Pickem Team Code')
    mutexParams.add_argument('-ytc', '--yahoo_team_code', type=str, help='Yahoo Team Code')

    subParser.add_argument('-gwp', '--game_win_points', required=True, type=int, help='Game Win Points')
    subParser.set_defaults(func=setLeagueGame)

    # -- set_league_games sub-command (MULTIPLE games)
    subParser = subArgParsers.add_parser('set_league_games')
    subParser.add_argument('-w', '--week', type=int, required=True, help='Week number')
    subParser.add_argument('-lcs', '--league_codes', required=True, nargs='+', type=str, help='League codes')
    subParser.add_argument('-gids', '--game_ids', required=True, nargs='+', type=int, help='Game Ids')
    subParser.set_defaults(func=setLeagueGames)

     # -- set_league_games_from_yahoo 
    subParser = subArgParsers.add_parser('set_league_games_from_yahoo')
    subParser.add_argument('-w', '--week', type=int, required=True, help='Week number')
    subParser.add_argument('-lcs', '--league_codes', required=True, nargs='+', type=str, help='League codes')
    subParser.set_defaults(func=setLeagueGamesFromYahoo)

    # -- setup_week sub-command
    subParser = subArgParsers.add_parser('set_league_week')
    subParser.add_argument('-lcs', '--league_codes', required=True, nargs='+', type=str, help='League codes')
    subParser.add_argument('-w', '--week', type=int, required=True, help='Week number')
    subParser.set_defaults(func=setupLeagueWeek)

    # -- synch_games sub-command
    subParser = subArgParsers.add_parser('synch_games')
    subParser.add_argument('-psc', '--pickem_season_code', type=str, required=True, help='Pickem Season Code (YY)')
    subParser.add_argument('-esc', '--espn_season_code', type=str, required=True, help='Espn Season Code (YYYY)')
    subParser.add_argument('-w', '--week', type=int, required=True, help='Week number')
    subParser.add_argument('-a', '--action', required=True, choices=['update', 'u', 'insert', 'i'])
    subParser.add_argument('-les', '--loop_every_sec', type=int, required=False, help='Seconds to pause between loops')
    subParser.add_argument('-dj', '--dump_json', action='store_true', help='Use to dump source data as json with timestamp')
    subParser.set_defaults(func=synchGames)

    # -- update_game sub-command
    subParser = subArgParsers.add_parser('update_game')
    subParser.add_argument('-gid', '--game_id', required=True, type=int, help='Game Id')
    subParser.add_argument('-gt', '--game_title', required=True, type=str, help='Game Title')
    subParser.set_defaults(func=updateGame)

    # -- update_spreads sub-command
    subParser = subArgParsers.add_parser('update_spreads')
    subParser.add_argument('-psc', '--pickem_season_code', type=str, required=True, help='Pickem Season Code')
    subParser.add_argument('-w', '--week', type=int, required=True, help='Week number')
    subParser.add_argument('-a', '--action', required=True, choices=['update', 'u', 'lock', 'l'])
    subParser.set_defaults(func=updateSpreads)

    # -- update_teams sub-command
    subParser = subArgParsers.add_parser('update_teams')
    subParser.add_argument('-psc', '--pickem_season_code', type=str, required=True, help='Pickem Season Code')
    subParser.add_argument('-w', '--week', type=int, required=True, help='Week number')
    subParser.add_argument('-rs', '--rankings_source', nargs='?', const='ap', default='ap', choices=['ap', 'cfp'])
    subParser.set_defaults(func=updateTeams)

    return argParser

#============================
#  Main
#============================
print("----------------------------------------")
print("  Pickem Console - {0:s} ".format(VERSION))
print("----------------------------------------")

argParser = setupArgumentParsers()

# parse and run the subcommand
args = argParser.parse_args()
if ( hasattr(args, 'func') ):

    core = pickemCore.Core(args.func.__name__)

    # this runs the function in parser's set_defaults()
    args.func(args)

else:
    print("Missing sub command name")
    
