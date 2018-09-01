#!/usr/bin/env python3

import argparse
import datetime
import json
import requests

# "configs"
VERSION = "0.3.4"

URL_SEASON_TOKEN = "{SeasonCode}"
URL_WEEK_TOKEN = "{WeekNumber}"
NCAA_DOMAIN_URL = "http://data.ncaa.com"
NCAA_BASE_DATA_URL = "http://data.ncaa.com/sites/default/files/data/scoreboard/football/fbs/" + URL_SEASON_TOKEN + "/" + URL_WEEK_TOKEN + "/scoreboard.json"
PICKEM_COMPONENT_NAME = "Pick'Em NCAA Game Loader"
PICKEM_SERVER_BASE_URL = "http://localhost:51890/api"
PICKEM_LOG_LEVEL_DEBUG = "DEBUG"
PICKEM_LOG_LEVEL_INFO = "INFO"
PICKEM_LOG_LEVEL_WARN = "WARN"
PICKEM_LOG_LEVEL_ERROR = "ERROR"
PICKEM_LOG_LEVEL_WTF = "WTF"

# Log function
def log(logLevel, message):
    print(logLevel + ": " + message)

    pickemLogUrl = PICKEM_SERVER_BASE_URL + "/logs"
    logData = '{"component": "' + PICKEM_COMPONENT_NAME + '","logMessage": "' + message + '", "logLevel": "' + logLevel + '"}'
    response = requests.post(pickemLogUrl, data=logData, headers={'Content-Type': 'application/json'})

    if(not response.ok):
        response.raise_for_status()

    return

def insertNcaaGame(gameUrlPath, pickemSeasonCode, weekNumber):
    url = NCAA_DOMAIN_URL + gameUrlPath

    try:
        response = requests.get(url, headers={'Content-Type': 'application/json'})
    except:
        log(PICKEM_LOG_LEVEL_ERROR, "HTTP Read timeout *probably*: " + url)
        return False


    if(not response.ok):
        log(PICKEM_LOG_LEVEL_ERROR, "FAILED READ: " + url + " HTTP CODE: " + str(response.status_code))
        return False

    else:
        responseJson = response.json()

        gameId = responseJson['id']
        neutralField = "false"
        awayTeamCode = responseJson['away']['nameSeo']
        homeTeamCode = responseJson['home']['nameSeo']
        gameStart = responseJson['startDate'] + "T" 
        startTime = responseJson['startTime']

        if ( startTime == "Cancelled" ):
            log(PICKEM_LOG_LEVEL_WARN, "Game Cancelled: " + url)
            return False
        elif ( startTime == "Postponed" ):
            log(PICKEM_LOG_LEVEL_WARN, "Game Postponed: " + url)
            return False

        gameStart = extractGameStart(responseJson)
        pickemGamePostUrl = PICKEM_SERVER_BASE_URL + "/private/" + pickemSeasonCode + "/" + weekNumber + "/games"
        #        {
        #            "gameId": 0, 
        #            "gameStart": "2018-08-03T19:18:08.826Z", responseJson['startDate']  2017-11-18, "startTime": "10:15 PM ET",
        #            "neutralField": true,
        #            "awayTeamCode": "string", -- responseJson['away']['nameSeo']
        #            "homeTeamCode": "string" -- responseJson['home']['nameSeo']
        #        }
        gameData = '{"gameId": "' + gameId + '","gameStart": "' + gameStart + '", "neutralField": "' + neutralField + '", "awayTeamCode": "' + awayTeamCode + '", "homeTeamCode": "' + homeTeamCode + '"}'
        response = requests.post(pickemGamePostUrl, data=gameData, headers={'Content-Type': 'application/json'})

        if(not response.ok):
            log(PICKEM_LOG_LEVEL_ERROR, "FAILED SAVE: " + url + " HTTP CODE: " + str(response.status_code))
            return False
        else:
            print("SUCCESS SAVE: " + url)
            return True

