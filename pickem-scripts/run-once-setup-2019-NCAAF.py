#!/usr/bin/env python3

import pickemCore

core = pickemCore.Core("Run once - Setup 2019 NCAAF Leagues")

# Create league NeOnYa-NCAAF-19
postData = '''
    {
        "currentWeekRef": 1,
        "leagueCode": "NeOnYa-NCAAF-19",
        "leagueTitle": "Get any on ya? - 2019",
        "ncaaSeasonCodeRef": "2019",
        "pickemScoringType": "AllWinsOnePoint",
        "seasonCodeRef": "19",
        "weekNumbers": [
            1,2,3,4,5,6,7,8,9,10,11,12,13,14
        ]
    }
'''
core.apiClient.postToPickemApi("/leagues", postData)
core.logger.info("Added NeOnYa-NCAAF-19 league")

# Add user Kip to NeOnYa-NCAAF-19
core.apiClient.postToPickemApi("/NeOnYa-NCAAF-19/players", '{ "playerTag": "kip", "userName": "kip" }')
core.logger.info("Added Kip to NeOnYa-NCAAF-19")

# Set Default league for Kip
core.apiClient.putToPickemApi("/useraccounts/kip", '{ "defaultLeagueCode": "NeOnYa-NCAAF-19" }')
core.logger.info("Set default to NeOnYa-NCAAF-19 for Kip")