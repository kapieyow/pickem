#!/usr/bin/env python3

import datetime;
import json
import psycopg2
from psycopg2.extras import Json
import requests
from bs4 import BeautifulSoup

class Jsonable:
	def toJSON(self):
        	return json.dumps(self, default=lambda o: o.__dict__)


# "configs"
SPREADER_VERSION = "0.4.10"
DB_CONN_STRING = "dbname=PickemSpreads user=kip"


print("---------------------------------")
print("  Pickem Spread Loader - {0:s} ".format(SPREADER_VERSION))
print("---------------------------------")

webRequest = requests.get("https://www.thespread.com/ncaa-college-football-public-betting-chart")
webHtml = webRequest.text

soup = BeautifulSoup(webHtml, "html.parser")

contentTable = soup.find("div", id="pb")
containerDiv = contentTable.find("div", recursive=False);

# set load time only once so all inserts have the same run time
loaderRunTime = datetime.datetime.now()

# fun with db conn
dbConn = psycopg2.connect(DB_CONN_STRING)
dbCursor = dbConn.cursor()

gameCount = 0

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

	# spread in two "columns" (divs) over
	thisDataDiv = thisDataDiv.next_sibling.next_sibling
	thisDataDiv = thisDataDiv.find_next("div", class_="child-current")
	textsInDiv = thisDataDiv.get_text("|", strip=True).split("|")
	spreadValue = textsInDiv[1]	
	
	nextGameSpread.SpreadToVisitor = spreadValue[:spreadValue.find(" ")]

	spreadJson = nextGameSpread.toJSON()
	print (spreadJson)

	sql = "INSERT INTO SpreadLoads (SpreadData, Loaded, SpreadLoaderVersion) VALUES (%s, %s, %s)"	
	dbCursor.execute(sql, (spreadJson, loaderRunTime, SPREADER_VERSION,))
	dbConn.commit()
	gameCount += 1


dbCursor.close()
dbConn.close()

print ("read {0} game spreads".format(gameCount))	





