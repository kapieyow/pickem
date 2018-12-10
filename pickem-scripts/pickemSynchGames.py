#!/usr/bin/env python3
import json
import pickemLogger
import pickemApiClient
import re
import requests
import time

URL_SEASON_TOKEN = "{SeasonCode}"
URL_WEEK_TOKEN = "{WeekNumber}"
NCAA_DOMAIN_URL = "http://data.ncaa.com"
NCAA_BASE_DATA_URL = "https://data.ncaa.com/casablanca/scoreboard/football/fbs/" + URL_SEASON_TOKEN + "/" + URL_WEEK_TOKEN + "/scoreboard.json"


class PickemSynchGamesHandler:
    def __init__(self, apiClient, logger):
        self.apiClient = apiClient
        self.logger = logger

    def Run(self, actionCode, ncaaSeason, pickemSeason, weekNumber, gameSource):

        gamesModified = 0
        if ( actionCode == "insert" or actionCode == "i" ):

            if ( gameSource == "ncaa" ):
                gameUrls = self.__readNcaaGames(ncaaSeason, weekNumber)

                for gameUrl in gameUrls:
                    if ( self.__insertNcaaGame(gameUrl, pickemSeason, weekNumber) ):
                        gamesModified += 1
                        
                self.logger.info("Loaded (" + str(gamesModified) + ") games for NCAA season (" + str(ncaaSeason) + ") week (" + str(weekNumber) + ")")
            else:
                # TODO : implement this
                self.logger.error("Game source (" + gameSource + ") is not supported for game synchs where the -action is insert")
        
        elif ( actionCode == "update" or actionCode == "u" ):

            # read pickem games used (so not all NCAA games)
            pickemGames = self.apiClient.readPickemGamesAnyLeague(pickemSeason, weekNumber)

            if ( gameSource == "ncaa" ):
                for pickemGame in pickemGames:
                    if ( self.__updateNcaaGameFromCasablanca(pickemGame, ncaaSeason, pickemSeason) ):
                        gamesModified += 1

                self.logger.info("Updated (" + str(gamesModified) + ") games for NCAA season (" + str(ncaaSeason) + ") week (" + str(weekNumber) + ")")

            elif ( gameSource == "espn" ):
                self.__readEspnGames()
                self.logger.error("FULLY IMPL espn update")

            else:
                self.logger.error("Game source (" + gameSource + ") is not supported for game synchs where the -action is update")

        else:
            self.logger.wtf("Unhandled action (a) parameter (" + actionCode + ") why didn't the argparser catch it?")

    def __insertNcaaGame(self, gameUrlPath, pickemSeasonCode, weekNumber):
        url = NCAA_DOMAIN_URL + "/casablanca" + gameUrlPath + "/gameInfo.json"

        responseJson = self.apiClient.getApi(url, "")

        gameId = responseJson['id']
        neutralField = "false"
        awayTeamCode = responseJson['away']['names']['seo']
        homeTeamCode = responseJson['home']['names']['seo']
        gameStart = self.__extractGameStart(responseJson['status']['startTimeEpoch'])

        try:
            self.apiClient.insertGame(pickemSeasonCode, weekNumber, gameId, gameStart, None, neutralField, awayTeamCode, homeTeamCode)
            return True
        except requests.exceptions.HTTPError:
            return False

    def __readEspnGames(self):
        # TODO : this could be better
        #url = "http://www.espn.com/college-football/scoreboard/_/group/80/year/2018/seasontype/3/week/1"
        url = "http://www.espn.com/college-football/scoreboard/_/group/80/year/2018/seasontype/2/week/14"

        espnHtml = self.apiClient.getHtml(url)

        # TODO: Error handle
        match = re.search("\<script\>window\.espn\.scoreboardData\s*=\s*(.*);window\.espn\.scoreboardSettings \=", espnHtml)

        gamesJsonString = match.group(1)
        gamesJson = json.loads(gamesJsonString)

        for event in gamesJson['events']:            
            self.logger.debug("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-")
            self.logger.debug("=- shortName " + event['shortName'])
            self.logger.debug("-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-")
            self.logger.debug("gameId = " + event['id'])
            self.logger.debug("gameStart = " + event['competitions'][0]['startDate'])
            self.logger.debug("lastUpdated =  ummm hmm.... ")
            self.logger.debug("gameState = " + event['status']['type']['state'])
            self.logger.debug("currentPeriod = " + str(event['status']['period']))
            self.logger.debug("timeClock = " + event['status']['displayClock'])
            self.logger.debug("gameTitle = None")

            for competitor in event['competitions'][0]['competitors']:
                if ( competitor['homeAway'] == "away" ):
                    self.logger.debug("awayTeamScore = " + competitor['score'])
                elif ( competitor['homeAway'] == "home" ):
                    self.logger.debug("homeTeamScore = " + competitor['score'])
        '''
        updateGame(
            self, 
            gameId,  =id
            gameStart, =date || =competitions[0]/startDate
            lastUpdated, {now}
            gameState, =status/type/state 
            currentPeriod, =status/period
            timeClock, =status/displayClock
            awayTeamScore, =competitions[0]/competitors(homeAway='away')/score
            homeTeamScore, =competitions[0]/competitors(homeAway='home')/score
            gameTitle
            ):
        '''


    
    def __readNcaaGames(self, ncaaSeason, week):
        self.logger.info("Reading NCAA Games...")

        url = NCAA_BASE_DATA_URL
        url = url.replace(URL_SEASON_TOKEN, str(ncaaSeason))
        if (week < 10):
            url = url.replace(URL_WEEK_TOKEN, "0" + str(week))
        else:
            url = url.replace(URL_WEEK_TOKEN, str(week))

        responseJson = self.apiClient.getApi(url, "")

        games = list()

        for game in responseJson['games']:
            games.append(game['game']['url'])

        self.logger.info("Read " + str(len(games)) + " games")

        return games

    
    def __updateNcaaGameFromCasablanca(self, pickemGameJson, ncaaSeason, pickemSeason):

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
            self.apiClient.updateGame(pickemGameJson['gameId'], gameStart, lastUpdated, gameState, currentPeriod, timeClock, awayTeamScore, homeTeamScore, None)
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
    


