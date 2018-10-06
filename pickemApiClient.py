#!/usr/bin/env python3

import json
import requests

class PickemApiClient:
    def __init__(self, pickemServerBaseUrl, logger):
        self.pickemServerBaseUrl = pickemServerBaseUrl
        self.logger = logger

    jwt = None

    def authenticate(self, username, password):
        self.logger.debug("Authenticating as (" + username + ")")

        postData = '''{
                "userName": "''' + username + '''",
                "password": "''' + password + '''"
            }'''
        responseJson = self.postToApi(self.pickemServerBaseUrl + "/auth/login", postData, "")

        self.jwt = responseJson['token']

    def readPickemGamesAnyLeague(self, pickemSeasonCode, weekNumber):
        pickemAllGamesUrl = self.pickemServerBaseUrl + "/" + str(pickemSeasonCode) + "/" + str(weekNumber) + "/games_in_any_league"
        return self.getApi(pickemAllGamesUrl, self.jwt)

    def readSystemSettings(self):
        # http://:BaseUrl/api/settings
        url = self.pickemServerBaseUrl + "/settings"
        return self.getApi(url, self.jwt)


    def insertGame(
        self, 
        pickemSeasonCode, 
        weekNumber,
        gameId,
        gameStart,
        neutralField,
        awayTeamCode,
        homeTeamCode
        ):

        pickemGamePostUrl = self.pickemServerBaseUrl + "/" + pickemSeasonCode + "/" + str(weekNumber) + "/games"
        #        {
        #            "gameId": 0, 
        #            "gameStart": "2018-08-03T19:18:08.826Z", responseJson['startDate']  2017-11-18, "startTime": "10:15 PM ET",
        #            "neutralField": true,
        #            "awayTeamCode": "string", -- responseJson['away']['nameSeo']
        #            "homeTeamCode": "string" -- responseJson['home']['nameSeo']
        #        }
        gameData = '{"gameId": "' + gameId + '","gameStart": "' + gameStart + '", "neutralField": "' + neutralField + '", "awayTeamCode": "' + awayTeamCode + '", "homeTeamCode": "' + homeTeamCode + '"}'
        self.postToApi(pickemGamePostUrl, gameData, self.jwt)


    def updateGame(
        self, 
        pickemSeasonCode, 
        weekNumber,
        gameId,
        gameStart,
        lastUpdated,
        gameState,
        currentPeriod,
        timeClock,
        awayTeamScore,
        homeTeamScore
        ):

        # /api/private/{SeasonCode}/{WeekNumber}/games/{GameId}
        pickemGamePutUrl = self.pickemServerBaseUrl + "/" + str(pickemSeasonCode) + "/" + str(weekNumber) + "/games/" + str(gameId)
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
        self.putToApi(pickemGamePutUrl, gameData, self.jwt)

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