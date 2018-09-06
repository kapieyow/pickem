#!/usr/bin/env python3

import argparse
import configparser
import datetime
import json
import requests
from bs4 import BeautifulSoup

class Jsonable:
	def toJSON(self):
        	return json.dumps(self, default=lambda o: o.__dict__)


# "configs"
VERSION = "0.8.12"

PICKEM_COMPONENT_NAME = "Pick'Em Spread Loader"
PICKEM_INI = "pickem.ini"
PICKEM_LOG_LEVEL_DEBUG = "DEBUG"
PICKEM_LOG_LEVEL_INFO = "INFO"
PICKEM_LOG_LEVEL_WARN = "WARN"
PICKEM_LOG_LEVEL_ERROR = "ERROR"
PICKEM_LOG_LEVEL_WTF = "WTF"
SPREAD_SITE_URL = "https://www.thespread.com/ncaa-college-football-public-betting-chart"


def getApi(url):
    response = requests.get(url, headers={'Content-Type': 'application/json'})

    if(not response.ok):
        response.raise_for_status()
    else:
        print(response)
        print(response.json())

    return response.json()


def putToApi(url, postData, jwt):
    if ( jwt == "" ):
        response = requests.put(url, data=postData, headers={'Content-Type': 'application/json'})
    else:
        response = requests.put(url, data=postData, headers={'Content-Type': 'application/json', 'authorization': "Bearer " + jwt})

    if(not response.ok):
        response.raise_for_status()
    else:
        print(response)
        print(response.json())

    return response.json()

# +++++++
#  Main
# +++++++

# Command Line Interface
# - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
parser = argparse.ArgumentParser(description='Load NCAA games for a week')
parser.add_argument('-ps', '--pickem_season', type=int, required=True, help='PickEm season in YY e.g. 17')
parser.add_argument('-w', '--week', type=int, required=True, help='Week in ## e.g. 07')
args = parser.parse_args()

print("----------------------------------------")
print("  {0:s} - {1:s} ".format(PICKEM_COMPONENT_NAME, VERSION))
print("----------------------------------------")

configParser = configparser.ConfigParser()
configParser.read(PICKEM_INI)
PICKEM_SERVER_BASE_URL = configParser.get("URLS", "PICKEM_SERVER_BASE_URL")


# get games for week
pickemGamesForWeek = getApi(PICKEM_SERVER_BASE_URL + "/private/" + str(args.pickem_season) + "/" + str(args.week) + "/games_in_any_league")

webRequest = requests.get(SPREAD_SITE_URL)
webHtml = webRequest.text

soup = BeautifulSoup(webHtml, "html.parser")

contentTable = soup.find("div", id="pb")
containerDiv = contentTable.find("div", recursive=False)

# set load time only once so all inserts have the same run time
loaderRunTime = datetime.datetime.now()

spreadsGameCount = 0
updatedPickemGameCount = 0

# Loop on "rows" one for each game
for divRow in containerDiv.find_all("div", class_="datarow"):
	nextGameSpread = Jsonable()

	# date and time in first child div
	thisDataDiv = divRow.div
	textsInDiv = thisDataDiv.get_text("|", strip=True).split("|")

	nextGameSpread.Date = textsInDiv[1]
	nextGameSpread.Time = textsInDiv[2]

	# teams in next div
	thisDataDiv = thisDataDiv.next_sibling

	nextGameSpread.VisitorTeam = thisDataDiv.find_next("span", id="tmv").string
	nextGameSpread.HomeTeam = thisDataDiv.find_next("span", id="tmh").string
	# team name may have "(N)" at the end indicating neutral field, if it do, cut it
	if len(nextGameSpread.HomeTeam) > 5 and nextGameSpread.HomeTeam[-4:] == " (N)":
		nextGameSpread.HomeTeam = nextGameSpread.HomeTeam[:-4]

	# spread in two "columns" (divs) over
	thisDataDiv = thisDataDiv.next_sibling.next_sibling
	thisDataDiv = thisDataDiv.find_next("div", class_="child-current")
	textsInDiv = thisDataDiv.get_text("|", strip=True).split("|")
	spreadValue = textsInDiv[1]	
	
	nextGameSpread.SpreadToVisitor = spreadValue[:spreadValue.find(" ")]

	spreadJson = nextGameSpread.toJSON()
	print (spreadJson)

	# see if there is a match in the pickem games
	for pickemGame in pickemGamesForWeek:
		if pickemGame['awayTeam']['team']['theSpreadName'] == nextGameSpread.VisitorTeam and pickemGame['homeTeam']['team']['theSpreadName'] == nextGameSpread.HomeTeam:
			# matched update pickem spread

			gameId = pickemGame['gameId']
			# /api/private/{SeasonCode}/{WeekNumber}/games/{GameId}/spread
			spreadPostUrl = PICKEM_SERVER_BASE_URL + "/private/" + str(args.pickem_season) + "/" + str(args.week) + "/games/" + str(gameId) + "/spread"

			# which way does the spread go?
			spreadDirection = "THIS_IS_WRONG_SHOULD_BE_SET"
			if ( nextGameSpread.SpreadToVisitor == "0" ):
				spreadDirection = "None"
				absSpread = "0"
			elif ( nextGameSpread.SpreadToVisitor[:1] == "-" ):
				spreadDirection = "ToHome"
				absSpread = nextGameSpread.SpreadToVisitor[1:]
			else:
				spreadDirection = "ToAway"
				absSpread = nextGameSpread.SpreadToVisitor[1:]

			postData = '''
			{
				"spreadDirection": "''' + spreadDirection + '''",
				"pointSpread": "''' + absSpread + '''"
			}
			'''
			putToApi(spreadPostUrl, postData, "")
			updatedPickemGameCount = updatedPickemGameCount + 1
			break

	spreadsGameCount = spreadsGameCount + 1


print ("Read {0} game spreads. Matched {1} games and updated.".format(spreadsGameCount, updatedPickemGameCount))	





