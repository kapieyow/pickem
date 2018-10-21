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
VERSION = "1.5.15-B"

PICKEM_COMPONENT_NAME = "Pick'Em Spread Loader"
PICKEM_INI = "pickem-settings.ini"
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

def lockSpreads(pickemGamesForWeek):
	for pickemGame in pickemGamesForWeek:

		gameId = pickemGame['gameId']

		# api/private/{SeasonCode}/{WeekNumber}/games/{GameId}/spread/lock 
		spreadLockPutUrl = PICKEM_SERVER_BASE_URL + "/private/" + str(args.pickem_season) + "/" + str(args.week) + "/games/" + str(gameId) + "/spread/lock"
		putToApi(spreadLockPutUrl, "", "")

	print ("Locked {0} game spreads.".format(len(pickemGamesForWeek)))	

def updateSpreads(pickemGamesForWeek):
	webRequest = requests.get(SPREAD_SITE_URL)
	webHtml = webRequest.text

	soup = BeautifulSoup(webHtml, "html.parser")

	contentTable = soup.find("div", id="pb")
	containerDiv = contentTable.find("div", recursive=False)

	spreadsGameCount = 0
	updatedPickemGameCount = 0

	# Loop on "rows" one for each game
	for divRow in containerDiv.find_all("div", class_="datarow"):
		nextGameSpread = Jsonable()
		neutralFieldGame = False

		# date and time in first child div
		thisDataDiv = divRow.div
		textsInDiv = thisDataDiv.get_text("|", strip=True).split("|")

		nextGameSpread.Date = textsInDiv[1]
		nextGameSpread.Time = textsInDiv[2]

		# teams in next div
		thisDataDiv = thisDataDiv.next_sibling

		nextGameSpread.VisitorTeam = thisDataDiv.find_next("span", id="tmv").string
		nextGameSpread.HomeTeam = thisDataDiv.find_next("span", id="tmh").string

		# HACK-O on *nix we get weird text for "Texas A&M" comes through "Texas A&M;" with the semicolon
		# TODO fix this. Better call in BeautifulSoup?
		if (nextGameSpread.VisitorTeam[-1:] == ";"):
			nextGameSpread.VisitorTeam = nextGameSpread.VisitorTeam[:-1]

		if (nextGameSpread.HomeTeam[-1:] == ";"):
			nextGameSpread.HomeTeam = nextGameSpread.HomeTeam[:-1]

		# Can be used to dump spread names
		#	print (nextGameSpread.VisitorTeam)
		#	print (nextGameSpread.HomeTeam)

		# team name may have "(N)" at the end indicating neutral field, if it do, cut it
		if len(nextGameSpread.HomeTeam) > 5 and nextGameSpread.HomeTeam[-4:] == " (N)":
			nextGameSpread.HomeTeam = nextGameSpread.HomeTeam[:-4]
			neutralFieldGame = True

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

			foundMatchingGame = False
			ncaaAndSpreadTeamsReversed = False # indicates if the home/away teams don't match between NCAA and spread data. Occurs with neutral site games sometimes.

			# NOTE: in neautral field cases the "home/away" teams don't always match
			# between NCAA data (pickem game source) and the spread data
			# this check is to see if they are reversed and if so, flip them in the data
			if neutralFieldGame and pickemGame['awayTeam']['team']['theSpreadName'] == nextGameSpread.HomeTeam and pickemGame['homeTeam']['team']['theSpreadName'] == nextGameSpread.VisitorTeam:
				foundMatchingGame = True
				ncaaAndSpreadTeamsReversed = True

			elif pickemGame['awayTeam']['team']['theSpreadName'] == nextGameSpread.VisitorTeam and pickemGame['homeTeam']['team']['theSpreadName'] == nextGameSpread.HomeTeam:
				foundMatchingGame = True
				ncaaAndSpreadTeamsReversed = False

			if foundMatchingGame:
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
					if ncaaAndSpreadTeamsReversed:
						spreadDirection = "ToAway"	
					else:
						spreadDirection = "ToHome"
					absSpread = nextGameSpread.SpreadToVisitor[1:]
				else:
					if ncaaAndSpreadTeamsReversed:
						spreadDirection = "ToHome"	
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

# +++++++
#  Main
# +++++++

# Command Line Interface
# - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
parser = argparse.ArgumentParser(description='Load NCAA games for a week')
parser.add_argument('-ps', '--pickem_season', type=int, required=True, help='PickEm season in YY e.g. 17')
parser.add_argument('-w', '--week', type=int, required=True, help='Week in ## e.g. 07')
parser.add_argument('-a', '--action', required=True, choices=['update', 'u', 'lock', 'l'])
args = parser.parse_args()

print("----------------------------------------")
print("  {0:s} - {1:s} ".format(PICKEM_COMPONENT_NAME, VERSION))
print("----------------------------------------")

configParser = configparser.ConfigParser()
configParser.read(PICKEM_INI)
PICKEM_SERVER_BASE_URL = configParser.get("URLS", "PICKEM_SERVER_BASE_URL")


# get games for week
pickemGamesForWeek = getApi(PICKEM_SERVER_BASE_URL + "/private/" + str(args.pickem_season) + "/" + str(args.week) + "/games_in_any_league")

if ( args.action == "update" or args.action == "u" ):
	updateSpreads(pickemGamesForWeek)

elif ( args.action == "lock" or args.action == "l" ):
	lockSpreads(pickemGamesForWeek)

else:
	print("Unhandled input --action " + args.action)








