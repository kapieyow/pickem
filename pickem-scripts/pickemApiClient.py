#!/usr/bin/env python3
 
import json
import requests

class PickemApiClient:
    def __init__(self, pickemServerBaseUrl, logger):
        self.pickemServerBaseUrl = pickemServerBaseUrl
        self.logger = logger

    jwt = None

    def authenticate(self, username, password):
        self.logger.info("Authenticating as (" + username + ")")

        postData = '''{
                "userName": "''' + username + '''",
                "password": "''' + password + '''"
            }'''
        responseJson = self.postToApi(self.pickemServerBaseUrl + "/auth/login", postData, "")

        self.jwt = responseJson['token']

    def lockSpread(self, gameId):
        spreadLockPutUrl = self.pickemServerBaseUrl + "/games/" + str(gameId) + "/spread/lock"
        self.putToApi(spreadLockPutUrl, "", self.jwt)

    def insertGame(
        self, 
        pickemSeasonCode, 
        weekNumber,
        gameId,
        gameStart,
        gameTitle,
        neutralField,
        awayTeamCode,
        homeTeamCode
        ):

        pickemGamePostUrl = self.pickemServerBaseUrl + "/games/" + pickemSeasonCode + "/" + str(weekNumber) 
        gameData = '''
            { 
                "gameId": "''' + str(gameId) + '''",
                "gameStart": "''' + gameStart + '''", 
                "gameTitle" : ''' + self.__handleNoneStr(gameTitle) + ''',
                "neutralField": "''' + neutralField + '''", 
                "awayTeamCode": "''' + awayTeamCode + '''", 
                "homeTeamCode": "''' + homeTeamCode + '''"
            }'''
        self.postToApi(pickemGamePostUrl, gameData, self.jwt)

    def readGame(self, gameId):
        return self.getApi(self.pickemServerBaseUrl + "/games/" + str(gameId), self.jwt)
        
    def readPickemGamesAnyLeague(self, pickemSeasonCode, weekNumber):
        pickemAllGamesUrl = self.pickemServerBaseUrl + "/" + str(pickemSeasonCode) + "/" + str(weekNumber) + "/games_in_any_league"
        return self.getApi(pickemAllGamesUrl, self.jwt)

    def readPickemTeams(self):
        return self.getApi(self.pickemServerBaseUrl + "/teams", self.jwt)
        
    def setLeagueGame(self, leagueCode, weekNumber, gameId, winPoints):
        leagueGameUrl = self.pickemServerBaseUrl + "/" + leagueCode + "/" + str(weekNumber) + "/pickemgames"
        self.postToApi(leagueGameUrl, "{ 'gameId': " + str(gameId) + ", 'winPoints': " + str(winPoints) + " }", self.jwt)
        
    def updateGame(
        self, 
        gameId,
        gameStart,
        lastUpdated,
        gameState,
        currentPeriod,
        timeClock,
        awayTeamScore,
        homeTeamScore,
        gameTitle
        ):

        pickemGamePutUrl = self.pickemServerBaseUrl + "/games/" + str(gameId)
        gameData = '''
            { 
                "lastUpdated": "''' + lastUpdated + '''",
                "gameState": "''' + gameState + '''", 
                "gameStart": "''' + gameStart + '''", 
                "currentPeriod": ''' + self.__handleNoneStr(currentPeriod) + ''', 
                "timeClock": "''' + timeClock + '''", 
                "awayTeamScore": "''' + str(awayTeamScore) + '''", 
                "homeTeamScore": "''' + str(homeTeamScore) + '''",
                "gameTitle" : ''' + self.__handleNoneStr(gameTitle) + '''
            }'''
        self.putToApi(pickemGamePutUrl, gameData, self.jwt)

    def updateLeague(self, leagueCode, currentWeekNumber):
        spreadPutUrl = self.pickemServerBaseUrl + "/" + leagueCode
        putData = '''
        {
            "currentWeekRef": ''' + str(currentWeekNumber) + '''
        }
        '''
        self.putToApi(spreadPutUrl, putData, self.jwt)

    def updateSpread(self, gameId, spreadDirection, absSpread):
        spreadPutUrl = self.pickemServerBaseUrl + "/games/" + str(gameId) + "/spread"
        putData = '''
        {
            "spreadDirection": "''' + spreadDirection + '''",
            "pointSpread": "''' + absSpread + '''"
        }
        '''
        self.putToApi(spreadPutUrl, putData, self.jwt)

    def updateTeam(self, teamCode, pickemSeasonCode, weekNumber, wins, losses, fbsRank):
        apiUrl = self.pickemServerBaseUrl + "/teams/" + teamCode + "/" + str(pickemSeasonCode) + "/" + str(weekNumber) + "/stats"
        putData = '''
        {
            "wins": ''' + str(wins) + ''',
            "losses": ''' + str(losses) + ''',
            "fbsRank": ''' + str(fbsRank) + '''
        }
        '''
        self.putToApi(apiUrl, putData, self.jwt)

    def __handleNoneStr(self, strValue):
        returnString = "null"
        if strValue is not None:
            returnString = "\"" + strValue + "\""
        return returnString

    #============================================
    #  generic HTTP methods
    #============================================
    def getApi(self, url, jwt):
        
        self.logger.debug("GET: " + url)
        
        if ( jwt == "" ):
            response = requests.get(url, headers={'Content-Type': 'application/json'})
        else:
            response = requests.get(url, headers={'Content-Type': 'application/json', 'authorization': "Bearer " + jwt})

        if(not response.ok):
            self.logger.error("HTTP GET api Failure. Code: " + str(response.status_code) + " URL: " + url)
            response.raise_for_status()
        else:
            self.logger.debug(response)

        return response.json()

    def postToApi(self, url, postData, jwt):
        
        self.logger.debug("POST: " + url)

        if ( jwt == "" ):
            response = requests.post(url, data=postData, headers={'Content-Type': 'application/json'})
        else:
            response = requests.post(url, data=postData, headers={'Content-Type': 'application/json', 'authorization': "Bearer " + jwt})

        if(not response.ok):
            self.logger.error("HTTP POST Failure. Code: " + str(response.status_code) + " URL: " + url)
            response.raise_for_status()
        else:
            self.logger.debug(response)

        return response.json()
        
    def putToApi(self, url, postData, jwt):

        self.logger.debug("PUT: " + url)

        if ( jwt == "" ):
            response = requests.put(url, data=postData, headers={'Content-Type': 'application/json'})
        else:
            response = requests.put(url, data=postData, headers={'Content-Type': 'application/json', 'authorization': "Bearer " + jwt})

        if(not response.ok):
            self.logger.error("HTTP PUT Failure. Code: " + str(response.status_code) + " URL: " + url)
            response.raise_for_status()
        else:
            self.logger.debug(response)

        return response.json()


    def getHtml(self, url):
        self.logger.debug("GET: " + url)
        response = requests.get(url)

        if(not response.ok):
            self.logger.error("HTTP GET Failure. Code: " + str(response.status_code) + " URL: " + url)
            response.raise_for_status()
        else:
            self.logger.debug(response)

        return response.text