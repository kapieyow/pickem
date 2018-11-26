#!/usr/bin/env python3
 
import argparse
import configparser
import datetime
import json
import requests
import time
import pickemLogger
import pickemApiClient
import pickemSynchGames
import pickemUpdateSpreads
import pickemUpdateTeams

# "configs"
VERSION = "2.0.24"
PICKEM_INI = "pickem-settings.ini"

# globals
class Settings:
    # from INI
    MinLogLevelToApi = ""
    PickemAdminUsername = ""
    PickemAdminPassword = ""
    PickemServerBaseUrl = ""

settings = Settings()
apiClient = None


#=====================================
# sub command methods
#=====================================
def setLeagueGames(args):
    gamesSet = 0

    for leagueCode in args.league_codes:
        for gameId in args.gameids:
            apiClient.setLeagueGame(leagueCode, args.week, gameId)
            gamesSet = gamesSet + 1

        logger.info("Set (" + str(gamesSet) + ") games for league (" + leagueCode + ") in week (" + str(args.week) + ")")

def setupLeagueWeek(args):
    for leagueCode in args.league_codes:
        apiClient.updateLeague(leagueCode, args.week)
        logger.info("Set week to (" + str(args.week) + ") for league (" + leagueCode + ")")

def synchGames(args):
    synchGamesHandler = pickemSynchGames.PickemSynchGamesHandler(apiClient, logger)

    if ( args.loop_every_sec == None ):
        synchGamesHandler.Run(args.action, args.ncaa_season_code, args.pickem_season_code, args.week)
    else:
        runCount = 0
        failCount = 0
        while(True): # for eva .. eva?
            try:
                synchGamesHandler.Run(args.action, args.ncaa_season_code, args.pickem_season_code, args.week)
            except:
                failCount = failCount + 1
           
            runCount = runCount + 1

            logger.debug("-- Run #" + str(runCount) + " complete. Fail count - " + str(failCount) + ". Snoozing " + str(args.loop_every_sec) + " seconds")
            time.sleep(args.loop_every_sec)

def updateSpreads(args):
    synchGamesHandler = pickemUpdateSpreads.PickemUpdateSpreadsHandler(apiClient, logger)
    synchGamesHandler.Run(args.action, args.pickem_season_code, args.week)

def updateTeams(args):
    updateTeamsHandler = pickemUpdateTeams.PickemUpdateSpreadsHandler(apiClient, logger)
    updateTeamsHandler.Run(args.pickem_season_code, args.week, args.rankings_source)


#=====================================
# Helper functions
#=====================================
def loadIniConfig(iniFile, settingsContainer):
    configParser = configparser.ConfigParser()
    configParser.read(iniFile)
    settingsContainer.MinLogLevelToApi = configParser.get("LOGGING", "MIN_LOG_LEVEL_TO_API")
    settingsContainer.PickemAdminUsername = configParser.get("ADMIN", "PICKEM_ADMIN_USERNAME")
    settingsContainer.PickemAdminPassword = configParser.get("ADMIN", "PICKEM_ADMIN_PASSWORD")
    settingsContainer.PickemServerBaseUrl = configParser.get("URLS", "PICKEM_SERVER_BASE_URL")

def setupArgumentParsers():
    # arg setup
    argParser = argparse.ArgumentParser()
    subArgParsers = argParser.add_subparsers()

    # -- set_league_games sub-command
    subParser = subArgParsers.add_parser('set_league_games')
    subParser.add_argument('-w', '--week', type=int, required=True, help='Week number')
    subParser.add_argument('-lcs', '--league_codes', required=True, nargs='+', type=str, help='League codes')
    subParser.add_argument('-gids', '--gameids', required=True, nargs='+', type=int, help='Game Ids')
    subParser.set_defaults(func=setLeagueGames)

    # -- setup_week sub-command
    subParser = subArgParsers.add_parser('set_league_week')
    subParser.add_argument('-lcs', '--league_codes', required=True, nargs='+', type=str, help='League codes')
    subParser.add_argument('-w', '--week', type=int, required=True, help='Week number')
    subParser.set_defaults(func=setupLeagueWeek)

    # -- synch_games sub-command
    subParser = subArgParsers.add_parser('synch_games')
    subParser.add_argument('-psc', '--pickem_season_code', type=str, required=True, help='Pickem Season Code')
    subParser.add_argument('-nsc', '--ncaa_season_code', type=str, required=True, help='NCAA Season Code')
    subParser.add_argument('-w', '--week', type=int, required=True, help='Week number')
    subParser.add_argument('-a', '--action', required=True, choices=['update', 'u', 'insert', 'i'])
    subParser.add_argument('-les', '--loop_every_sec', type=int, required=False, help='Seconds to pause between loops')
    subParser.set_defaults(func=synchGames)

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

loadIniConfig(PICKEM_INI, settings)
argParser = setupArgumentParsers()

# parse and run the subcommand
args = argParser.parse_args()
if ( hasattr(args, 'func') ):

    logger = pickemLogger.Logger(settings.PickemServerBaseUrl, settings.MinLogLevelToApi, args.func.__name__)
    apiClient = pickemApiClient.PickemApiClient(settings.PickemServerBaseUrl, logger)

    # login
    apiClient.authenticate(settings.PickemAdminUsername, settings.PickemAdminPassword)

    # this runs the function in parser's set_defaults()
    args.func(args)

else:
    print("Missing sub command name")
    
