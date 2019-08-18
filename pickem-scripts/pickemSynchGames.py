#!/usr/bin/env python3
import datetime
import json
import pickemLogger
import pickemApiClient
import re
import requests
import time
from dateutil.parser import parse
import pytz

URL_SEASON_TOKEN = "{SeasonCode}"
URL_WEEK_TOKEN = "{WeekNumber}"
NCAA_DOMAIN_URL = "http://data.ncaa.com"
NCAA_BASE_DATA_URL = "https://data.ncaa.com/casablanca/scoreboard/football/fbs/" + URL_SEASON_TOKEN + "/" + URL_WEEK_TOKEN + "/scoreboard.json"

class Jsonable:
	def toJSON(self):
        	return json.dumps(self, default=lambda o: o.__dict__)

class PickemSynchGamesHandler:
    def __init__(self, apiClient, logger):
        self.apiClient = apiClient
        self.logger = logger

    def Run(self, actionCode, espnSeason, pickemSeason, weekNumber, dumpJson):

        gamesModified = 0
        if ( actionCode == "insert" or actionCode == "i" ):

            pickemTeams = self.apiClient.readPickemTeams()

            # NOTE/TODO: espn season type hard coded to 2, think this is regular season 
            espnGames = self.__readEspnGames(espnSeason, 2, weekNumber, dumpJson)

            for espnGameId in espnGames:

                espnGame = espnGames[espnGameId]

                # match to team
                matchedAwayPickemTeam = self.__findPickemTeamByEspnAbbreviation(pickemTeams, espnGame.awayTeamAbbreviation)
                matchedHomePickemTeam = self.__findPickemTeamByEspnAbbreviation(pickemTeams, espnGame.homeTeamAbbreviation)

                if ( matchedAwayPickemTeam == None ):
                    self.logger.warn("No Pickem team for espn away team (" + espnGame.awayTeamAbbreviation + ") - (" + espnGame.awayTeamDisplayName + ")")
                elif ( matchedHomePickemTeam == None ):
                    self.logger.warn("No Pickem team for espn away team (" + espnGame.homeTeamAbbreviation + ") - (" + espnGame.homeTeamDisplayName + ")")
                else:
                    self.apiClient.insertGame(
                        pickemSeason, 
                        weekNumber, 
                        espnGame.gameId, 
                        espnGame.gameStart, 
                        None, 
                        espnGame.neutralSite, 
                        matchedAwayPickemTeam['teamCode'], 
                        matchedHomePickemTeam['teamCode']
                        )
                    gamesModified += 1

            self.logger.info("Added (" + str(gamesModified) + ") games for season (" + str(espnSeason) + ") week (" + str(weekNumber) + ")")
 
        
        elif ( actionCode == "update" or actionCode == "u" ):

            # read pickem games used (so not all NCAA games)
            pickemGames = self.apiClient.readPickemGamesAnyLeague(pickemSeason, weekNumber)

            espnGames = self.__readEspnGames(espnSeason, 2, weekNumber, dumpJson)

            # loop through pickem games and match to espn
            for pickemGame in pickemGames:
                pickemGameId = str(pickemGame['gameId'])
                if ( pickemGameId in espnGames ):
                    matchedEspnGame = espnGames[pickemGameId]
                    # if no game state change (is before game start)
                    # pass back pickem state which will have spread status etc.
                    if ( matchedEspnGame.gameState == None ):
                        matchedEspnGame.gameState = pickemGame['gameState']

                    self.apiClient.updateGame(
                        matchedEspnGame.gameId,
                        matchedEspnGame.gameStart,
                        matchedEspnGame.lastUpdated,
                        matchedEspnGame.gameState,
                        matchedEspnGame.currentPeriod,
                        matchedEspnGame.timeClock,
                        matchedEspnGame.awayTeamScore,
                        matchedEspnGame.homeTeamScore,
                        None
                    )
                    gamesModified += 1
                else:
                    self.logger.info("Pick'em game id (" + str(pickemGameId) + ") not found in the ESPN game data")

            self.logger.info("Updated (" + str(gamesModified) + ") games from ESPN data for week (" + str(weekNumber) + ")")

        else:
            self.logger.wtf("Unhandled action (a) parameter (" + actionCode + ") why didn't the argparser catch it?")

    def __dumpJsonToFile(self, filePrefix, jsonData):
        outputFileName = filePrefix + "_" + datetime.datetime.now().strftime('%Y%m%d-%H%M%S.%f') + ".json"
        prettyJson = json.dumps(jsonData, indent=4)
        outputFile = open(outputFileName, "w")
        outputFile.write(prettyJson)
        self.logger.debug("Dumped json to: " + outputFileName)

    def __findPickemTeamByEspnAbbreviation(self, pickemTeams, espnAbbreviation):
        # prolly a betta way
        for pickemTeam in pickemTeams:
            if ( pickemTeam['espnAbbreviation'] == espnAbbreviation ):
                return pickemTeam

         # not found
        return None

    def __readEspnGames(self, espnSeason, espnSeasonType, week, dumpJson):
        # NOTE: group 80 == FBS (1-A)
        url = "http://www.espn.com/college-football/scoreboard/_/group/80/year/" + str(espnSeason) + "/seasontype/" + str(espnSeasonType) + "/week/" + str(week)

        espnHtml = self.apiClient.getHtml(url)

        # TODO: Error handle
        match = re.search("\<script\>window\.espn\.scoreboardData\s*=\s*(.*);window\.espn\.scoreboardSettings \=", espnHtml)

        gamesJsonString = match.group(1)
        gamesJson = json.loads(gamesJsonString)

        if ( dumpJson ):
            self.__dumpJsonToFile("espn_scoreboardData_", gamesJson)

        espnGames = dict()

        for event in gamesJson['events']:           

            # this isn't used to pickem API, but is for script logging
            shortName = event['shortName']

            espnGameData = Jsonable()

            espnGameData.gameId = event['id']
            espnGameData.gameStart = event['competitions'][0]['startDate']
            espnGameData.neutralSite = event['competitions'][0]['neutralSite']
            espnGameData.lastUpdated = datetime.datetime.now().isoformat()

            espnGameStateCode = event['status']['type']['name']
            if ( espnGameStateCode == "STATUS_SCHEDULED" ):
                # game has not started don't mess with spread set or not status
                espnGameData.gameState = None
            elif ( espnGameStateCode == "STATUS_IN_PROGRESS" ):
                espnGameData.gameState = "InGame"
            elif ( espnGameStateCode == "STATUS_CANCELED" ):
                espnGameData.gameState = "Cancelled"
            elif ( espnGameStateCode == "STATUS_FINAL" ):
                espnGameData.gameState = "Final"
            else:
                # In game?
                self.logger.warn("Unhandled ESPN game state (" + espnGameStateCode + ") defaulting to InGame. " + shortName)
                espnGameData.gameState = "InGame"

            espnPeriodNumber = event['status']['period']
            if ( espnPeriodNumber == 1 ):
                espnGameData.currentPeriod = "1st"
            elif ( espnPeriodNumber == 2 ):
                espnGameData.currentPeriod = "2nd"
            elif ( espnPeriodNumber == 3 ):
                espnGameData.currentPeriod = "3rd"
            elif ( espnPeriodNumber == 4 ):
                espnGameData.currentPeriod = "4th"
            else:
                espnGameData.currentPeriod = str(espnPeriodNumber)

            # note pickem api expects 00:00:00 espn is just the min:sec 00:00
            espnGameData.timeClock = "00:" + event['status']['displayClock']
            espnGameData.gameTitle = None

            espnGameData.awayTeamScore = 0
            espnGameData.homeTeamScore = 0
            for competitor in event['competitions'][0]['competitors']:
                if ( competitor['homeAway'] == "away" ):
                    espnGameData.awayTeamAbbreviation = competitor['team']['abbreviation']
                    espnGameData.awayTeamDisplayName = competitor['team']['displayName']
                    espnGameData.awayTeamScore = competitor['score']
                elif ( competitor['homeAway'] == "home" ):
                    espnGameData.homeTeamAbbreviation = competitor['team']['abbreviation']
                    espnGameData.homeTeamDisplayName = competitor['team']['displayName']
                    espnGameData.homeTeamScore = competitor['score']

            self.logger.debug("==> " + event['shortName'])
            self.logger.debug(espnGameData.toJSON())

            # add to dictionary with game id as the key
            espnGames[espnGameData.gameId] = espnGameData

        return espnGames

    def __parseTimeClock(self, ncaaTimeClock):
        if ( len(ncaaTimeClock) == 0 ):
            return "00:00:00"
        else:
            ncaaTimeClockParts = ncaaTimeClock.split(":")
            if ( len(ncaaTimeClockParts) == 2 ):
                if ( len(ncaaTimeClockParts[0]) == 0 ):
                    return "00:00:" + ncaaTimeClockParts[1]
                else:
                    return "00:" + ncaaTimeClock
            else:
                return "00:00:00"
    


