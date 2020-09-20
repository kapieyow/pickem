#!/usr/bin/env python3
 
import datetime
import json
import re
import requests
from bs4 import BeautifulSoup

TEAM_FBS_CFP_RANKINGS_URL = "THIS_AINT_RIGHT"
TEAM_FBS_STANDINGS_URL = "https://www.espn.com/college-football/standings"

class Jsonable:
    def toJSON(self):
        return json.dumps(self, default=lambda o: o.__dict__)

class TeamUpdates(Jsonable):
    wins = 0
    losses = 0
    fbsRank = 0

class PickemUpdateSpreadsHandler:
    def __init__(self, apiClient, logger):
        self.apiClient = apiClient
        self.logger = logger

    def Run(self, pickemSeason, weekNumber):

        pickemTeams = self.apiClient.readPickemTeams()

        espnTeamStats = dict()

        self.__loadFbsTeamWinLose(espnTeamStats, TEAM_FBS_STANDINGS_URL)
        self.__updatePickemWithStats(pickemSeason, weekNumber, espnTeamStats, pickemTeams)


    def __loadFbsTeamWinLose(self, espnTeamStats, url):
        fbsHtml = self.apiClient.getHtml(url)
        soup = BeautifulSoup(fbsHtml, "html.parser")

        fbsTeamCount = 0 

        # The basic deal is for each team there are two HTML tables side by side
        # loop on the first to get teams, the synch the w/l based on the same row index
        # the map is key to row id with team as value
        teamMap = dict()

        # get team names and rank if any
        trIndex = 0
        for divTeam in soup.find_all("div", class_="team-link"):

            teamStats = TeamUpdates()

            # check for ranking
            spanRank = divTeam.select_one("span.pr2")
            if ( spanRank != None ):
                teamStats.fbsRank = spanRank.get_text()
                
            aTeam = divTeam.find_next("span", class_="hide-mobile").find_next("a")
            espnTeamName = aTeam.get_text()

            # add to map of row id to team name
            teamMap[trIndex] = espnTeamName

            # add to overall dict of teamStats keyed by espn name
            espnTeamStats[espnTeamName] = teamStats

            self.logger.debug("Added team (%s) @ [%d]" % (teamMap[trIndex], trIndex ))

            trIndex = trIndex + 1

        # loop on second table and get w/l
        trIndex = 0

        for divSecondTableContainer in soup.find_all("div", class_="Table__Scroller"):
            tbodySecondTable = divSecondTableContainer.find_next("tbody", class_="Table__TBODY")
            for trStatsRow in tbodySecondTable.find_all("tr", class_="Table__TR"):

                # want all the Table__TR rows that DO NOT have the subgroup-headers class also
                # I bet there is a fancy way to do this with BS4 select or something above
                if ( "subgroup-headers" in trStatsRow['class'] ):
                    continue

                # win / lose is in the 4th column/cell
                statSpansForRow = trStatsRow.find_all("span", class_="stat-cell")
                winLossText = statSpansForRow[3].get_text()
                # cell has win and loss like "win-loss" e.g. "4-3"
                winAndLoss = winLossText.split("-")

                # update stats with win/loss, note the team map is used to get team name by row id
                teamStats = espnTeamStats[teamMap[trIndex]]
                teamStats.wins = winAndLoss[0]
                teamStats.losses = winAndLoss[1]

                self.logger.debug("Set team (%s) stats as W/L (%s-%s) Rank (%s) @ [%d]" % (teamMap[trIndex], teamStats.wins, teamStats.losses, teamStats.fbsRank, trIndex ))

                trIndex = trIndex + 1
                fbsTeamCount = fbsTeamCount + 1

        self.logger.info("Read (" + str(fbsTeamCount) + ") FBS teams from NCAA")


    def __findPickemTeamByEspnDisplayName(self, pickemTeams, espnDisplayName):
        # prolly a betta way
        for pickemTeam in pickemTeams:
            if ( pickemTeam['espnDisplayName'] == espnDisplayName ):
                return pickemTeam

        # not found
        return None


    def __updatePickemWithStats(self, seasonNumber, weekNumber, espnTeamStats, pickemTeams):

        updatedTeamCount = 0

        for espnDisplayName, teamStats in espnTeamStats.items():
            pickemTeam = self.__findPickemTeamByEspnDisplayName(pickemTeams, espnDisplayName)

            if ( pickemTeam == None ):
                self.logger.warn("No pickem team found for ESPN Display Name: " + espnDisplayName)
            else:
                self.apiClient.updateTeam(pickemTeam['teamCode'], seasonNumber, weekNumber, teamStats.wins, teamStats.losses, teamStats.fbsRank)
                updatedTeamCount = updatedTeamCount + 1

        self.logger.info("Updated (" + str(updatedTeamCount) + ") pickem team's stats")