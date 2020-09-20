#!/usr/bin/env python3	
import pickemCore	
import pickemAddYahooGames
import pickemSynchGames

core = pickemCore.Core("Setup of 2020 Burlington Mafia CFB season")	

currentLeagueCode = "BurlMafia-NCAAF-20"		
currentLeagueWeek = 4
currentUserName = ""	
currentPlayerTag = ""


# -- TEST 2020 NeOnYa league -----------------	
core.apiClient.insertLeague(currentLeagueWeek, currentLeagueCode, "Burlington Mafia - 2020", "2020", "AllWinsOnePoint", "20", [4,5,6,7,8,9,10,11,12,13,14])	

currentUserName = "kip"	
currentPlayerTag = "Kip"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)		
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "CusheGoblin"	
currentPlayerTag = "Tony"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)		
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "PickBot"	
currentPlayerTag = "Pickbot 9000"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)	

currentUserName = "Cary"	
currentPlayerTag = "Cary"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)	

currentUserName = "fvedder"	
currentPlayerTag = "Frank"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)	

currentUserName = "tobyschau2@yahoo.com"	
currentPlayerTag = "Toby"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)	