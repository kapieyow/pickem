#!/usr/bin/env python3
import json
import requests

PickemUserName = "SET_YOUR_USER_NAME"
PickemPassword = "SET_YOUR_PASSWORD"
PickemBotLeagueCode = "SET_LEAGUE_CODE"
PickemBaseUrl = "SET_BASE_PICKEM_URL"

def getFromApi(url, jwt):    
    response = requests.get(url, headers={'Content-Type': 'application/json', 'authorization': "Bearer " + jwt})
    if(not response.ok):
        print("HTTP GET api Failure. Code: " + str(response.status_code) + " URL: " + url)
        response.raise_for_status()
    return response.json()

def postToApi(url, postData):
    response = requests.post(url, data=postData, headers={'Content-Type': 'application/json'})
    if(not response.ok):
        print("HTTP POST Failure. Code: " + str(response.status_code) + " URL: " + url)
        response.raise_for_status()
    return response.json()
    
def putToApi(url, putData, jwt):
    response = requests.put(url, data=putData, headers={'Content-Type': 'application/json', 'authorization': "Bearer " + jwt})
    if(not response.ok):
        print("HTTP PUT Failure. Code: " + str(response.status_code) + " URL: " + url)
        response.raise_for_status()
    return response.json()



# authenticate
userCredentials = "{ 'userName': '" + PickemUserName + "', 'password': '" + PickemPassword + "' }'"
userLoggedIn = postToApi(PickemBaseUrl + "/auth/login", userCredentials)

jwt = userLoggedIn['token']

# get player tag for this user in the league
player = getFromApi(PickemBaseUrl + "/" + PickemBotLeagueCode + "/players/" + PickemUserName, jwt)

# get pickem games for current week
botLeague = [league for league in userLoggedIn['leagues'] if league['leagueCode'] == PickemBotLeagueCode][0] 
playerScoreboard = getFromApi(
    PickemBaseUrl + "/" + PickemBotLeagueCode + "/" + str(botLeague['currentWeekRef']) + "/" + player['playerTag'] + "/scoreboard",
    jwt)

for gamePickScoreboard in playerScoreboard['gamePickScoreboards']:
    pick = "Away"

    putToApi(
        PickemBaseUrl + "/" + PickemBotLeagueCode + "/" + str(botLeague['currentWeekRef']) + "/" + player['playerTag'] + "/scoreboard/" + str(gamePickScoreboard['gameId']) + "/pick", 
        "{ 'pick': '" + pick + "'}",
        jwt)



