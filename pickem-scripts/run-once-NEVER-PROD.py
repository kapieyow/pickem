#!/usr/bin/env python3

#=================================================
# These are dev hacks
#=================================================

import pickemCore

core = pickemCore.Core("Run once - TEMP HACKS")

core.apiClient.insertLeague(1, "League-A", "League-A", "2019", "AllWinsOnePoint", "19", [1])
core.apiClient.insertLeague(1, "League-B", "League-B", "2019", "AllWinsOnePoint", "19", [1])
core.apiClient.insertLeague(1, "League-C", "League-C", "2019", "AllWinsOnePoint", "19", [1])
core.apiClient.insertLeague(1, "League-D", "League-D", "2019", "AllWinsOnePoint", "19", [1])
core.apiClient.insertLeague(1, "League-E", "League-E", "2019", "AllWinsOnePoint", "19", [1])

core.apiClient.addPlayerToLeague("League-A", "kip", "kip")
core.apiClient.addPlayerToLeague("League-B", "kip", "kip")
core.apiClient.addPlayerToLeague("League-C", "kip", "kip")
core.apiClient.addPlayerToLeague("League-D", "kip", "kip")
core.apiClient.addPlayerToLeague("League-E", "kip", "kip")