#!/usr/bin/env python3
 
import datetime
import json
import pickemLogger
import pickemApiClient
import requests
import time
from bs4 import BeautifulSoup

SPREAD_SITE_URL = "https://www.espn.com/college-football/lines"

class SpreadData:
    HomeTeam = ""
    NeutralFieldGame = False
    VisitorTeam = ""
    SpreadToVisitor = ""

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
            spreads = self.__loadSpreads()
            self.__updateSpreads(pickemGamesForWeek, spreads)

        elif ( actionCode == "lock" or actionCode == "l" ):
            self.__lockSpreads(pickemGamesForWeek)

        else:
            self.logger.wtf("Unhandled input why didn't argparser catch it? --action " + actionCode)


    def __findMatchingSpread(self, pickemGame, spreads):

        # TODO, there has to be a better way. This is straight up loopin'
        for spread in spreads:

            # NOTE: in neutral field cases the "home/away" teams don't always match
            # between NCAA data (pickem game source) and the spread data
            # this check is to see if they are reversed and if so, flip them in the data
            if spread.NeutralFieldGame and pickemGame['awayTeam']['team']['espnDisplayName'] == spread.HomeTeam and pickemGame['homeTeam']['team']['espnDisplayName'] == spread.VisitorTeam:
                return spread

            elif pickemGame['awayTeam']['team']['espnDisplayName'] == spread.VisitorTeam and pickemGame['homeTeam']['team']['espnDisplayName'] == spread.HomeTeam:
                return spread

        # was not matched in spreads
        return None

    def __loadSpreads(self):

        webHtml = self.apiClient.getHtml(SPREAD_SITE_URL)

        soup = BeautifulSoup(webHtml, "html.parser")

        spreads = []
        spreadsGameCount = 0

        # Loop on dates, these are the row groups with a specific date and time
        for tbodyGame in soup.find_all("tbody", class_="Table__TBODY"):
            
            nextGameSpread = SpreadData()

            # == Row 1 - visitor row
            trVisitor = tbodyGame.find_next("tr")
            aVisitorTeam = trVisitor.find_next("a").find_next("a")

            nextGameSpread.VisitorTeam = aVisitorTeam.get_text()

            # 3rd column is either line or spread
            tdVisitorSpreadOrOverUnder = trVisitor.find_next("td").next_sibling.next_sibling
            visitorSpreadOrOverUnderText = tdVisitorSpreadOrOverUnder.get_text()

            # == Row 2 - home row
            trHome = trVisitor.find_next("tr")
            aHomeTeam = trHome.find_next("a").find_next("a")

            nextGameSpread.HomeTeam = aHomeTeam.get_text()

            # 3rd column is either line or spread
            tdHomeSpreadOrOverUnder = trHome.find_next("td").next_sibling.next_sibling

            homeSpreadOrOverUnderText = tdHomeSpreadOrOverUnder.get_text()


            # **** fix this comment
                # looking for a negative number. "--" means nuthing
                # this is the spread value (instead of line)
                # we are in the visitor row so leave as is for SpreadToVisitor

            if ( visitorSpreadOrOverUnderText == "--" and homeSpreadOrOverUnderText == "--" ):
                # no line or spread data for this game, bail out
                continue

            elif ( visitorSpreadOrOverUnderText != "--" and visitorSpreadOrOverUnderText[:1] == "-" ):
                # spread in first row
                nextGameSpread.SpreadToVisitor = visitorSpreadOrOverUnderText

            elif ( homeSpreadOrOverUnderText[:1] == "-" ):
                # we are in the home row so "reverse" the spread value for visitor. Make it positive
                nextGameSpread.SpreadToVisitor = "+" + homeSpreadOrOverUnderText[1:]

            else:
                self.logger.wtf("Where is the spread? visitorSpreadOrOverUnderText (%s) homeSpreadOrOverUnderText(%s)" % (visitorSpreadOrOverUnderText, homeSpreadOrOverUnderText) )
                continue

            spreadJson = nextGameSpread.toJSON()
            self.logger.debug(spreadJson)

            spreads.append(nextGameSpread)
            spreadsGameCount = spreadsGameCount + 1

        self.logger.info("Read (%d) game spreads" % spreadsGameCount)	

        return spreads


    def __lockSpreads(self, pickemGamesForWeek):
        for pickemGame in pickemGamesForWeek:
            gameId = pickemGame['gameId']
            self.apiClient.lockSpread(gameId)

        self.logger.info("Locked {0} game spreads.".format(len(pickemGamesForWeek)))	

    def __updateSpreads(self, pickemGamesForWeek, spreads):

        updatedPickemGameCount = 0

        for pickemGame in pickemGamesForWeek:

            spreadForGame = self.__findMatchingSpread(pickemGame, spreads)

            if ( spreadForGame != None ):
                gameId = pickemGame['gameId']

                # "home/away" teams don't always match
                # between NCAA data (pickem game source) and the spread data
                # this check is to see if they are reversed and if so, flip them in the data
                ncaaAndSpreadTeamsReversed = pickemGame['awayTeam']['team']['espnDisplayName'] == spreadForGame.HomeTeam

                # which way does the spread go?
                spreadDirection = "THIS_IS_WRONG_SHOULD_BE_SET"
                if ( spreadForGame.SpreadToVisitor == "0" ):
                    spreadDirection = "None"
                    absSpread = "0"
                elif ( spreadForGame.SpreadToVisitor[:1] == "-" ):
                    if ncaaAndSpreadTeamsReversed:
                        spreadDirection = "ToAway"	
                    else:
                        spreadDirection = "ToHome"
                    absSpread = spreadForGame.SpreadToVisitor[1:]
                else:
                    if ncaaAndSpreadTeamsReversed:
                        spreadDirection = "ToHome"	
                    else:
                        spreadDirection = "ToAway"
                    absSpread = spreadForGame.SpreadToVisitor[1:]

                self.apiClient.updateSpread(gameId, spreadDirection, absSpread)
                
                updatedPickemGameCount = updatedPickemGameCount + 1
            else:
                # no spread found
                self.logger.warn("No spread found for game (%s) with espnDisplayName (%s) @ (%s) with espnDisplayName (%s)" % (pickemGame['awayTeam']['team']['teamCode'], pickemGame['awayTeam']['team']['espnDisplayName'], pickemGame['homeTeam']['team']['teamCode'], pickemGame['homeTeam']['team']['espnDisplayName']))

        self.logger.info("Read {0} game spreads. Matched {1} games and updated.".format(len(spreads), updatedPickemGameCount))	
        