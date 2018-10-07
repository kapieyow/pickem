#!/usr/bin/env python3

import datetime
import json
import requests
from bs4 import BeautifulSoup

TEAM_FBS_AP_RANKINGS_URL = "https://www.ncaa.com/rankings/football/fbs/associated-press"
TEAM_FBS_STANDINGS_URL = "https://www.ncaa.com/standings/football/fbs"
TEAM_FCS_STANDINGS_URL = "https://www.ncaa.com/standings/football/fcs"

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

        ncaaTeamStats = dict()

        self.__loadFbsTeamWinLose(ncaaTeamStats, TEAM_FBS_STANDINGS_URL)
        self.__loadNcaaFbsRankings(ncaaTeamStats, pickemTeams, TEAM_FBS_AP_RANKINGS_URL)
        self.__updatePickemWithStats(pickemSeason, weekNumber, ncaaTeamStats, pickemTeams)

    def __loadFbsTeamWinLose(self, ncaaTeamStats, url):
        fbsHtml = self.apiClient.getHtml(url)
        soup = BeautifulSoup(fbsHtml, "html.parser")

        # html parse and build 
        fbsTeamCount = 0 

        for teamTd in soup.find_all("td", class_="standings-team"):
        
            teamImg = teamTd.img
            teamImgSrc = teamImg['src']
            # get team code name, which is part of the img src
            # e.g. https://i.turner.ncaa.com/sites/default/files/images/logos/schools/g/ga-southern.24.png
            # == ga-southern
            imgUrlParts = teamImgSrc.split("/")
            imgFileName = imgUrlParts[len(imgUrlParts) - 1]
            imgFileNameParts = imgFileName.split(".")
            teamCode = imgFileNameParts[0]

            # 3 columns (tds) over is the Wins
            winsTd = teamTd.find_next("td").find_next("td").find_next("td")

            thisTeam = TeamUpdates()
            thisTeam.wins = winsTd.text

            # losses beside wins
            lossesTd = winsTd.find_next("td")

            thisTeam.losses = lossesTd.text

            # add to dictionary
            ncaaTeamStats[teamCode] = thisTeam

            fbsTeamCount = fbsTeamCount + 1

        self.logger.debug("Read (" + str(fbsTeamCount) + ") FBS teams from NCAA")

    def __findPickemTeamByLongName(self, pickemTeams, teamLongName):
        # prolly a betta way
        for pickemTeam in pickemTeams:
            if ( pickemTeam['longName'] == teamLongName ):
                return pickemTeam

        # not found
        return None

    def __findPickemTeamByTeamCode(self, pickemTeams, teamCode):
        # prolly a betta way
        for pickemTeam in pickemTeams:
            if ( pickemTeam['teamCode'] == teamCode ):
                return pickemTeam

        # not found
        return None


    def __loadNcaaFbsRankings(self, ncaaTeamStats, pickemTeams, url):
        fbsHtml = self.apiClient.getHtml(url)
        soup = BeautifulSoup(fbsHtml, "html.parser")

        containTable = soup.find("table", class_="sticky")
        
        totalRankingRead = 0
        rankingsMatched = 0

        for tableRow in containTable.find("tbody").find_all("tr"):

            rankTd = tableRow.find_next("td")
            rank = rankTd.text

            teamTd = rankTd.find_next("td")
            teamName = teamTd.text
            # some teams have the vote count at the end like Clemson (99)
            # this dumps the parens
            teamName = teamName.split(" (")[0]

            pickemTeam = self.__findPickemTeamByLongName(pickemTeams, teamName)

            if ( pickemTeam == None ):
                self.logger.warn("No pickem team found for NCAA team long name: " + teamName)
            else:
                rankingsMatched = rankingsMatched + 1
                matchedTeamStats = ncaaTeamStats[pickemTeam['teamCode']]
                matchedTeamStats.fbsRank = rank

            totalRankingRead = totalRankingRead + 1

        self.logger.debug("Read (" + str(totalRankingRead) + ") FBS rankings from NCAA. Matched (" + str(rankingsMatched) + ")")


    def __updatePickemWithStats(self, seasonNumber, weekNumber, ncaaTeamStats, pickemTeams):

        updatedTeamCount = 0

        for teamCode, teamStats in ncaaTeamStats.items():
            pickemTeam = self.__findPickemTeamByTeamCode(pickemTeams, teamCode)

            if ( pickemTeam == None ):
                self.logger.warn("No pickem team found for NCAA team code: " + teamCode)
            else:
                self.apiClient.updateTeam(teamCode, seasonNumber, weekNumber, teamStats.wins, teamStats.losses, teamStats.fbsRank)
                updatedTeamCount = updatedTeamCount + 1

        self.logger.warn("Updated (" + str(updatedTeamCount) + ") pickem team's stats")