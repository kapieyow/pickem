#!/usr/bin/env python3

import pickemLogger
import pickemApiClient

URL_SEASON_TOKEN = "{SeasonCode}"
URL_WEEK_TOKEN = "{WeekNumber}"
NCAA_DOMAIN_URL = "http://data.ncaa.com"
NCAA_BASE_DATA_URL = "http://data.ncaa.com/sites/default/files/data/scoreboard/football/fbs/" + URL_SEASON_TOKEN + "/" + URL_WEEK_TOKEN + "/scoreboard.json"


class PickemSynchGamesHandler:
    def __init__(self, apiClient, logger):
        self.apiClient = apiClient
        self.logger = logger

    def Run(self, actionCode, sourceCode, ncaaSeason, pickemSeason, weekNumber):

        gamesModified = 0
        if ( actionCode == "insert" or pickemSeason == "i"):
            gameUrls = self.__readNcaaGames(ncaaSeason, weekNumber)

            for gameUrl in gameUrls:
                print('CUT ME: ' + gameUrl)
                
                #if ( self.__insertNcaaGame(gameUrl, pickemSeason, weekNumber) ):
                #    gamesModified += 1
                    
            self.logger.debug("Loaded (" + str(gamesModified) + ") games for NCAA season (" + str(ncaaSeason) + ") week (" + str(weekNumber) + ")")
        
        elif ( actionCode == "update" or actionCode == "u" ):
            '''
            # read pickem games used (so not all NCAA games)
            pickemGames = readPickemGames(pickemSeason, weekNumber)

            for pickemGame in pickemGames:
                if ( actionCode == 'ncaaDefault' ):
                    if ( udpateNcaaGameFromDefaultSource(pickemGame, ncaaSeason, pickemSeason, weekNumber) ):
                        gamesModified += 1
                else:
                    if ( udpateNcaaGameFromCasablanca(pickemGame, ncaaSeason, pickemSeason, weekNumber) ):
                        gamesModified += 1

            self.logger.debug("Updated (" + str(gamesModified) + ") games for NCAA season (" + str(ncaaSeason) + ") week (" + str(weekNumber) + ")")
            '''
        else:
            self.logger.wtf("Unhandled action (a) parameter (" + pickemSeason + ") why didn't the argparser catch it?")

    '''
    def __insertNcaaGame(gameUrlPath, pickemSeasonCode, weekNumber):
        url = NCAA_DOMAIN_URL + gameUrlPath

        try:
            response = requests.get(url, headers={'Content-Type': 'application/json'})
        except:
            log(PICKEM_LOG_LEVEL_ERROR, "HTTP Read timeout *probably*: " + url)
            return False


        ## TODO : need to handle failed call as OOPS, but keep trucking
        if(not response.ok):
            log(PICKEM_LOG_LEVEL_ERROR, "FAILED READ: " + url + " HTTP CODE: " + str(response.status_code))
            return False

        else:
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

            gameStart = extractGameStart(responseJson)
            pickemGamePostUrl = PICKEM_SERVER_BASE_URL + "/private/" + pickemSeasonCode + "/" + weekNumber + "/games"
            #        {
            #            "gameId": 0, 
            #            "gameStart": "2018-08-03T19:18:08.826Z", responseJson['startDate']  2017-11-18, "startTime": "10:15 PM ET",
            #            "neutralField": true,
            #            "awayTeamCode": "string", -- responseJson['away']['nameSeo']
            #            "homeTeamCode": "string" -- responseJson['home']['nameSeo']
            #        }
            gameData = '{"gameId": "' + gameId + '","gameStart": "' + gameStart + '", "neutralField": "' + neutralField + '", "awayTeamCode": "' + awayTeamCode + '", "homeTeamCode": "' + homeTeamCode + '"}'
            response = requests.post(pickemGamePostUrl, data=gameData, headers={'Content-Type': 'application/json'})

            if(not response.ok):
                log(PICKEM_LOG_LEVEL_ERROR, "FAILED SAVE: " + url + " HTTP CODE: " + str(response.status_code))
                return False
            else:
                print("SUCCESS SAVE: " + url)
                return True
    '''
    
    def __readNcaaGames(ncaaSeason, week):
        self.logger.debug("Reading NCAA Games...")

        url = NCAA_BASE_DATA_URL
        url = url.replace(URL_SEASON_TOKEN, str(ncaaSeason))
        if (week < 10):
            url = url.replace(URL_WEEK_TOKEN, "0" + str(week))
        else:
            url = url.replace(URL_WEEK_TOKEN, str(week))

        responseJson = self.apiClient.getApi(url, self.apiClient.jwt)

        games = list()

        for day in responseJson['scoreboard']:
            for game in day['games']:
                games.append(game)

        self.logger.debug("Read " + str(len(games)) + " games")

        return games

