#!/usr/bin/env python3

import json
import requests

PICKEM_LOG_LEVEL_DEBUG = "Debug"
PICKEM_LOG_LEVEL_INFO = "Information"
PICKEM_LOG_LEVEL_WARN = "Warning"
PICKEM_LOG_LEVEL_ERROR = "Error"
PICKEM_LOG_LEVEL_WTF = "WTF"

class Logger:
    def __init__(self, pickemServerBaseUrl, subComponentName):
        self.pickemServerBaseUrl = pickemServerBaseUrl
        self.subComponentName = subComponentName

    def log(self, logLevel, message):
        print(logLevel + ": " + message)

        pickemLogUrl = self.pickemServerBaseUrl + "/logs"
        logData = '{"component": "' + self.subComponentName + '","logMessage": "' + message + '", "logLevel": "' + logLevel + '"}'
        response = requests.post(pickemLogUrl, data=logData, headers={'Content-Type': 'application/json'})

        if(not response.ok):
            response.raise_for_status()

        return

    def debug(self, logMessage):
        # TODO config this.
        # self.log(PICKEM_LOG_LEVEL_DEBUG, logMessage)
        print(logMessage)
    
    def info(self, logMessage):
        self.log(PICKEM_LOG_LEVEL_INFO, logMessage)
    
    def warn(self, logMessage):
        self.log(PICKEM_LOG_LEVEL_WARN, logMessage)
        
    def error(self, logMessage):
        self.log(PICKEM_LOG_LEVEL_ERROR, logMessage)
        
    def wtf(self, logMessage):
        self.log(PICKEM_LOG_LEVEL_WTF, logMessage)
