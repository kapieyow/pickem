#!/usr/bin/env python3

import argparse
import datetime
import json
import requests

# "configs"
VERSION = "0.3.3"

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


def loadNcaaGame(gameUrlPath, pickemSeasonCode, weekNumber):
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

        # TODO better time handling. Example value "10:15 PM ET"
        # dt = datetime.datetime()
        timeParts = responseJson['startTime'].split(" ")
        hourMins = timeParts[0].split(":")
        if ( startTime == "TBA" ):
            # Start Time To Be Announced this means no start time yet and the parses above are empty, post as zeros (midnight) will be updated in game synchs later
            log(PICKEM_LOG_LEVEL_WARN, "Game start TBA" + url)
            gameStart = gameStart + "00:00:00"
        elif ( hourMins[0] == "12" and timeParts[1] == "AM" ):
            gameStart = gameStart + "00:" + hourMins[1] + ":00"
        elif ( hourMins[0] != "12" and timeParts[1] == "PM" ):
            gameStart = gameStart + str(int(hourMins[0]) + 12) + ":" + hourMins[1] + ":00"
        else:
            gameStart = gameStart + hourMins[0] + ":" + hourMins[1] + ":00"

#        {
#            "gameId": 0, 
#            "gameStart": "2018-08-03T19:18:08.826Z", responseJson['startDate']  2017-11-18, "startTime": "10:15 PM ET",
#            "neutralField": true,
#            "awayTeamCode": "string", -- responseJson['away']['nameSeo']
#            "homeTeamCode": "string" -- responseJson['home']['nameSeo']
#        }
        pickemGamePostUrl = PICKEM_SERVER_BASE_URL + "/private/" + pickemSeasonCode + "/" + weekNumber + "/games"
        gameData = '{"gameId": "' + gameId + '","gameStart": "' + gameStart + '", "neutralField": "' + neutralField + '", "awayTeamCode": "' + awayTeamCode + '", "homeTeamCode": "' + homeTeamCode + '"}'
        response = requests.post(pickemGamePostUrl, data=gameData, headers={'Content-Type': 'application/json'})

        if(not response.ok):
            log(PICKEM_LOG_LEVEL_ERROR, "FAILED SAVE: " + url + " HTTP CODE: " + str(response.status_code))
            return False
        else:
            print("SUCCESS SAVE: " + url)
            return True

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
        if ( loadNcaaGame(gameUrl, str(args.pickem_season), str(args.week)) ):
            gamesModified += 1
            
    log(PICKEM_LOG_LEVEL_INFO, "Loaded (" + str(gamesModified) + ") games for NCAA season (" + str(args.ncaa_season) + ") week (" + str(args.week) + ")")

elif ( args.action == "update" or args.action == "u" ):
    # TODO call server to get full game list
    log(PICKEM_LOG_LEVEL_INFO, "Updated (" + str(gamesModified) + ") games for NCAA season (" + str(args.ncaa_season) + ") week (" + str(args.week) + ")")

else:
    log(PICKEM_LOG_LEVEL_WTF, "Unhandled action (a) parameter (" + args.action + ") why didn't the argparser catch it?")
