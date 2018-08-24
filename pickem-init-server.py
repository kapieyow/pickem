#!/usr/bin/env python3

# ====================================================
#  Initializes server environment. ASSuMEs empty db.
# ====================================================

import argparse
import json
import requests
import subprocess

# "configs"
VERSION = "0.3.0"

PICKEM_COMPONENT_NAME = "Pick'Em Server Initializer"
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

print("----------------------------------------")
print("  {0:s} - {1:s} ".format(PICKEM_COMPONENT_NAME, VERSION))
print("----------------------------------------")

print("Pick'em Server: " + PICKEM_SERVER_BASE_URL)

print("init-db job run")
postData = '{ "jobTag": "init-db" }'
postToApi(PICKEM_SERVER_BASE_URL + "/private/jobs", postData, "")


print("Adding 'root' user")
postData = '''{
        "email": "kip.porterfield@gmail.com",
        "password": "iamroot",
        "userName": "root",
        "doNotSetDefaultLeague": true
    }'''
postToApi(PICKEM_SERVER_BASE_URL + "/useraccounts", postData, "")


print("Authenticating as 'root'")
postData = '''{
        "userName": "root",
        "password": "iamroot"
    }'''
responseJson = postToApi(PICKEM_SERVER_BASE_URL + "/auth/login", postData, "")

jwt = responseJson['token']


print("Creating Default league'")
postData = '''{
    "leagueCode": "Default",
    "leagueTitle": "Default League for 2018",
    "weekNumbers": [ 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15 ]
}'''
responseJson = postToApi(PICKEM_SERVER_BASE_URL + "/18/leagues", postData, jwt)
