#!/usr/bin/env python3	
import pickemCore	
import pickemAddYahooGames

core = pickemCore.Core("Run once - Setup 2019 NCAAF Bot League")	

currentLeagueCode = "BOTS-NCAAF-19"		
currentLeagueWeek = 8
currentUserName = ""	
currentPlayerTag = ""


# -- Bot League 2019 -----------------	
core.apiClient.insertLeague(currentLeagueWeek, currentLeagueCode, "Pickbots - 2019", "2019", "AllWinsOnePoint", "19", [8,9,10,11,12,13,14,15])	

currentUserName = "kip"	
currentPlayerTag = "sigterm"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)		

currentUserName = "Chris"	
currentPlayerTag = "Chrbot"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)		

currentUserName = "CusheGoblin"	
currentPlayerTag = "tosTeBot"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)	

currentUserName = "Tolowe"
currentPlayerTag = "LoweBot"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)	

currentUserName = "gumanchew"	
currentPlayerTag = "AgBot"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)	

currentUserName = "PickBot"	
currentPlayerTag = "Pickbot 9000"	
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)	

pickemAddYahooGamesHandler = pickemAddYahooGames.PickemAddYahooGamesHandler(core)
pickemAddYahooGamesHandler.Run([currentLeagueCode], currentLeagueWeek)