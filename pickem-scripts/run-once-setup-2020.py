#!/usr/bin/env python3	
import pickemCore	
import pickemAddYahooGames
import pickemSynchGames

core = pickemCore.Core("Setup of 2020 CFB season")	

currentLeagueCode = "NeOnYa-NCAAF-20"		
currentLeagueWeek = 1
currentUserName = ""	
currentPlayerTag = ""


# -- TEST 2020 NeOnYa league -----------------	
core.apiClient.insertLeague(currentLeagueWeek, currentLeagueCode, "Get any on ya? - 2020", "2020", "AllWinsOnePoint", "20", [1,2,3,4,5,6,7,8,9,10,11,12,13,14])	

currentUserName = "kip"	
currentPlayerTag = "Virgil"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)		
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "poopa"	
currentPlayerTag = "Poopa"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)		
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "Brodie"	
currentPlayerTag = "Brodie"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)		
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "ali"	
currentPlayerTag = "Ali"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)		
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "PickBot"	
currentPlayerTag = "Pickbot 9000"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)	

gameSyncher = pickemSynchGames.PickemSynchGamesHandler(core.apiClient, core.logger)

# week 1 *inserts*
gameSyncher.Run("insert", 2020, 20, currentLeagueWeek, False)

## add known week 1 games
#core.setLeagueGames([currentLeagueCode], currentLeagueWeek, [401207101, 401207098, 401235700, 401212484, 401234576])

