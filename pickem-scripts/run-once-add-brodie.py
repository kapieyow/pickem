#!/usr/bin/env python3
import pickemCore
import subprocess

core = pickemCore.Core("Run once - Add Brodie to NeOnYa - 2019")

# -- NeOnYa 2019 -----------------
currentLeagueCode = "NeOnYa-NCAAF-19"

currentUserName = "Brodie"
currentPlayerTag = "Brodie"
core.apiClient.addPlayerToLeague(currentLeagueCode, currentUserName, currentPlayerTag)
core.apiClient.updateUserAccount(currentUserName, currentLeagueCode)
