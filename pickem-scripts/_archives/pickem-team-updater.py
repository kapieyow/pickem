#!/usr/bin/env python3

import argparse
import configparser
import datetime
import json
import requests
from bs4 import BeautifulSoup

# "configs"
VERSION = "1.2.1"

PICKEM_COMPONENT_NAME = "Pick'Em Team Updater"
PICKEM_INI = "pickem-settings.ini"
PICKEM_LOG_LEVEL_DEBUG = "DEBUG"
PICKEM_LOG_LEVEL_INFO = "INFO"
PICKEM_LOG_LEVEL_WARN = "WARN"
PICKEM_LOG_LEVEL_ERROR = "ERROR"
PICKEM_LOG_LEVEL_WTF = "WTF"
TEAM_FBS_AP_RANKINGS_URL = "https://www.ncaa.com/rankings/football/fbs/associated-press"
TEAM_FBS_STANDINGS_URL = "https://www.ncaa.com/standings/football/fbs"
TEAM_FCS_STANDINGS_URL = "https://www.ncaa.com/standings/football/fcs"

class Jsonable:
	def toJSON(self):
        	return json.dumps(self, default=lambda o: o.__dict__)

def log(logLevel, message):
    print(logLevel + ": " + message)

    pickemLogUrl = PICKEM_SERVER_BASE_URL + "/logs"
    logData = '{"component": "' + PICKEM_COMPONENT_NAME + '","logMessage": "' + message + '", "logLevel": "' + logLevel + '"}'
    response = requests.post(pickemLogUrl, data=logData, headers={'Content-Type': 'application/json'})

    if(not response.ok):
        response.raise_for_status()

def getApi(url, jwt):
    
    print("GET: " + url)
    
    if ( jwt == "" ):
        response = requests.get(url, headers={'Content-Type': 'application/json'})
    else:
        response = requests.get(url, headers={'Content-Type': 'application/json', 'authorization': "Bearer " + jwt})

    if(not response.ok):
        response.raise_for_status()
    else:
        print(response)

    return response.json()

def postToApi(url, postData, jwt):
    
    print("POST: " + url)

    if ( jwt == "" ):
        response = requests.post(url, data=postData, headers={'Content-Type': 'application/json'})
    else:
        response = requests.post(url, data=postData, headers={'Content-Type': 'application/json', 'authorization': "Bearer " + jwt})

    if(not response.ok):
        response.raise_for_status()
    else:
        print(response)

    return response.json()
    
def putToApi(url, postData, jwt):

    print("PUT: " + url)

    if ( jwt == "" ):
        response = requests.put(url, data=postData, headers={'Content-Type': 'application/json'})
    else:
        response = requests.put(url, data=postData, headers={'Content-Type': 'application/json', 'authorization': "Bearer " + jwt})

    if(not response.ok):
        response.raise_for_status()
    else:
        print(response)

    return response.json()

def getHtml(url):
    print("GET: " + url)
    response = requests.get(url)

    if(not response.ok):
        response.raise_for_status()
    else:
        print(response)

    return response.text

def loadFbsTeamWinLose(ncaaTeamStats, url):
    fbsHtml = getHtml(url)
    soup = BeautifulSoup(fbsHtml, "html.parser")

    # html parse and build 
    fbsTeamCount = 0 

    for teamTd in soup.find_all("td", class_="standings-team"):
    
        teamImg = teamTd.img
        teamImgSrc = teamImg['src']
        # get team code name, which is part of the img src
        # e.g. https://i.turner.ncaa.com/sites/default/files/images/logos/schools/g/ga-southern.24.png
        # == ga-southern
        imgUrlParts = teamImgSrc.split("/")
        imgFileName = imgUrlParts[len(imgUrlParts) - 1]
        imgFileNameParts = imgFileName.split(".")
        teamCode = imgFileNameParts[0]

        # 3 columns (tds) over is the Wins
        winsTd = teamTd.find_next("td").find_next("td").find_next("td")

        thisTeam = Jsonable()
        thisTeam.wins = winsTd.text

        # losses beside wins
        lossesTd = winsTd.find_next("td")

        thisTeam.losses = lossesTd.text

        # add to dictionary
        ncaaTeamStats[teamCode] = thisTeam

        fbsTeamCount = fbsTeamCount + 1

    log(PICKEM_LOG_LEVEL_DEBUG, "Read (" + str(fbsTeamCount) + ") FBS teams from NCAA")


