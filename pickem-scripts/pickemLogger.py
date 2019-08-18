#!/usr/bin/env python3
 

import json
import requests

PICKEM_LOG_LEVEL_DEBUG = "Debug"
PICKEM_LOG_LEVEL_INFO = "Information"
PICKEM_LOG_LEVEL_WARN = "Warning"
PICKEM_LOG_LEVEL_ERROR = "Error"
PICKEM_LOG_LEVEL_WTF = "WTF"

class Logger:
    def __init__(self, pickemServerBaseUrl, minLogLevelToApi, minLogLevelToConsole, subComponentName):
        self.pickemServerBaseUrl = pickemServerBaseUrl
        self.subComponentName = subComponentName

        # check for invalid log levels on input and if invalid default to Debug
        validLogLevels = [PICKEM_LOG_LEVEL_DEBUG.lower(), PICKEM_LOG_LEVEL_INFO.lower(), PICKEM_LOG_LEVEL_WARN.lower(), PICKEM_LOG_LEVEL_ERROR.lower(), PICKEM_LOG_LEVEL_WTF.lower()]

        if ( validLogLevels.count(minLogLevelToApi.lower()) == 0 ):
            minLogLevelToApi = PICKEM_LOG_LEVEL_DEBUG

        if ( validLogLevels.count(minLogLevelToConsole.lower()) == 0 ):
            minLogLevelToConsole = PICKEM_LOG_LEVEL_DEBUG

        # run logic to check if posting to API for each log level once and set in vars
        # -----------------------------------------------------------------------------

        # these are in order of lowest log level to highest. So if MIN is Debug it will set all to true
        # based on the "stacking" boolean compares here
        self.postDebugToApi = minLogLevelToApi.lower() == PICKEM_LOG_LEVEL_DEBUG.lower()
        self.postInfoToApi = minLogLevelToApi.lower() == PICKEM_LOG_LEVEL_INFO.lower() or self.postDebugToApi
        self.postWarnToApi = minLogLevelToApi.lower() == PICKEM_LOG_LEVEL_WARN.lower() or self.postInfoToApi
        self.postErrorToApi = minLogLevelToApi.lower() == PICKEM_LOG_LEVEL_ERROR.lower() or self.postWarnToApi
        self.postWtfToApi = minLogLevelToApi.lower() == PICKEM_LOG_LEVEL_WTF.lower() or self.postErrorToApi

        # same for console value
        self.postDebugToConsole = minLogLevelToConsole.lower() == PICKEM_LOG_LEVEL_DEBUG.lower()
        self.postInfoToConsole = minLogLevelToConsole.lower() == PICKEM_LOG_LEVEL_INFO.lower() or self.postDebugToConsole
        self.postWarnToConsole = minLogLevelToConsole.lower() == PICKEM_LOG_LEVEL_WARN.lower() or self.postInfoToConsole
        self.postErrorToConsole = minLogLevelToConsole.lower() == PICKEM_LOG_LEVEL_ERROR.lower() or self.postWarnToConsole
        self.postWtfToConsole = minLogLevelToConsole.lower() == PICKEM_LOG_LEVEL_WTF.lower() or self.postErrorToConsole


    def log(self, logLevel, message):
        pickemLogUrl = self.pickemServerBaseUrl + "/logs"
        logData = '{"component": "' + self.subComponentName + '","logMessage": "' + str(message) + '", "logLevel": "' + logLevel + '"}'
        response = requests.post(pickemLogUrl, data=logData, headers={'Content-Type': 'application/json'})

        if(not response.ok):
            response.raise_for_status()

        return

    def debug(self, logMessage):
        if ( self.postDebugToConsole ):
            # note debugs are tabbed in
            print("   " + str(logMessage))

        if ( self.postDebugToApi ):
            self.log(PICKEM_LOG_LEVEL_DEBUG, logMessage)
    
    def info(self, logMessage):
        if ( self.postInfoToConsole ):
            print(logMessage)

        if ( self.postInfoToApi ):
            self.log(PICKEM_LOG_LEVEL_INFO, logMessage)
    
    def warn(self, logMessage):
        if ( self.postWarnToConsole ):
            print("WARN: " + str(logMessage))

        if ( self.postWarnToApi ):
            self.log(PICKEM_LOG_LEVEL_WARN, logMessage)
        
    def error(self, logMessage):
        if ( self.postErrorToConsole ):
            print("ERROR: " + str(logMessage))

        if ( self.postErrorToApi ):
            self.log(PICKEM_LOG_LEVEL_ERROR, logMessage)
        
    def wtf(self, logMessage):
        if ( self.postWtfToConsole ):
            print("WTF: " + str(logMessage))

        if ( self.postWtfToApi ):
            self.log(PICKEM_LOG_LEVEL_WTF, logMessage)
