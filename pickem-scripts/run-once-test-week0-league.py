#!/usr/bin/env python3
import pickemCore
import pickemSynchGames
import pickemUpdateSpreads
import pickemUpdateTeams
import subprocess

core = pickemCore.Core("Run once - Create test week 0 league")

synchGamesHandler = pickemSynchGames.PickemSynchGamesHandler(core.apiClient, core.logger)

currentLeagueCode = ""
currentUserName = ""
currentPlayerTag = ""

# !!!!!!!!!!!!!!!!! -- this part is TEST only cut before prod !!!!!!!!!!!!!!!!!
# -- TEST league for "week 0" -----------------
currentLeagueCode = "TEST-NCAAF-Week-0-19"
core.apiClient.insertLeague(1, currentLeagueCode, "Test of Week 0 - 2019", "2019", "AllWinsOnePoint", "19", [1])

currentUserName = "kip"
currentPlayerTag = "Kip"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)

# add week "0" games are really in week 1 but the early weekend
core.setLeagueGames([currentLeagueCode], 1, [401110723, 401114164])

# ###**AP not posted yet for 2019** pickem.py update_teams -rs ap -psc 19 -w 1
# subprocess.call("pickem.py update_teams -rs ap -psc 19 -w 1", shell=True)
# core.logger.info("Updated teams for week \"0\"")
# updateTeamsHandler = pickemUpdateTeams.PickemUpdateSpreadsHandler(core.apiClient, core.logger)
# updateTeamsHandler.Run(19, 1, "ap")

# update spreads for week one
synchGamesHandler = pickemUpdateSpreads.PickemUpdateSpreadsHandler(core.apiClient, core.logger)
synchGamesHandler.Run("update", 19, 1)

# /!!!!!!!!!!!!!!!!! end TEST garbage !!!!!!!!!!!!!!!!!