'''    def __readPickemGames(pickemSeasonCode, weekNumber):
        pickemAllGamesUrl = PICKEM_SERVER_BASE_URL + "/private/" + str(pickemSeasonCode) + "/" + str(weekNumber) + "/games_in_any_league"
        print(pickemAllGamesUrl)
        response = requests.get(pickemAllGamesUrl, headers={'Content-Type': 'application/json'})

        if(not response.ok):
            response.raise_for_status()

        return response.json()


    def __udpateNcaaGameFromDefaultSource(pickemGameJson, ncaaSeason, pickemSeason, weekNumber):

        # TODO making several URL assumptions here e.g. "fbs"
        url = NCAA_DOMAIN_URL + "/sites/default/files/data/game/football/fbs/" + str(ncaaSeason) + "/"

        # TODO fix this terrible "date" parsing. Example date value "2017-09-02T19:30:00"
        dateParts = pickemGameJson['gameStart'].split("T")[0].split("-")

        # append month/day
        url = url + dateParts[1] + "/" + dateParts[2] + "/"
        # append away team ncaa code "-" home team ncaa code
        url = url + pickemGameJson['awayTeam']['team']['teamCode'] + "-" + pickemGameJson['homeTeam']['team']['teamCode'] + "/"
        url = url + "gameinfo.json"

        try:
            response = requests.get(url, headers={'Content-Type': 'application/json'})
        except:
            log(PICKEM_LOG_LEVEL_ERROR, "HTTP Read timeout *probably*: " + url)
            return False

        if(not response.ok):
            log(PICKEM_LOG_LEVEL_ERROR, "FAILED READ: " + url + " HTTP CODE: " + str(response.status_code))
            return False

        else:
            responseJson = response.json()

            # game state
            ncaaGameState = responseJson['gameState'] # pre, cancelled, final, TODO??
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
                log(PICKEM_LOG_LEVEL_WARN, "Unhandled NCAA game state (" + ncaaGameState + ") defaulting to InGame. " + url)
                gameState = "InGame"
            
            gameStart = extractGameStart(responseJson['startTimeEpoch'])
            lastUpdated = responseJson['updatedTimestamp']
            currentPeriod = responseJson['currentPeriod']
            ncaaTimeClock = responseJson['timeclock']
            timeClock = parseTimeClock(ncaaTimeClock)
            ncaaAwayTeamScore = responseJson['away']['currentScore']

            if ( ncaaAwayTeamScore == "" ):
                awayTeamScore = 0
            else:
                awayTeamScore = int(ncaaAwayTeamScore)

            ncaaHomeTeamScore = responseJson['home']['currentScore']
            if ( ncaaHomeTeamScore == "" ):
                homeTeamScore = 0
            else:
                homeTeamScore = int(ncaaHomeTeamScore)

            # /api/private/{SeasonCode}/{WeekNumber}/games/{GameId}
            pickemGamePutUrl = PICKEM_SERVER_BASE_URL + "/private/" + str(pickemSeason) + "/" + str(weekNumber) + "/games/" + str(pickemGameJson['gameId'])
            print(pickemGamePutUrl)
            #    {
            #        "lastUpdated": "2018-08-21T23:10:04.783Z",
            #        "gameState": "SpreadNotSet",
            #        "gameStart": "2018-08-21T23:10:04.783Z",
            #        "currentPeriod": "string",
            #        "timeClock": "string",
            #        "awayTeamScore": 0,
            #        "homeTeamScore": 0
            #    }
            gameData = '{"lastUpdated": "' + lastUpdated + '","gameState": "' + gameState + '", "gameStart": "' + gameStart + '", "currentPeriod": "' + currentPeriod + '", "timeClock": "' + timeClock + '", "awayTeamScore": "' + str(awayTeamScore) + '", "homeTeamScore": "' + str(homeTeamScore) + '"}'
            response = requests.put(pickemGamePutUrl, data=gameData, headers={'Content-Type': 'application/json'})

            if(not response.ok):
                log(PICKEM_LOG_LEVEL_ERROR, "FAILED PUT: " + url + " HTTP CODE: " + str(response.status_code))
                return False
            else:
                print("SUCCESS PUT: " + url)
                return True

            return True

    def __udpateNcaaGameFromCasablanca(pickemGameJson, ncaaSeason, pickemSeason, weekNumber):

        # TODO making several URL assumptions here e.g. "fbs"
        url = NCAA_DOMAIN_URL + "/casablanca/game/football/fbs/" + str(ncaaSeason) + "/"

        # TODO fix this terrible "date" parsing. Example date value "2017-09-02T19:30:00"
        dateParts = pickemGameJson['gameStart'].split("T")[0].split("-")

        # append month/day
        url = url + dateParts[1] + "/" + dateParts[2] + "/"
        # append away team ncaa code "-" home team ncaa code
        url = url + pickemGameJson['awayTeam']['team']['teamCode'] + "-" + pickemGameJson['homeTeam']['team']['teamCode'] + "/"
        url = url + "gameInfo.json"

        try:
            response = requests.get(url, headers={'Content-Type': 'application/json'})
        except:
            log(PICKEM_LOG_LEVEL_ERROR, "HTTP Read timeout *probably*: " + url)
            return False

        if(not response.ok):
            log(PICKEM_LOG_LEVEL_ERROR, "FAILED READ: " + url + " HTTP CODE: " + str(response.status_code))
            return False

        else:
            responseJson = response.json()

            # game state
            ncaaGameState = responseJson['status']['gameState'] # pre, cancelled, final, TODO??
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
                log(PICKEM_LOG_LEVEL_WARN, "Unhandled NCAA game state (" + ncaaGameState + ") defaulting to InGame. " + url)
                gameState = "InGame"
            
            gameStart = extractGameStart(responseJson['status']['startTimeEpoch'])
            lastUpdated = responseJson['status']['updatedTimestamp']
            currentPeriod = responseJson['status']['currentPeriod']
            ncaaTimeClock = responseJson['status']['clock']
            timeClock = parseTimeClock(ncaaTimeClock)
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

            # /api/private/{SeasonCode}/{WeekNumber}/games/{GameId}
            pickemGamePutUrl = PICKEM_SERVER_BASE_URL + "/private/" + str(pickemSeason) + "/" + str(weekNumber) + "/games/" + str(pickemGameJson['gameId'])
            print(pickemGamePutUrl)
            #    {
            #        "lastUpdated": "2018-08-21T23:10:04.783Z",
            #        "gameState": "SpreadNotSet",
            #        "gameStart": "2018-08-21T23:10:04.783Z",
            #        "currentPeriod": "string",
            #        "timeClock": "string",
            #        "awayTeamScore": 0,
            #        "homeTeamScore": 0
            #    }
            gameData = '{"lastUpdated": "' + lastUpdated + '","gameState": "' + gameState + '", "gameStart": "' + gameStart + '", "currentPeriod": "' + currentPeriod + '", "timeClock": "' + timeClock + '", "awayTeamScore": "' + str(awayTeamScore) + '", "homeTeamScore": "' + str(homeTeamScore) + '"}'
            response = requests.put(pickemGamePutUrl, data=gameData, headers={'Content-Type': 'application/json'})

            if(not response.ok):
                log(PICKEM_LOG_LEVEL_ERROR, "FAILED PUT: " + url + " HTTP CODE: " + str(response.status_code))
                return False
            else:
                print("SUCCESS PUT: " + url)
                return True

            return True

    def __extractGameStart(startTimeEpoch):
        epochStartInt = int(startTimeEpoch)
        gameStart = time.strftime('%Y-%m-%d %H:%M:%S', time.localtime(epochStartInt))
        return gameStart


    def __parseTimeClock(ncaaTimeClock):
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

'''