def readNcaaGames(ncaaSeason, week):
    print("Reading NCAA Games...")

    url = NCAA_BASE_DATA_URL
    url = url.replace(URL_SEASON_TOKEN, str(ncaaSeason))
    if (week < 10):
        url = url.replace(URL_WEEK_TOKEN, "0" + str(week))
    else:
        url = url.replace(URL_WEEK_TOKEN, str(week))

    print("URL: " + url)

    response = requests.get(url, headers={'Content-Type': 'application/json'})

    if(not response.ok):
        response.raise_for_status()

    responseJson = response.json()

    games = list()

    for day in responseJson['scoreboard']:
        for game in day['games']:
            games.append(game)

    print("Read " + str(len(games)) + " games")

    return games

def readPickemGames(pickemSeasonCode, weekNumber):
    pickemAllGamesUrl = PICKEM_SERVER_BASE_URL + "/private/" + str(pickemSeasonCode) + "/" + str(weekNumber) + "/games_in_any_league"
    print(pickemAllGamesUrl)
    response = requests.get(pickemAllGamesUrl, headers={'Content-Type': 'application/json'})

    if(not response.ok):
        response.raise_for_status()

    return response.json()


def udpateNcaaGame(pickemGameJson, ncaaSeason, pickemSeason, weekNumber):

    # TODO making several URL assumptions here e.g. "fbs"
    url = NCAA_DOMAIN_URL + "/sites/default/files/data/game/football/fbs/" + str(ncaaSeason) + "/"

    # TODO fix this terrible "date" parsing. Example date value "2017-09-02T19:30:00"
    dateParts = pickemGameJson['gameStart'].split("T")[0].split("-")

    # append month/day
    url = url + dateParts[1] + "/" + dateParts[2] + "/"
    # append away team ncaa code "-" home team ncaa code
    url = url + pickemGameJson['awayTeam']['team']['teamCode'] + "-" + pickemGameJson['homeTeam']['team']['teamCode'] + "/"
    url = url + "gameinfo.json"

    try:
        response = requests.get(url, headers={'Content-Type': 'application/json'})
    except:
        log(PICKEM_LOG_LEVEL_ERROR, "HTTP Read timeout *probably*: " + url)
        return False

    if(not response.ok):
        log(PICKEM_LOG_LEVEL_ERROR, "FAILED READ: " + url + " HTTP CODE: " + str(response.status_code))
        return False

    else:
        responseJson = response.json()

        # game state
        ncaaGameState = responseJson['gameState'] # pre, cancelled, final, TODO??
        gameState = "NOT_SET_YET" # SpreadNotSet, SpreadSet, InGame, Final, Cancelled

        if ( ncaaGameState == "cancelled" ):
            gameState = "Cancelled"
        elif ( ncaaGameState == "final" ):
            gameState = "Final"
        elif ( ncaaGameState == "pre" ):
            # game has not started don't mess with spread set or not status
            gameState = pickemGameJson['gameState']
        elif ( ncaaGameState == "live" ):
            # game has not started don't mess with spread set or not status
            gameState = "InGame"
        else:
            # In game?
            log(PICKEM_LOG_LEVEL_WARN, "Unhandled NCAA game state (" + ncaaGameState + ") defaulting to InGame. " + url)
            gameState = "InGame"
        
        gameStart = extractGameStart(responseJson)
        lastUpdated = responseJson['updatedTimestamp']
        currentPeriod = responseJson['currentPeriod']
        ncaaTimeClock = responseJson['timeclock']
        # TODO parse this correctly
        timeClock = "00:00:00"
        ncaaAwayTeamScore = responseJson['away']['currentScore']
        if ( ncaaAwayTeamScore == "" ):
            awayTeamScore = 0
        else:
            awayTeamScore = int(ncaaAwayTeamScore)

        ncaaHomeTeamScore = responseJson['home']['currentScore']
        if ( ncaaHomeTeamScore == "" ):
            homeTeamScore = 0
        else:
            homeTeamScore = int(ncaaHomeTeamScore)

        # /api/private/{SeasonCode}/{WeekNumber}/games/{GameId}
        pickemGamePutUrl = PICKEM_SERVER_BASE_URL + "/private/" + str(pickemSeason) + "/" + str(weekNumber) + "/games/" + str(pickemGameJson['gameId'])
        print(pickemGamePutUrl)
        #    {
        #        "lastUpdated": "2018-08-21T23:10:04.783Z",
        #        "gameState": "SpreadNotSet",
        #        "gameStart": "2018-08-21T23:10:04.783Z",
        #        "currentPeriod": "string",
        #        "timeClock": "string",
        #        "awayTeamScore": 0,
        #        "homeTeamScore": 0
        #    }
        gameData = '{"lastUpdated": "' + lastUpdated + '","gameState": "' + gameState + '", "gameStart": "' + gameStart + '", "currentPeriod": "' + currentPeriod + '", "timeClock": "' + timeClock + '", "awayTeamScore": "' + str(awayTeamScore) + '", "homeTeamScore": "' + str(homeTeamScore) + '"}'
        response = requests.put(pickemGamePutUrl, data=gameData, headers={'Content-Type': 'application/json'})

        if(not response.ok):
            log(PICKEM_LOG_LEVEL_ERROR, "FAILED PUT: " + url + " HTTP CODE: " + str(response.status_code))
            return False
        else:
            print("SUCCESS PUT: " + url)
            return True

        return True


