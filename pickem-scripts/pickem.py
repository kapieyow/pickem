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
VERSION = "2.0.26"
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

# TODO: this is a one time run. Delete after in prod.
def setupBowls(args):
    
    pickemSeasonCode = "18"

    weekNumber = 16
    apiClient.insertGame(pickemSeasonCode, weekNumber, 1, "2018-12-15T13:30:00.000Z", "AutoNation Cure Bowl", "false", "tulane", "la-lafayette")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 2, "2018-12-15T14:00:00.000Z", "New Mexico Bowl", "false", "north-texas", "utah-st")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 3, "2018-12-15T15:30:00.000Z", "Mitsubishi Motors Las Vegas Bowl", "false", "arizona-st", "fresno-st")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 4, "2018-12-15T17:30:00.000Z", "Raycom Media Camellia Bowl", "false", "ga-southern", "eastern-mich")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 5, "2018-12-15T21:00:00.000Z", "R+L Carriers New Orleans Bowl", "false", "middle-tenn", "appalachian-st")

    weekNumber = 17
    apiClient.insertGame(pickemSeasonCode, weekNumber, 6, "2018-12-18T19:00:00.000Z", "Cheribundi Tart Cherry Boca Raton Bowl", "false", "uab", "northern-ill")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 7, "2018-12-19T20:00:00.000Z", "DXL Frisco Bowl", "false", "san-diego-st", "ohio")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 8, "2018-12-20T20:00:00.000Z", "Bad Boy Mowers Gasparilla Bowl", "false", "marshall", "south-fla")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 9, "2018-12-21T12:30:00.000Z", "Makers Wanted Bahamas Bowl", "false", "fiu", "toledo")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 10, "2018-12-21T16:00:00.000Z", "Famous Idaho Potato Bowl", "false", "western-mich", "byu")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 11, "2018-12-22T12:00:00.000Z", "Jared Birmingham Bowl", "false", "memphis", "wake-forest")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 12, "2018-12-22T15:30:00.000Z", "Lockheed Martin Armed Forces Bowl", "false", "houston", "army")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 13, "2018-12-22T19:00:00.000Z", "Dollar General Bowl", "false", "buffalo", "troy")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 14, "2018-12-22T22:30:00.000Z", "Hawai'i Bowl", "false", "hawaii", "louisiana-tech")

    weekNumber = 18
    apiClient.insertGame(pickemSeasonCode, weekNumber, 15, "2018-12-26T13:30:00.000Z", "SERVPRO First Responder Bowl", "false", "boston-college", "boise-st")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 16, "2018-12-26T17:15:00.000Z", "Quick Lane Bowl", "false", "minnesota", "georgia-tech")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 17, "2018-12-26T21:00:00.000Z", "Cheez-It Bowl", "false", "california", "tcu")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 18, "2018-12-27T13:30:00.000Z", "Walk-On's Independence Bowl", "false", "temple", "duke")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 19, "2018-12-27T17:15:00.000Z", "New Era Pinstripe Bowl", "false", "miami-fl", "wisconsin")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 20, "2018-12-27T21:00:00.000Z", "Academy Sports + Outdoors Texas Bowl", "false", "baylor", "vanderbilt")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 21, "2018-12-28T13:30:00.000Z", "Franklin American Mortgage Music City Bowl", "false", "purdue", "auburn")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 22, "2018-12-28T17:15:00.000Z", "Camping World Bowl", "false", "west-virginia", "syracuse")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 24, "2018-12-28T21:00:00.000Z", "Valero Alamo Bowl", "false", "iowa-st", "washington-st")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 25, "2018-12-29T12:00:00.000Z", "Chick-fil-A Peach Bowl", "false", "florida", "michigan")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 26, "2018-12-29T12:00:00.000Z", "Belk Bowl", "false", "south-carolina", "virginia")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 27, "2018-12-29T16:00:00.000Z", "College Football Playoff Semifinal at the Goodyear Cotton Bowl Classic", "false", "notre-dame", "clemson")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 28, "2018-12-29T20:00:00.000Z", "College Football Playoff Semifinal at the Capital One Orange Bowl", "false", "oklahoma", "alabama")

    weekNumber = 19
    apiClient.insertGame(pickemSeasonCode, weekNumber, 29, "2018-12-31T12:00:00.000Z", "Military Bowl Presented by Northrop Grumman", "false", "cincinnati", "virginia-tech")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 30, "2018-12-31T14:00:00.000Z", "Hyundai Sun Bowl", "false", "stanford", "pittsburgh")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 31, "2018-12-31T15:00:00.000Z", "Redbox Bowl", "false", "michigan-st", "oregon")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 32, "2018-12-31T15:45:00.000Z", "AutoZone Liberty Bowl", "false", "missouri", "oklahoma-st")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 33, "2018-12-31T19:00:00.000Z", "San Diego County Credit Union Holiday Bowl", "false", "northwestern", "utah")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 34, "2018-12-31T19:30:00.000Z", "TaxSlayer Gator Bowl", "false", "north-carolina-st", "texas-am")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 35, "2019-01-01T12:00:00.000Z", "Outback Bowl", "false", "mississippi-st", "iowa")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 36, "2019-01-01T13:00:00.000Z", "Citrus Bowl", "false", "kentucky", "penn-st")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 37, "2019-01-01T13:00:00.000Z", "PlayStation Fiesta Bowl", "false", "lsu", "ucf")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 38, "2019-01-01T12:00:00.000Z", "Rose Bowl Game Presented by Northwestern Mutual", "false", "washington", "ohio-st")
    apiClient.insertGame(pickemSeasonCode, weekNumber, 39, "2019-01-01T20:45:00.000Z", "Allstate Sugar Bowl", "false", "texas", "georgia")

    logger.info("2 oh bowls are setup!")


