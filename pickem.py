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

# "configs"
VERSION = "1.6.12"
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


#=====================================
# sub command methods
#=====================================
def setupWeek(args):
    logger.debug("setup week, w = " + str(args.week) + " l = " + args.league)

def synchGames(args):
    logger.debug("synch games")

    synchGamesHandler = pickemSynchGames.PickemSynchGamesHandler(apiClient, logger)
    synchGamesHandler.Run(args.action, args.source, settings.PickemNcaaSeasonCode, settings.PickemSeasonCode, settings.PickemWeekNumber)

def updateSpreads(args):
    logger.debug("update spreads")

def updateTeams(args):
    logger.debug("update teams")


#=====================================
# Helper functions
#=====================================
def loadIniConfig(iniFile, settingsContainer):
    configParser = configparser.ConfigParser()
    configParser.read(iniFile)
    settingsContainer.PickemAdminUsername = configParser.get("ADMIN", "PICKEM_ADMIN_USERNAME")
    settingsContainer.PickemAdminPassword = configParser.get("ADMIN", "PICKEM_ADMIN_PASSWORD")
    settingsContainer.PickemServerBaseUrl = configParser.get("URLS", "PICKEM_SERVER_BASE_URL")

def setServerSettings(apiClient, settingsContainer):
    # TODO current week not linked to league?
    systemSettings = apiClient.readSystemSettings()
    settingsContainer.PickemWeekNumber = systemSettings['currentWeekRef']
    settingsContainer.PickemSeasonCode = systemSettings['seasonCodeRef']
    settingsContainer.PickemNcaaSeasonCode = systemSettings['ncaaSeasonCodeRef']

def setupArgumentParsers():
    # arg setup
    argParser = argparse.ArgumentParser()
    subArgParsers = argParser.add_subparsers()

    # -- setup_week sub-command
    argParserSetupWeek = subArgParsers.add_parser('setup_week')
    argParserSetupWeek.add_argument('-l', '--league', required=True, help='League code')
    argParserSetupWeek.add_argument('-w', '--week', type=int, required=True, help='Week in # e.g. 7')
    argParserSetupWeek.set_defaults(func=setupWeek)

    # -- synch_games sub-command
    argParserSetupWeek = subArgParsers.add_parser('synch_games')
    argParserSetupWeek.add_argument('-a', '--action', required=True, choices=['update', 'u', 'insert', 'i'])
    argParserSetupWeek.add_argument('-s', '--source', required=True, choices=['ncaaDefault', 'ncaaCasablanca'])
    #parser.add_argument('-ns', '--ncaa_season', type=int, required=True, help='NCAA season in YYYY e.g. 2017')
    #parser.add_argument('-ps', '--pickem_season', type=int, required=True, help='PickEm season in YY e.g. 17')
    #parser.add_argument('-w', '--week', type=int, required=True, help='Week in ## e.g. 07')
    argParserSetupWeek.set_defaults(func=synchGames)


    # -- update_spreads sub-command
    argParserSetupWeek = subArgParsers.add_parser('update_spreads')
    argParserSetupWeek.add_argument('-a', '--action', required=True, choices=['update', 'u', 'lock', 'l'])
    #parser.add_argument('-ps', '--pickem_season', type=int, required=True, help='PickEm season in YY e.g. 17')
    #parser.add_argument('-w', '--week', type=int, required=True, help='Week in ## e.g. 07')
    argParserSetupWeek.set_defaults(func=updateTeams)

    # -- update_teams sub-command
    argParserSetupWeek = subArgParsers.add_parser('update_teams')
    # parser.add_argument('-ps', '--pickem_season', type=int, required=True, help='PickEm season in YY e.g. 17')
    argParserSetupWeek.set_defaults(func=updateTeams)

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

    # build logger
    logger = pickemLogger.Logger(settings.PickemServerBaseUrl, args.func.__name__)
    # build api client
    apiClient = pickemApiClient.PickemApiClient(settings.PickemServerBaseUrl, logger)

    # login
    apiClient.authenticate(settings.PickemAdminUsername, settings.PickemAdminPassword)
    setServerSettings(apiClient, settings)


    args.func(args)

else:
    print("Missing sub command name")
    
