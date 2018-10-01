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

    def readSystemSettings(self):
        # http://:BaseUrl/api/settings
        url = self.pickemServerBaseUrl + "/settings"
        return self.getApi(url, self.jwt)

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
            response.raise_for_status()
        else:
            self.logger.debug(response)

        return response.json()

    def getHtml(self, url):
        self.logger.debug("GET: " + url)
        response = requests.get(url)

        if(not response.ok):
            response.raise_for_status()
        else:
            self.logger.debug(response)

        return response.text