def setLeagueGame(args):
    gamesSet = 0

    for leagueCode in args.league_codes:
        apiClient.setLeagueGame(leagueCode, args.week, args.game_id, args.game_win_points)
        gamesSet = gamesSet + 1

        logger.info("Set (" + str(gamesSet) + ") games for league (" + leagueCode + ") in week (" + str(args.week) + ")")

def setLeagueGames(args):
    gamesSet = 0

    for leagueCode in args.league_codes:
        for gameId in args.game_ids:
            # no win points can be sent in, when setting multiple games at a time, so default to "1"
            apiClient.setLeagueGame(leagueCode, args.week, gameId, 1)
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

def updateGame(args):
    # read current data. Then update ONLY the title and re-send the rest of the current state
    # TODO: better way to do this?
    gameData = apiClient.readGame(args.game_id)
    apiClient.updateGame(args.game_id, gameData['gameStart'], gameData['lastUpdated'], gameData['gameState'], gameData['currentPeriod'], gameData['timeClock'], gameData['awayTeam']['score'], gameData['homeTeam']['score'], args.game_title)

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

    # -- set_league_game sub-command (SINGLE Game)
    subParser = subArgParsers.add_parser('set_league_game')
    subParser.add_argument('-w', '--week', type=int, required=True, help='Week number')
    subParser.add_argument('-lcs', '--league_codes', required=True, nargs='+', type=str, help='League codes')
    subParser.add_argument('-gid', '--game_id', required=True, type=int, help='Game Id')
    subParser.add_argument('-gwp', '--game_win_points', required=True, type=int, help='Game Win Points')
    subParser.set_defaults(func=setLeagueGame)

    # -- set_league_games sub-command (MULTIPLE games)
    subParser = subArgParsers.add_parser('set_league_games')
    subParser.add_argument('-w', '--week', type=int, required=True, help='Week number')
    subParser.add_argument('-lcs', '--league_codes', required=True, nargs='+', type=str, help='League codes')
    subParser.add_argument('-gids', '--game_ids', required=True, nargs='+', type=int, help='Game Ids')
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


    # -- This is a ONE TIME setup script for the 2018 bowl season
    # TODO: trash after is in prod
    subParser = subArgParsers.add_parser('_setup_bowls')
    subParser.set_defaults(func=setupBowls)


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
    
