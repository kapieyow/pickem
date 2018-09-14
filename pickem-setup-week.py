#!/usr/bin/env python3

import argparse
import configparser
import json
import requests

PICKEM_INI = "pickem-settings.ini"

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

def putToApi(url, postData, jwt):
    if ( jwt == "" ):
        response = requests.put(url, data=postData, headers={'Content-Type': 'application/json'})
    else:
        response = requests.put(url, data=postData, headers={'Content-Type': 'application/json', 'authorization': "Bearer " + jwt})

    if(not response.ok):
        response.raise_for_status()
    else:
        print(response)
        print(response.json())

    return response.json()

# +++++++
#  Main
# +++++++

# Command Line Interface
# - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
parser = argparse.ArgumentParser(description='Setup week. Set games and current week')
parser.add_argument('-l', '--league', required=True, help='League code')
parser.add_argument('-w', '--week', type=int, required=True, help='Week in # e.g. 7')
args = parser.parse_args()


print("----------------------------------------")
print("  Pick'em week setup")
print("----------------------------------------")

configParser = configparser.ConfigParser()
configParser.read(PICKEM_INI)
PICKEM_SERVER_BASE_URL = configParser.get("URLS", "PICKEM_SERVER_BASE_URL")

print("Authenticating as 'root'")
postData = '''{
        "userName": "root",
        "password": "iamroot"
    }'''
responseJson = postToApi(PICKEM_SERVER_BASE_URL + "/auth/login", postData, "")

jwt = responseJson['token']


print("Setting current week to " + str(args.week) + " for league " + args.league)
leagueCurrentWeekUrl = PICKEM_SERVER_BASE_URL + "/18/" + args.league + "/weeks/current"

putToApi(leagueCurrentWeekUrl, str(args.week), jwt)


print("Setting all games")
leagueGameUrl = PICKEM_SERVER_BASE_URL + "/18/" + args.league + "/" + str(args.week) + "/games"

postToApi(leagueGameUrl, "{ 'gameId': 2954298 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953483 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953573 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954386 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953456 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953638 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953854 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953096 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953384 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953234 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953418 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954251 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954324 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953251 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953274 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953661 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953542 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953781 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953380 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954315 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2954174 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953079 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953812 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953283 }", jwt)
postToApi(leagueGameUrl, "{ 'gameId': 2953379 }", jwt)
