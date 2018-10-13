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
VERSION = "1.6.20"
PICKEM_INI = "pickem-settings.ini"

# globals
class Settings:
    # from INI
    PickemAdminUsername = ""
    PickemAdminPassword = ""
    PickemServerBaseUrl = ""
    # from server
    PickemWeekNumber = 0
    PickemSeasonCode = ""
    PickemNcaaSeasonCode = ""

settings = Settings()
apiClient = None


#=====================================
# sub command methods
#=====================================
def setLeagueGames(args):
    gamesSet = 0

    for gameId in args.gameids:
        apiClient.setLeagueGame(args.league, settings.PickemWeekNumber, gameId)
        gamesSet = gamesSet + 1

    logger.debug("Set (" + str(gamesSet) + ") games for league (" + args.league + ") in week (" + str(settings.PickemWeekNumber) + ")")

def setupWeek(args):
    apiClient.updateSettingWeek(args.week)
    logger.debug("Week set to: " + str(args.week))

def synchGames(args):
    synchGamesHandler = pickemSynchGames.PickemSynchGamesHandler(apiClient, logger)

    if ( args.loop_every_sec == None ):
        synchGamesHandler.Run(args.action, settings.PickemNcaaSeasonCode, settings.PickemSeasonCode, settings.PickemWeekNumber)
    else:
        runCount = 0
        failCount = 0
        while(True): # for eva .. eva?
            try:
                synchGamesHandler.Run(args.action, settings.PickemNcaaSeasonCode, settings.PickemSeasonCode, settings.PickemWeekNumber)
            except:
                failCount = failCount + 1
           
            runCount = runCount + 1

            logger.debug("-- Run #" + str(runCount) + " complete. Fail count - " + str(failCount) + ". Snoozing " + str(args.loop_every_sec) + " seconds")
            time.sleep(args.loop_every_sec)

def updateSpreads(args):
    synchGamesHandler = pickemUpdateSpreads.PickemUpdateSpreadsHandler(apiClient, logger)
    synchGamesHandler.Run(args.action, settings.PickemSeasonCode, settings.PickemWeekNumber)

def updateTeams(args):
    updateTeamsHandler = pickemUpdateTeams.PickemUpdateSpreadsHandler(apiClient, logger)
    updateTeamsHandler.Run(settings.PickemSeasonCode, settings.PickemWeekNumber)


#=====================================
# Helper functions
#=====================================
def loadIniConfig(iniFile, settingsContainer):
    configParser = configparser.ConfigParser()
    configParser.read(iniFile)
    settingsContainer.PickemAdminUsername = configParser.get("ADMIN", "PICKEM_ADMIN_USERNAME")
    settingsContainer.PickemAdminPassword = configParser.get("ADMIN", "PICKEM_ADMIN_PASSWORD")
    settingsContainer.PickemServerBaseUrl = configParser.get("URLS", "PICKEM_SERVER_BASE_URL")

def setServerSettings(pickemApiClient, settingsContainer):
    # TODO current week not linked to league?
    systemSettings = pickemApiClient.readSystemSettings()
    settingsContainer.PickemWeekNumber = systemSettings['currentWeekRef']
    settingsContainer.PickemSeasonCode = systemSettings['seasonCodeRef']
    settingsContainer.PickemNcaaSeasonCode = systemSettings['ncaaSeasonCodeRef']

def setupArgumentParsers():
    # arg setup
    argParser = argparse.ArgumentParser()
    subArgParsers = argParser.add_subparsers()

    # -- set_league_games sub-command
    subParser = subArgParsers.add_parser('set_league_games')
    subParser.add_argument('-l', '--league', required=True, help='League code')
    subParser.add_argument('-gids', '--gameids', required=True, nargs='+', type=int)
    subParser.set_defaults(func=setLeagueGames)

    # -- setup_week sub-command
    subParser = subArgParsers.add_parser('setup_week')
    subParser.add_argument('-w', '--week', type=int, required=True, help='Week in # e.g. 7')
    subParser.set_defaults(func=setupWeek)

    # -- synch_games sub-command
    subParser = subArgParsers.add_parser('synch_games')
    subParser.add_argument('-a', '--action', required=True, choices=['update', 'u', 'insert', 'i'])
    subParser.add_argument('-les', '--loop_every_sec', type=int, required=False)
    subParser.set_defaults(func=synchGames)

    # -- update_spreads sub-command
    subParser = subArgParsers.add_parser('update_spreads')
    subParser.add_argument('-a', '--action', required=True, choices=['update', 'u', 'lock', 'l'])
    subParser.set_defaults(func=updateSpreads)

    # -- update_teams sub-command
    subParser = subArgParsers.add_parser('update_teams')
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

    logger = pickemLogger.Logger(settings.PickemServerBaseUrl, args.func.__name__)
    apiClient = pickemApiClient.PickemApiClient(settings.PickemServerBaseUrl, logger)

    # login
    apiClient.authenticate(settings.PickemAdminUsername, settings.PickemAdminPassword)
    setServerSettings(apiClient, settings)

    # this runs the function in parser's set_defaults()
    args.func(args)

else:
    print("Missing sub command name")
    
