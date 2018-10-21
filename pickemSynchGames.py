#!/usr/bin/env python3

import pickemLogger
import pickemApiClient
import requests
import time

URL_SEASON_TOKEN = "{SeasonCode}"
URL_WEEK_TOKEN = "{WeekNumber}"
NCAA_DOMAIN_URL = "http://data.ncaa.com"
NCAA_BASE_DATA_URL = "http://data.ncaa.com/sites/default/files/data/scoreboard/football/fbs/" + URL_SEASON_TOKEN + "/" + URL_WEEK_TOKEN + "/scoreboard.json"


class PickemSynchGamesHandler:
    def __init__(self, apiClient, logger):
        self.apiClient = apiClient
        self.logger = logger

    def Run(self, actionCode, ncaaSeason, pickemSeason, weekNumber):

        gamesModified = 0
        if ( actionCode == "insert" or actionCode == "i" ):
            gameUrls = self.__readNcaaGames(ncaaSeason, weekNumber)

            for gameUrl in gameUrls:
                if ( self.__insertNcaaGame(gameUrl, pickemSeason, weekNumber) ):
                    gamesModified += 1
                    
            self.logger.debug("Loaded (" + str(gamesModified) + ") games for NCAA season (" + str(ncaaSeason) + ") week (" + str(weekNumber) + ")")
        
        elif ( actionCode == "update" or actionCode == "u" ):

            # read pickem games used (so not all NCAA games)
            pickemGames = self.apiClient.readPickemGamesAnyLeague(pickemSeason, weekNumber)

            for pickemGame in pickemGames:
                if ( self.__updateNcaaGameFromCasablanca(pickemGame, ncaaSeason, pickemSeason, weekNumber) ):
                    gamesModified += 1

            self.logger.debug("Updated (" + str(gamesModified) + ") games for NCAA season (" + str(ncaaSeason) + ") week (" + str(weekNumber) + ")")

        else:
            self.logger.wtf("Unhandled action (a) parameter (" + actionCode + ") why didn't the argparser catch it?")

    def __insertNcaaGame(self, gameUrlPath, pickemSeasonCode, weekNumber):
        url = NCAA_DOMAIN_URL + gameUrlPath

        responseJson = self.apiClient.getApi(url, "")

        gameId = responseJson['id']
        neutralField = "false"
        awayTeamCode = responseJson['away']['nameSeo']
        homeTeamCode = responseJson['home']['nameSeo']
        gameStart = responseJson['startDate'] + "T" 
        startTime = responseJson['startTime']

        if ( startTime == "Cancelled" ):
            self.logger.warn("Game Cancelled: " + url)
            return False
        elif ( startTime == "Postponed" ):
            self.logger.warn("Game Postponed: " + url)
            return False

        gameStart = self.__extractGameStart(responseJson['startTimeEpoch'])

        try:
            self.apiClient.insertGame(pickemSeasonCode, weekNumber, gameId, gameStart, neutralField, awayTeamCode, homeTeamCode)
            return True
        except requests.exceptions.HTTPError:
            return False
    
    def __readNcaaGames(self, ncaaSeason, week):
        self.logger.debug("Reading NCAA Games...")

        url = NCAA_BASE_DATA_URL
        url = url.replace(URL_SEASON_TOKEN, str(ncaaSeason))
        if (week < 10):
            url = url.replace(URL_WEEK_TOKEN, "0" + str(week))
        else:
            url = url.replace(URL_WEEK_TOKEN, str(week))

        responseJson = self.apiClient.getApi(url, "")

        games = list()

        for day in responseJson['scoreboard']:
            for game in day['games']:
                games.append(game)

        self.logger.debug("Read " + str(len(games)) + " games")

        return games

    
    def __updateNcaaGameFromCasablanca(self, pickemGameJson, ncaaSeason, pickemSeason, weekNumber):

        # TODO making several URL assumptions here e.g. "fbs"
        url = NCAA_DOMAIN_URL + "/casablanca/game/football/fbs/" + str(ncaaSeason) + "/"

        # TODO fix this terrible "date" parsing. Example date value "2017-09-02T19:30:00"
        dateParts = pickemGameJson['gameStart'].split("T")[0].split("-")

        # append month/day
        url = url + dateParts[1] + "/" + dateParts[2] + "/"
        # append away team ncaa code "-" home team ncaa code
        url = url + pickemGameJson['awayTeam']['team']['teamCode'] + "-" + pickemGameJson['homeTeam']['team']['teamCode'] + "/"
        url = url + "gameInfo.json"

        responseJson = self.apiClient.getApi(url, "")

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
        
        gameStart = self.__extractGameStart(responseJson['status']['startTimeEpoch'])
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
            self.apiClient.updateGame(pickemSeason, weekNumber, pickemGameJson['gameId'], gameStart, lastUpdated, gameState, currentPeriod, timeClock, awayTeamScore, homeTeamScore)
            return True
        except requests.exceptions.HTTPError:
            return False

    def __extractGameStart(self, startTimeEpoch):
        epochStartInt = int(startTimeEpoch)
        gameStart = time.strftime('%Y-%m-%d %H:%M:%S', time.localtime(epochStartInt))
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
    


