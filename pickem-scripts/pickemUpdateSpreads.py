#!/usr/bin/env python3
 
import datetime
import json
import pickemLogger
import pickemApiClient
import requests
import time
from bs4 import BeautifulSoup

SPREAD_SITE_URL = "https://www.thespread.com/ncaa-college-football-public-betting-chart"

class Jsonable:
	def toJSON(self):
        	return json.dumps(self, default=lambda o: o.__dict__)

class PickemUpdateSpreadsHandler:
    def __init__(self, apiClient, logger):
        self.apiClient = apiClient
        self.logger = logger

    def Run(self, actionCode, pickemSeason, weekNumber):
        # read pickem games used (so not all NCAA games)
        pickemGamesForWeek = self.apiClient.readPickemGamesAnyLeague(pickemSeason, weekNumber)

        if ( actionCode == "update" or actionCode == "u" ):
            self.__updateSpreads(pickemGamesForWeek, pickemSeason, weekNumber)

        elif ( actionCode == "lock" or actionCode == "l" ):
            self.__lockSpreads(pickemGamesForWeek, pickemSeason, weekNumber)

        else:
            self.logger.wtf("Unhandled input why didn't argparser catch it? --action " + actionCode)

    def __lockSpreads(self, pickemGamesForWeek, pickemSeason, weekNumber):
        for pickemGame in pickemGamesForWeek:
            gameId = pickemGame['gameId']
            self.apiClient.lockSpread(pickemSeason, weekNumber, gameId)

        self.logger.info("Locked {0} game spreads.".format(len(pickemGamesForWeek)))	

    def __updateSpreads(self, pickemGamesForWeek, pickemSeason, weekNumber):

        webHtml = self.apiClient.getHtml(SPREAD_SITE_URL)

        soup = BeautifulSoup(webHtml, "html.parser")

        contentTable = soup.find("div", id="pb")
        containerDiv = contentTable.find("div", recursive=False)

        spreadsGameCount = 0
        updatedPickemGameCount = 0

        # Loop on "rows" one for each game
        for divRow in containerDiv.find_all("div", class_="datarow"):
            nextGameSpread = Jsonable()
            neutralFieldGame = False

            # date and time in first child div
            thisDataDiv = divRow.div
            textsInDiv = thisDataDiv.get_text("|", strip=True).split("|")

            nextGameSpread.Date = textsInDiv[1]
            nextGameSpread.Time = textsInDiv[2]

            # teams in next div
            thisDataDiv = thisDataDiv.next_sibling

            nextGameSpread.VisitorTeam = thisDataDiv.find_next("span", id="tmv").string
            nextGameSpread.HomeTeam = thisDataDiv.find_next("span", id="tmh").string

            # HACK-O on *nix we get weird text for "Texas A&M" comes through "Texas A&M;" with the semicolon
            # TODO fix this. Better call in BeautifulSoup?
            if (nextGameSpread.VisitorTeam[-1:] == ";"):
                nextGameSpread.VisitorTeam = nextGameSpread.VisitorTeam[:-1]

            if (nextGameSpread.HomeTeam[-1:] == ";"):
                nextGameSpread.HomeTeam = nextGameSpread.HomeTeam[:-1]

            # Can be used to dump spread names
            #	print (nextGameSpread.VisitorTeam)
            #	print (nextGameSpread.HomeTeam)

            # team name may have "(N)" at the end indicating neutral field, if it do, cut it
            if len(nextGameSpread.HomeTeam) > 5 and nextGameSpread.HomeTeam[-4:] == " (N)":
                nextGameSpread.HomeTeam = nextGameSpread.HomeTeam[:-4]
                neutralFieldGame = True

            # spread in two "columns" (divs) over
            thisDataDiv = thisDataDiv.next_sibling.next_sibling
            thisDataDiv = thisDataDiv.find_next("div", class_="child-current")
            textsInDiv = thisDataDiv.get_text("|", strip=True).split("|")
            spreadValue = textsInDiv[1]	
            
            nextGameSpread.SpreadToVisitor = spreadValue[:spreadValue.find(" ")]

            spreadJson = nextGameSpread.toJSON()
            self.logger.debug(spreadJson)

            # see if there is a match in the pickem games
            for pickemGame in pickemGamesForWeek:

                foundMatchingGame = False
                ncaaAndSpreadTeamsReversed = False # indicates if the home/away teams don't match between NCAA and spread data. Occurs with neutral site games sometimes.

                # NOTE: in neutral field cases the "home/away" teams don't always match
                # between NCAA data (pickem game source) and the spread data
                # this check is to see if they are reversed and if so, flip them in the data
                if neutralFieldGame and pickemGame['awayTeam']['team']['theSpreadName'] == nextGameSpread.HomeTeam and pickemGame['homeTeam']['team']['theSpreadName'] == nextGameSpread.VisitorTeam:
                    foundMatchingGame = True
                    ncaaAndSpreadTeamsReversed = True

                elif pickemGame['awayTeam']['team']['theSpreadName'] == nextGameSpread.VisitorTeam and pickemGame['homeTeam']['team']['theSpreadName'] == nextGameSpread.HomeTeam:
                    foundMatchingGame = True
                    ncaaAndSpreadTeamsReversed = False

                if foundMatchingGame:
                    # matched update pickem spread

                    gameId = pickemGame['gameId']

                    # which way does the spread go?
                    spreadDirection = "THIS_IS_WRONG_SHOULD_BE_SET"
                    if ( nextGameSpread.SpreadToVisitor == "0" ):
                        spreadDirection = "None"
                        absSpread = "0"
                    elif ( nextGameSpread.SpreadToVisitor[:1] == "-" ):
                        if ncaaAndSpreadTeamsReversed:
                            spreadDirection = "ToAway"	
                        else:
                            spreadDirection = "ToHome"
                        absSpread = nextGameSpread.SpreadToVisitor[1:]
                    else:
                        if ncaaAndSpreadTeamsReversed:
                            spreadDirection = "ToHome"	
                        else:
                            spreadDirection = "ToAway"
                        absSpread = nextGameSpread.SpreadToVisitor[1:]

                    self.apiClient.updateSpread(pickemSeason, weekNumber, gameId, spreadDirection, absSpread)
                    
                    updatedPickemGameCount = updatedPickemGameCount + 1
                    break

            spreadsGameCount = spreadsGameCount + 1


        self.logger.info("Read {0} game spreads. Matched {1} games and updated.".format(spreadsGameCount, updatedPickemGameCount))	