#!/usr/bin/env python3

import json
import requests

PICKEM_SERVER_BASE_URL = "http://localhost:51890/api"

def postToApi(url, postData, jwt):
    if ( jwt == "" ):
        response = requests.post(url, data=postData, headers={'Content-Type': 'application/json'})
    else:
        response = requests.post(url, data=postData, headers={'Content-Type': 'application/json', 'authorization': "Bearer " + jwt})

    if(not response.ok):
        response.raise_for_status()
    else:
        print(response)
        print(response.json())

    return response.json()


print("Authenticating as 'root'")
postData = '''{
        "userName": "root",
        "password": "iamroot"
    }'''
responseJson = postToApi(PICKEM_SERVER_BASE_URL + "/auth/login", postData, "")

jwt = responseJson['token']


print("Setting all games")
leagueGameUrl = PICKEM_SERVER_BASE_URL + "/18/Default/1/games"

postToApi(leagueGameUrl, '{ "gameId": 2953222 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953158 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953962 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2954427 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953342 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953003 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953304 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953776 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2954051 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953428 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2954452 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2954171 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953515 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2954387 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953218 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953808 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953974 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2954153 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953375 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953037 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953473 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953366 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2954241 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2953258 }', jwt)
postToApi(leagueGameUrl, '{ "gameId": 2954332 }', jwt)