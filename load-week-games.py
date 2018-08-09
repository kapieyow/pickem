#!/usr/bin/env python3

import argparse
import datetime
import json
import requests

# "configs"
VERSION = "0.1.1"

URL_SEASON_TOKEN = "{SeasonCode}"
URL_WEEK_TOKEN = "{WeekNumber}"
NCAA_DOMAIN_URL = "http://data.ncaa.com"
NCAA_BASE_DATA_URL = "http://data.ncaa.com/sites/default/files/data/scoreboard/football/fbs/" + URL_SEASON_TOKEN + "/" + URL_WEEK_TOKEN + "/scoreboard.json"
#NCAA_DOMAIN_URL = "http://localhost:51890"
#NCAA_BASE_DATA_URL = "http://localhost:51890/scoreboard.json"
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

def readNcaaGames():
    print("Reading NCAA Games...")

    url = NCAA_BASE_DATA_URL
    url = url.replace(URL_SEASON_TOKEN, str(args.season))
    if (args.week < 10):
        url = url.replace(URL_WEEK_TOKEN, "0" + str(args.week))
    else:
        url = url.replace(URL_WEEK_TOKEN, str(args.week))

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


def loadNcaaGame(gameUrlPath, seasonCode, weekNumber):
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
        print("SUCCESS READ: " + url)

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
        if ( hourMins[0] == "12" and timeParts[1] == "AM" ):
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
        pickemGamePostUrl = PICKEM_SERVER_BASE_URL + "/private/" + seasonCode + "/" + weekNumber + "/games"
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
parser.add_argument('-s', '--season', type=int, required=True, help='season in YYYY e.g. 2001')
parser.add_argument('-w', '--week', type=int, required=True, help='Week in ## e.g. 07')
args = parser.parse_args()

print("----------------------------------------")
print("  {0:s} - {1:s} ".format(PICKEM_COMPONENT_NAME, VERSION))
print("----------------------------------------")

gamesLoaded = 0

gameUrls = readNcaaGames()

for gameUrl in gameUrls:
    if ( loadNcaaGame(gameUrl, str(args.season), str(args.week)) ):
        gamesLoaded += 1


log(PICKEM_LOG_LEVEL_INFO, "Loaded (" + str(gamesLoaded) + ") games for season (" + str(args.season) + ") week (" + str(args.week) + ")")



