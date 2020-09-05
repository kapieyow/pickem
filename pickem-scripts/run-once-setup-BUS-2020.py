#!/usr/bin/env python3	
import pickemCore	
import pickemAddYahooGames
import pickemSynchGames

core = pickemCore.Core("Setup of 2020 BUS CFB season")	

currentLeagueCode = "BUS-NCAAF-20"		
currentLeagueWeek = 1
currentUserName = ""	
currentPlayerTag = ""


# -- TEST 2020 NeOnYa league -----------------	
core.apiClient.insertLeague(currentLeagueWeek, currentLeagueCode, "BUS - 2020", "2020", "AllWinsOnePoint", "20", [1,2,3,4,5,6,7,8,9,10,11,12,13,14])	

currentUserName = "kip"	
currentPlayerTag = "Kip"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)		
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "Levolden"	
currentPlayerTag = "Laine"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)		
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "GingerNinja"	
currentPlayerTag = "Ethan"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)		
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "CusheGoblin"	
currentPlayerTag = "Tony"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)		
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "PickBot"	
currentPlayerTag = "Pickbot 9000"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)	

gameSyncher = pickemSynchGames.PickemSynchGamesHandler(core.apiClient, core.logger)

# add week 1 pickem games
core.setLeagueGame([currentLeagueCode], currentLeagueWeek, None, "army", None, 1)
core.setLeagueGame([currentLeagueCode], currentLeagueWeek, None, "marshall", None, 1)
core.setLeagueGame([currentLeagueCode], currentLeagueWeek, None, "memphis", None, 1)
core.setLeagueGame([currentLeagueCode], currentLeagueWeek, None, "navy", None, 1)
core.setLeagueGame([currentLeagueCode], currentLeagueWeek, None, "north-texas", None, 1)
core.setLeagueGame([currentLeagueCode], currentLeagueWeek, None, "southern-miss", None, 1)
core.setLeagueGame([currentLeagueCode], currentLeagueWeek, None, "texas-st", None, 1)
core.setLeagueGame([currentLeagueCode], currentLeagueWeek, None, "uab", None, 1)
core.setLeagueGame([currentLeagueCode], currentLeagueWeek, None, "utep", None, 1)