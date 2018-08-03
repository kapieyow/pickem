#!/usr/bin/env python3

import argparse
import json
import requests

# "configs"
VERSION = "0.1.1"

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

def readNcaaGames():
    print("Reading NCAA Games...")

    url = NCAA_BASE_DATA_URL
    url = url.replace(URL_SEASON_TOKEN, str(args.season))
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


def loadNcaaGame(gameUrlPath):
    url = NCAA_DOMAIN_URL + gameUrlPath

    response = requests.get(url, headers={'Content-Type': 'application/json'})

    if(not response.ok):
        print("FAILED: " + url + " HTTP CODE: " + str(response.status_code))
        return False

    else:
        responseJson = response.json()
        print("SUCCESS: " + url)
        #print("SUCCESS Game raw: " + json.dumps(responseJson))
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
    if ( loadNcaaGame(gameUrl) ):
        gamesLoaded += 1


log(PICKEM_LOG_LEVEL_INFO, "Loaded (" + str(gamesLoaded) + ") games for season (" + str(args.season) + ") week (" + str(args.week) + ")")