def readPickemTeams(jwt):
    pickemAllTeamsUrl = PICKEM_SERVER_BASE_URL + "/teams"
    return getApi(pickemAllTeamsUrl, jwt)


def findPickemTeamByLongName(pickemTeams, teamLongName):
    # prolly a betta way
    for pickemTeam in pickemTeams:
        if ( pickemTeam['longName'] == teamLongName ):
            return pickemTeam

    # not found
    return None

def findPickemTeamByTeamCode(pickemTeams, teamCode):
    # prolly a betta way
    for pickemTeam in pickemTeams:
        if ( pickemTeam['teamCode'] == teamCode ):
            return pickemTeam

    # not found
    return None


def loadNcaaFbsRankings(ncaaTeamStats, pickemTeams, url):
    fbsHtml = getHtml(url)
    soup = BeautifulSoup(fbsHtml, "html.parser")

    containTable = soup.find("table", class_="sticky")
    
    totalRankingRead = 0
    rankingsMatched = 0

    for tableRow in containTable.find("tbody").find_all("tr"):

        rankTd = tableRow.find_next("td")
        rank = rankTd.text

        teamTd = rankTd.find_next("td")
        teamName = teamTd.text
        # some teams have the vote count at the end like Clemson (99)
        # this dumps the parens
        teamName = teamName.split(" (")[0]

        pickemTeam = findPickemTeamByLongName(pickemTeams, teamName)

        if ( pickemTeam == None ):
            log(PICKEM_LOG_LEVEL_WARN, "No pickem team found for NCAA team long name: " + teamName)
        else:
            rankingsMatched = rankingsMatched + 1
            matchedTeamStats = ncaaTeamStats[pickemTeam['teamCode']]
            matchedTeamStats.fbsRank = rank

        totalRankingRead = totalRankingRead + 1

    log(PICKEM_LOG_LEVEL_DEBUG, "Read (" + str(totalRankingRead) + ") FBS rankings from NCAA. Matched (" + str(rankingsMatched) + ")")

def updatePickemWithStats(seasonNumber, weekNumber, ncaaTeamStats, pickemTeams, jwt):

    updatedTeamCount = 0

    for teamCode, teamStats in ncaaTeamStats.items():
        pickemTeam = findPickemTeamByTeamCode(pickemTeams, teamCode)

        if ( pickemTeam == None ):
            log(PICKEM_LOG_LEVEL_WARN, "No pickem team found for NCAA team code: " + teamCode)
        else:
            # http://localhost:51890/api/teams/clemson/18/4/stats
            apiUrl = PICKEM_SERVER_BASE_URL + "/teams/" + teamCode + "/" + str(seasonNumber) + "/" + str(weekNumber) + "/stats"
            putToApi(apiUrl, teamStats.toJSON(), jwt)
            updatedTeamCount = updatedTeamCount + 1

    log(PICKEM_LOG_LEVEL_DEBUG, "Updated (" + str(updatedTeamCount) + ") pickem team's stats")
    


# +++++++
#  Main
# +++++++

# Command Line Interface
# - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
parser = argparse.ArgumentParser(description='Update teams')
parser.add_argument('-ps', '--pickem_season', type=int, required=True, help='PickEm season in YY e.g. 17')
parser.add_argument('-w', '--week', type=int, required=True, help='Week in ## e.g. 07')
args = parser.parse_args()

print("----------------------------------------")
print("  {0:s} - {1:s} ".format(PICKEM_COMPONENT_NAME, VERSION))
print("----------------------------------------")

configParser = configparser.ConfigParser()
configParser.read(PICKEM_INI)
PICKEM_SERVER_BASE_URL = configParser.get("URLS", "PICKEM_SERVER_BASE_URL")


print("Authenticating as 'root'")
postData = '''{
        "userName": "root",
        "password": "iamroot"
    }'''
responseJson = postToApi(PICKEM_SERVER_BASE_URL + "/auth/login", postData, "")

jwt = responseJson['token']


pickemTeams = readPickemTeams(jwt)

ncaaTeamStats = dict()
loadFbsTeamWinLose(ncaaTeamStats, TEAM_FBS_STANDINGS_URL)
loadNcaaFbsRankings(ncaaTeamStats, pickemTeams, TEAM_FBS_AP_RANKINGS_URL)
updatePickemWithStats(args.pickem_season, args.week, ncaaTeamStats, pickemTeams, jwt)


