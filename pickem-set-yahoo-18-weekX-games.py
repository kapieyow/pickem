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
leagueGameUrl = PICKEM_SERVER_BASE_URL + "/18/Default/2/games"

postToApi(leagueGameUrl, "{ 'gameId': 2953834 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954006 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953777 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954434 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954112 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953741 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954288 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954259 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954143 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953778 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954335 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953517 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953679 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953836 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954376 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954296 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953475 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954155 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954162 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953827 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954276 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953056 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954405 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953915 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953619 }", jwt)
