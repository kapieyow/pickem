#!/usr/bin/env python3
import pickemCore
import pickemSynchGames
import subprocess

core = pickemCore.Core("Run once - Setup 2019 NCAAF Leagues")

synchGamesHandler = pickemSynchGames.PickemSynchGamesHandler(core.apiClient, core.logger)

# add all week 1 through 14 games from ESPN
synchGamesHandler.Run("insert", 2019, 19, 1, False)
synchGamesHandler.Run("insert", 2019, 19, 2, False)
synchGamesHandler.Run("insert", 2019, 19, 3, False)
synchGamesHandler.Run("insert", 2019, 19, 4, False)
synchGamesHandler.Run("insert", 2019, 19, 5, False)
synchGamesHandler.Run("insert", 2019, 19, 6, False)
synchGamesHandler.Run("insert", 2019, 19, 7, False)
synchGamesHandler.Run("insert", 2019, 19, 8, False)
synchGamesHandler.Run("insert", 2019, 19, 9, False)
synchGamesHandler.Run("insert", 2019, 19, 10, False)
synchGamesHandler.Run("insert", 2019, 19, 11, False)
synchGamesHandler.Run("insert", 2019, 19, 12, False)
synchGamesHandler.Run("insert", 2019, 19, 13, False)
synchGamesHandler.Run("insert", 2019, 19, 14, False)

currentLeagueCode = ""
currentUserName = ""
currentPlayerTag = ""

# -- NeOnYa 2019 -----------------
currentLeagueCode = "NeOnYa-NCAAF-19"
core.apiClient.insertLeague(1, currentLeagueCode, "Get any on ya? - 2019", "2019", "AllWinsOnePoint", "19", [1,2,3,4,5,6,7,8,9,10,11,12,13,14,15])

currentUserName = "ali"
currentPlayerTag = "Ali"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "kip"
currentPlayerTag = "Kip"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "poopa"
currentPlayerTag = "PooPa"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)


# -- BUS 2019 ------------------
currentLeagueCode = "BUS-NCAAF-19"
core.apiClient.insertLeague(1, currentLeagueCode, "BUS - 2019", "2019", "AllWinsOnePoint", "19", [1,2,3,4,5,6,7,8,9,10,11,12,13,14,15])

currentUserName = "kip"
currentPlayerTag = "Kip"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)

currentUserName = "Chris"
currentPlayerTag = "Chris"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "GingerNinja"
currentPlayerTag = "Ethan"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "gumanchew"
currentPlayerTag = "Adam"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "DrewWright"
currentPlayerTag = "Drew"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "Dharsan"
currentPlayerTag = "Dharsan"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "Tolowe"
currentPlayerTag = "Tom"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "CusheGoblin"
currentPlayerTag = "Tony"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)



# -- Burl Mafia 2019 ------------------
currentLeagueCode = "BurlMafia-NCAAF-19"
core.apiClient.insertLeague(1, currentLeagueCode, "Burlington Mafia - 2019", "2019", "AllWinsOnePoint", "19", [1,2,3,4,5,6,7,8,9,10,11,12,13,14,15])

currentUserName = "kip"
currentPlayerTag = "Kip"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)

currentUserName = "Cary"
currentPlayerTag = "Cary"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "fvedder"
currentPlayerTag = "Frank"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "msciole"
currentPlayerTag = "Mike"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

currentUserName = "CusheGoblin"
currentPlayerTag = "Tony"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)

currentUserName = "Chris"
currentPlayerTag = "Chris"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)