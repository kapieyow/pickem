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

if ( args.week == 3 ):
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

elif ( args.week == 4 ):
    postToApi(leagueGameUrl, "{ 'gameId': 2954204 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954362 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953851 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953549 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953959 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954297 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954359 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953210 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953717 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953429 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954018 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953898 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953961 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953561 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954147 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953259 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953041 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954050 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954222 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954415 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954482 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954450 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953060 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953956 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953116 }", jwt)

elif ( args.week == 5 ):
    postToApi(leagueGameUrl, "{ 'gameId': 2954381 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954330 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953478 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954187 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953018 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953605 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953728 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954392 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953957 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953983 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954483 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953875 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954152 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953929 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953081 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953887 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953348 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953303 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953563 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953036 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954052 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953193 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953913 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953550 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953068 }", jwt)

elif ( args.week == 6 ):
    postToApi(leagueGameUrl, "{ 'gameId': 2954286 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953609 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953107 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953299 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953817 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953859 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954049 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953004 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953986 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953847 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954087 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953382 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954145 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953729 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954416 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954091 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953771 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953922 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953219 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954142 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953311 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954225 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954044 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2953232 }", jwt)
    postToApi(leagueGameUrl, "{ 'gameId': 2954268 }", jwt)
