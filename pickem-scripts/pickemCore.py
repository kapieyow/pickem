#!/usr/bin/env python3

#=================================================
# These are the core runs of the pickem scripts
# is used by the CLI (pickem.py). May also be
# used directly for one off scripts. 
#=================================================
import configparser
import pickemLogger
import pickemApiClient
import random

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
    userLoggedIn = None

    def __init__(self, subComponentName, overrideUser = None, overridePassword = None):
        settings = Settings()
        
        self.__loadIniConfig(PICKEM_INI, settings)

        self.logger = pickemLogger.Logger(settings.PickemServerBaseUrl, settings.MinLogLevelToApi, settings.MinLogLevelToConsole, subComponentName)
        self.apiClient = pickemApiClient.PickemApiClient(settings.PickemServerBaseUrl, self.logger)

        # default to admin user/pwd from config
        apiUser = settings.PickemAdminUsername
        apiPassword = settings.PickemAdminPassword

        # if an override user was passed in use that instead
        if ( overrideUser != None ):
            apiUser = overrideUser
            apiPassword = overridePassword

        # login
        self.userLoggedIn = self.apiClient.authenticate(apiUser, apiPassword)

    def __loadIniConfig(self, iniFile, settingsContainer):
        configParser = configparser.ConfigParser()
        configParser.read(iniFile)
        settingsContainer.MinLogLevelToApi = configParser.get("LOGGING", "MIN_LOG_LEVEL_TO_API")
        settingsContainer.MinLogLevelToConsole = configParser.get("LOGGING", "MIN_LOG_LEVEL_TO_CONSOLE")
        settingsContainer.PickemAdminUsername = configParser.get("ADMIN", "PICKEM_ADMIN_USERNAME")
        settingsContainer.PickemAdminPassword = configParser.get("ADMIN", "PICKEM_ADMIN_PASSWORD")
        settingsContainer.PickemServerBaseUrl = configParser.get("URLS", "PICKEM_SERVER_BASE_URL")

    def __matchGameByTeamAttribute(self, pickemGames, teamAttributeName, teamAttributeValue):
        # probably smarter way to do this
        for pickemGame in pickemGames:
            # check away team
            if ( pickemGame['awayTeam']['team'][teamAttributeName] == teamAttributeValue ):
                return pickemGame

            # check home team
            if ( pickemGame['homeTeam']['team'][teamAttributeName] == teamAttributeValue ):
                return pickemGame

        # if we got here there is no match
        return None
            
    #=================================================
    # Core helper functions. May have some light logic
    #=================================================
    def extractPickemGames(self, pickemSeasonCode, weekNumber):
        pickemGames = self.apiClient.readPickemGames(pickemSeasonCode, weekNumber)
        for pickemGame in pickemGames:
            self.logger.info("(%d) : (%s) ptc (%s) yc (%s) @ (%s) ptc (%s) yc (%s)" % (pickemGame['gameId'], pickemGame['awayTeam']['team']['longName'], pickemGame['awayTeam']['team']['teamCode'], pickemGame['awayTeam']['team']['yahooCode'], pickemGame['homeTeam']['team']['longName'], pickemGame['homeTeam']['team']['teamCode'], pickemGame['homeTeam']['team']['yahooCode']))

    def pickRandomGamesForCurrentUser(self):

        pickedGameCount = 0
        currentUserName = self.userLoggedIn['userName']

        for league in self.userLoggedIn['leagues']:
            leagueCode = league['leagueCode']
            leagueTitle = league['leagueTitle']
            leagueCurrentWeekRef = league['currentWeekRef']

            self.logger.info("League: %s  Week: %s ----" % ( leagueTitle, leagueCurrentWeekRef ))

            leaguePlayer = self.apiClient.readPlayer(leagueCode, currentUserName)
            playerTagForLeague = leaguePlayer['playerTag']

            playerScoreboardForLeague = self.apiClient.readPlayerWeekScoreboard(leagueCode, leagueCurrentWeekRef, playerTagForLeague)

            for gamePickScoreboard in playerScoreboardForLeague['gamePickScoreboards']:
                gameDescription = "%s @ %s" % ( gamePickScoreboard['awayTeamLongName'], gamePickScoreboard['homeTeamLongName'] )
                gameState = gamePickScoreboard['gameState']
                gameId = gamePickScoreboard['gameId']
                
                if ( gameState == 'SpreadNotSet' or gameState == 'SpreadLocked' ):
                    pickedTeam = None
                    gamePick = random.choice(["Away", "Home"])
                    
                    if ( gamePick == "Away" ):
                        pickedTeam = gamePickScoreboard['awayTeamLongName']
                    else:
                        pickedTeam = gamePickScoreboard['homeTeamLongName']

                    self.apiClient.setPlayerGamePick(leagueCode, leagueCurrentWeekRef, playerTagForLeague, gameId, gamePick)
                    pickedGameCount = pickedGameCount + 1
                    self.logger.info("Picked  %s (%s)  for  %s  game" % ( pickedTeam, gamePick, gameDescription ))

                else:
                    self.logger.info("The  %s  game can not be picked because it is in a %s state" % ( gameDescription, gameState ))

        self.logger.info("Randomly picked (%d) games" % pickedGameCount)

    def setLeagueGame(self, leagueCodes, weekNumber, gameId, pickemTeamCode, yahooTeamCode, gameWinPoints):
        # it's expected that only one of these params (gameId, pickemTeamCode, yahooTeamCode) have a value
        noneParamCount = 0
        noneParamCount += 1 if gameId == None else 0
        noneParamCount += 1 if pickemTeamCode == None else 0
        noneParamCount += 1 if yahooTeamCode == None else 0

        if noneParamCount != 2:
            raise RuntimeError("only one of these params (gameId, pickemTeamCode, yahooTeamCode) should have a value")

        gamesSet = 0

        # if a team code will need to find the game id
        mapTeamCodeToGameId = ( pickemTeamCode != None or yahooTeamCode != None )

        for leagueCode in leagueCodes:

            if ( mapTeamCodeToGameId ):
                league = self.apiClient.readLeague(leagueCode)
                pickemSeasonCode = league['seasonCodeRef']
                pickemGamesForSeason = self.apiClient.readPickemGames(pickemSeasonCode, weekNumber)
                matchedPickemGame = None
                if ( pickemTeamCode != None ):
                    # match on native pickem teamCode
                    matchedPickemGame = self.__matchGameByTeamAttribute(pickemGamesForSeason, "teamCode", pickemTeamCode)

                    if ( matchedPickemGame == None ):
                        self.logger.warn("No game found in season (%s) for league (%s) with teamCode (%s)" % (pickemSeasonCode, leagueCode, pickemTeamCode))
                        continue
                else:
                    # match by yahoo code
                    matchedPickemGame = self.__matchGameByTeamAttribute(pickemGamesForSeason, "yahooCode", yahooTeamCode)

                    if ( matchedPickemGame == None ):
                        self.logger.warn("No game found in season (%s) for league (%s) with yahooCode (%s)" % (pickemSeasonCode, leagueCode, yahooTeamCode))
                        continue
                
                gameId = matchedPickemGame['gameId']

            self.apiClient.setLeagueGame(leagueCode, weekNumber, gameId, gameWinPoints)
            gamesSet = gamesSet + 1

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
