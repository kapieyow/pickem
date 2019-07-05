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

    # TODO: cut weekNumber and use roundIdentifier instead
    def Run(self, actionCode, ncaaSeason, pickemSeason, weekNumber, roundIdentifier, gameSource, dumpJson):

        gamesModified = 0
        if ( actionCode == "insert" or actionCode == "i" ):

            if ( gameSource == "ncaa" ):
                gameUrls = self.__readNcaaGames(ncaaSeason, weekNumber, dumpJson)

                for gameUrl in gameUrls:
                    if ( self.__insertNcaaGame(gameUrl, pickemSeason, weekNumber, dumpJson) ):
                        gamesModified += 1
                        
                self.logger.info("Loaded (" + str(gamesModified) + ") games for NCAA season (" + str(ncaaSeason) + ") week (" + str(weekNumber) + ")")
            else:
                pickemTeams = self.apiClient.readPickemTeams()
                espnGames = self.__readEspnGames(roundIdentifier, dumpJson)

                for espnGameId in espnGames:

                    espnGame = espnGames[espnGameId]

                    # match to team
                    matchedAwayPickemTeam = self.__findPickemTeamByShortName(pickemTeams, espnGame.awayTeamAbbreviation)
                    matchedHomePickemTeam = self.__findPickemTeamByShortName(pickemTeams, espnGame.homeTeamAbbreviation)

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
                            matchedAwayPickemTeam.teamCode, 
                            matchedHomePickemTeam.teamCode
                            )
                        gamesModified += 1

                # TODO : implement this
                self.logger.info("Added (" + str(gamesModified) + ") games for season (" + str(ncaaSeason) + ") round (" + str(roundIdentifier) + ")")
        
        elif ( actionCode == "update" or actionCode == "u" ):

            # read pickem games used (so not all NCAA games)
            pickemGames = self.apiClient.readPickemGamesAnyLeague(pickemSeason, weekNumber)

            if ( gameSource == "ncaa" ):
                for pickemGame in pickemGames:
                    if ( self.__updateNcaaGameFromCasablanca(pickemGame, ncaaSeason, pickemSeason, dumpJson) ):
                        gamesModified += 1

                self.logger.info("Updated (" + str(gamesModified) + ") games for NCAA season (" + str(ncaaSeason) + ") week (" + str(weekNumber) + ")")

            elif ( gameSource == "espn" ):
                espnGames = self.__readEspnGames(roundIdentifier, dumpJson)

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
                self.logger.error("Game source (" + gameSource + ") is not supported for game synchs where the -action is update")

        else:
            self.logger.wtf("Unhandled action (a) parameter (" + actionCode + ") why didn't the argparser catch it?")

    def __dumpJsonToFile(self, filePrefix, jsonData):
        outputFileName = filePrefix + "_" + datetime.datetime.now().strftime('%Y%m%d-%H%M%S.%f') + ".json"
        prettyJson = json.dumps(jsonData, indent=4)
        outputFile = open(outputFileName, "w")
        outputFile.write(prettyJson)
        self.logger.debug("Dumped json to: " + outputFileName)

    def __findPickemTeamByShortName(self, pickemTeams, shortName):
        # prolly a betta way
        for pickemTeam in pickemTeams:
            if ( pickemTeam['shortName'] == shortName ):
                return pickemTeam

        # not found
        return None

    def __insertNcaaGame(self, gameUrlPath, pickemSeasonCode, weekNumber, dumpJson):
        url = NCAA_DOMAIN_URL + "/casablanca" + gameUrlPath + "/gameInfo.json"

        responseJson = self.apiClient.getApi(url, "")

        if ( dumpJson ):
            self.__dumpJsonToFile("ncaa_gameInfo_", responseJson)

        gameId = responseJson['id']
        neutralField = "false"
        awayTeamCode = responseJson['away']['names']['seo']
        homeTeamCode = responseJson['home']['names']['seo']
        gameStart = self.__extractNcaaGameStartToUtc(responseJson['status']['startTimeEpoch'])

        try:
            self.apiClient.insertGame(pickemSeasonCode, weekNumber, gameId, gameStart, None, neutralField, awayTeamCode, homeTeamCode)
            return True
        except requests.exceptions.HTTPError:
            return False

    def __readEspnGames(self, roundIdentifier, dumpJson):
        # TODO : round identifier hacked in for NCAAM
        #       as is the mens-college-basketball etc, make work for football too
        url = "http://www.espn.com/mens-college-basketball/scoreboard/_/group/100/date/" + roundIdentifier

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

            # TODO: may not be the first note?
            espnGameData.headline = event['competitions'][0]['notes'][0]['headline']
            
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

    
    def __readNcaaGames(self, ncaaSeason, week, dumpJson):
        self.logger.info("Reading NCAA Games...")

        url = NCAA_BASE_DATA_URL
        url = url.replace(URL_SEASON_TOKEN, str(ncaaSeason))
        if (week < 10):
            url = url.replace(URL_WEEK_TOKEN, "0" + str(week))
        else:
            url = url.replace(URL_WEEK_TOKEN, str(week))

        responseJson = self.apiClient.getApi(url, "")
        
        if ( dumpJson ):
            self.__dumpJsonToFile("ncaa_scoreboard_", responseJson)

        games = list()

        for game in responseJson['games']:
            games.append(game['game']['url'])

        self.logger.info("Read " + str(len(games)) + " games")

        return games

    
    def __updateNcaaGameFromCasablanca(self, pickemGameJson, ncaaSeason, pickemSeason, dumpJson):

        # TODO making several URL assumptions here e.g. "fbs"
        url = NCAA_DOMAIN_URL + "/casablanca/game/football/fbs/" + str(ncaaSeason) + "/"

        # dates in database are UTC. NCAA is ET based. 
        startDateUtc = parse(pickemGameJson['gameStart'])

        # TODO: gut this hack for old data.
        if ( startDateUtc.tzinfo == None ):
            failureMessage = "Found start date in pickem data with no timezone. Game Id: " + str(pickemGameJson['gameId'])
            self.logger.error(failureMessage)
            raise Exception(failureMessage)

        startDateEt = startDateUtc.astimezone(pytz.timezone('US/Eastern'))

        # append zero padded month/day to url
        url = url + "{0:02d}".format(startDateEt.month) + "/" +  "{0:02d}".format(startDateEt.day) + "/"
        # append away team ncaa code "-" home team ncaa code
        url = url + pickemGameJson['awayTeam']['team']['teamCode'] + "-" + pickemGameJson['homeTeam']['team']['teamCode'] + "/"
        url = url + "gameInfo.json"

        responseJson = self.apiClient.getApi(url, "")

        if ( dumpJson ):
            self.__dumpJsonToFile("ncaa_gameInfo_", responseJson)

        # game state
        ncaaGameState = responseJson['status']['gameState'] # pre, cancelled, canceled, final, live, delayed
        gameState = "NOT_SET_YET" # SpreadNotSet, SpreadSet, InGame, Final, Cancelled

        if ( ncaaGameState == "cancelled" ):
            gameState = "Cancelled"
        elif ( ncaaGameState == "canceled" ):
            gameState = "Cancelled"
        elif ( ncaaGameState == "final" ):
            gameState = "Final"
        elif ( ncaaGameState == "pre" ):
            # game has not started don't mess with spread set or not status
            gameState = pickemGameJson['gameState']
        elif ( ncaaGameState == "live" ):
            gameState = "InGame"
        elif ( ncaaGameState == "delayed" ):
            # TODO - handle Delayed 
            gameState = "InGame"
        else:
            # In game?
            self.logger.warn("Unhandled NCAA game state (" + ncaaGameState + ") defaulting to InGame. " + url)
            gameState = "InGame"
        
        gameStart = self.__extractNcaaGameStartToUtc(responseJson['status']['startTimeEpoch'])
        lastUpdated = responseJson['status']['updatedTimestamp']
        currentPeriod = responseJson['status']['currentPeriod']
        ncaaTimeClock = responseJson['status']['clock']
        timeClock = self.__parseTimeClock(ncaaTimeClock)
        ncaaAwayTeamScore = responseJson['away']['score']

        if ( ncaaAwayTeamScore == "" ):
            awayTeamScore = 0
        else:
            awayTeamScore = int(ncaaAwayTeamScore)

        ncaaHomeTeamScore = responseJson['home']['score']
        if ( ncaaHomeTeamScore == "" ):
            homeTeamScore = 0
        else:
            homeTeamScore = int(ncaaHomeTeamScore)

        try:
            self.apiClient.updateGame(pickemGameJson['gameId'], gameStart, lastUpdated, gameState, currentPeriod, timeClock, awayTeamScore, homeTeamScore, None)
            return True
        except requests.exceptions.HTTPError:
            return False

    def __extractNcaaGameStartToUtc(self, startTimeEpoch):
        epochStartInt = int(startTimeEpoch)
        gameStart = time.strftime('%Y-%m-%d %H:%M:%SZ', time.gmtime(epochStartInt))
        return gameStart


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
    


