#!/usr/bin/env python3

#=================================================
# These are the core runs of the pickem scripts
# is used by the CLI (pickem.py). May also be
# used directly for one off scripts. 
#=================================================
import configparser
import pickemLogger
import pickemApiClient

# "configs"
PICKEM_INI = "pickem-settings.ini"

class Settings:
    # from INI
    MinLogLevelToApi = ""
    MinLogLevelToConsole = ""
    PickemAdminUsername = ""
    PickemAdminPassword = ""
    PickemServerBaseUrl = ""

class Core:

    apiClient = None
    logger = None

    def __init__(self, subComponentName):
        settings = Settings()
        
        self.__loadIniConfig(PICKEM_INI, settings)

        self.logger = pickemLogger.Logger(settings.PickemServerBaseUrl, settings.MinLogLevelToApi, settings.MinLogLevelToConsole, subComponentName)
        self.apiClient = pickemApiClient.PickemApiClient(settings.PickemServerBaseUrl, self.logger)

        # login
        self.apiClient.authenticate(settings.PickemAdminUsername, settings.PickemAdminPassword)

    def __loadIniConfig(self, iniFile, settingsContainer):
        configParser = configparser.ConfigParser()
        configParser.read(iniFile)
        settingsContainer.MinLogLevelToApi = configParser.get("LOGGING", "MIN_LOG_LEVEL_TO_API")
        settingsContainer.MinLogLevelToConsole = configParser.get("LOGGING", "MIN_LOG_LEVEL_TO_CONSOLE")
        settingsContainer.PickemAdminUsername = configParser.get("ADMIN", "PICKEM_ADMIN_USERNAME")
        settingsContainer.PickemAdminPassword = configParser.get("ADMIN", "PICKEM_ADMIN_PASSWORD")
        settingsContainer.PickemServerBaseUrl = configParser.get("URLS", "PICKEM_SERVER_BASE_URL")

    #=================================================
    # Core helper functions. May have some light logic
    #=================================================
    def setLeagueGame(self, leagueCodes, weekNumber, gameId, gameWinPoints):
        gamesSet = 0

        for leagueCode in leagueCodes:
            self.apiClient.setLeagueGame(leagueCode, weekNumber, gameId, gameWinPoints)
            gamesSet = gamesSet + 1

            self.logger.info("Set (" + str(gamesSet) + ") games for league (" + leagueCode + ") in week (" + str(weekNumber) + ")")

    def setLeagueGames(self, leagueCodes, weekNumber, gameIds):
        gamesSet = 0

        for leagueCode in leagueCodes:
            for gameId in gameIds:
                # no win points can be sent in, when setting multiple games at a time, so default to "1"
                self.apiClient.setLeagueGame(leagueCode, weekNumber, gameId, 1)
                gamesSet = gamesSet + 1

            self.logger.info("Set (" + str(gamesSet) + ") games for league (" + leagueCode + ") in week (" + str(weekNumber) + ")")
            gamesSet = 0

    def setupLeagueWeek(self, leagueCodes, weekNumber):
        for leagueCode in leagueCodes:
            self.apiClient.updateLeague(leagueCode, weekNumber)
            self.logger.info("Set week to (" + str(weekNumber) + ") for league (" + leagueCode + ")")