def extractGameStart(ncaaJson):
    gameStart = ncaaJson['startDate'] + "T" 
    startTime = ncaaJson['startTime']

    # TODO better time handling. Example value "10:15 PM ET"
    # dt = datetime.datetime()
    timeParts = ncaaJson['startTime'].split(" ")
    hourMins = timeParts[0].split(":")
    if ( startTime == "TBA" ):
        gameStart = gameStart + "00:00:00"
    elif ( hourMins[0] == "12" and timeParts[1] == "AM" ):
        gameStart = gameStart + "00:" + hourMins[1] + ":00"
    elif ( hourMins[0] != "12" and timeParts[1] == "PM" ):
        gameStart = gameStart + str(int(hourMins[0]) + 12) + ":" + hourMins[1] + ":00"
    else:
        gameStart = gameStart + hourMins[0] + ":" + hourMins[1] + ":00"

    return gameStart

# +++++++
#  Main
# +++++++

# Command Line Interface
# - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
parser = argparse.ArgumentParser(description='Load NCAA games for a week')
parser.add_argument('-ns', '--ncaa_season', type=int, required=True, help='NCAA season in YYYY e.g. 2017')
parser.add_argument('-ps', '--pickem_season', type=int, required=True, help='PickEm season in YY e.g. 17')
parser.add_argument('-w', '--week', type=int, required=True, help='Week in ## e.g. 07')
parser.add_argument('-a', '--action', required=True, choices=['update', 'u', 'insert', 'i'])
args = parser.parse_args()

print("----------------------------------------")
print("  {0:s} - {1:s} ".format(PICKEM_COMPONENT_NAME, VERSION))
print("----------------------------------------")

gamesModified = 0

if ( args.action == "insert" or args.action == "i"):

    gameUrls = readNcaaGames(args.ncaa_season, args.week)

    for gameUrl in gameUrls:
        if ( insertNcaaGame(gameUrl, str(args.pickem_season), str(args.week)) ):
            gamesModified += 1
            
    log(PICKEM_LOG_LEVEL_INFO, "Loaded (" + str(gamesModified) + ") games for NCAA season (" + str(args.ncaa_season) + ") week (" + str(args.week) + ")")

elif ( args.action == "update" or args.action == "u" ):
    # read pickem games used (so not all NCAA games)
    pickemGames = readPickemGames(args.pickem_season, args.week)

    for pickemGame in pickemGames:
        if ( udpateNcaaGame(pickemGame, args.ncaa_season, args.pickem_season, args.week) ):
            gamesModified += 1

    log(PICKEM_LOG_LEVEL_INFO, "Updated (" + str(gamesModified) + ") games for NCAA season (" + str(args.ncaa_season) + ") week (" + str(args.week) + ")")

else:
    log(PICKEM_LOG_LEVEL_WTF, "Unhandled action (a) parameter (" + args.action + ") why didn't the argparser catch it?")
