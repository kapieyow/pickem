#!/usr/bin/env python3

#=================================================
# These are the core runs of the pickem scripts
# is used by the CLI (pickem.py). May also be
# used directly for one off scripts
#=================================================
import configparser
import pickemLogger
import pickemApiClient

# "configs"
PICKEM_INI = "pickem-settings.ini"

class Settings:
    # from INI
    MinLogLevelToApi = ""
    PickemAdminUsername = ""
    PickemAdminPassword = ""
    PickemServerBaseUrl = ""

class Core:

    apiClient = None
    logger = None

    def __init__(self, subComponentName):
        settings = Settings()
        
        self.__loadIniConfig(PICKEM_INI, settings)

        self.logger = pickemLogger.Logger(settings.PickemServerBaseUrl, settings.MinLogLevelToApi, subComponentName)
        self.apiClient = pickemApiClient.PickemApiClient(settings.PickemServerBaseUrl, self.logger)

        # login
        self.apiClient.authenticate(settings.PickemAdminUsername, settings.PickemAdminPassword)

    def __loadIniConfig(self, iniFile, settingsContainer):
        configParser = configparser.ConfigParser()
        configParser.read(iniFile)
        settingsContainer.MinLogLevelToApi = configParser.get("LOGGING", "MIN_LOG_LEVEL_TO_API")
        settingsContainer.PickemAdminUsername = configParser.get("ADMIN", "PICKEM_ADMIN_USERNAME")
        settingsContainer.PickemAdminPassword = configParser.get("ADMIN", "PICKEM_ADMIN_PASSWORD")
        settingsContainer.PickemServerBaseUrl = configParser.get("URLS", "PICKEM_SERVER_BASE_URL")
