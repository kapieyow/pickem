#!/usr/bin/env python3

import re
import requests
from bs4 import BeautifulSoup

YAHOO_GAMES_URL = "https://football.fantasysports.yahoo.com/college/pickdistribution?gid=&type=c&week="

class PickemAddYahooGamesHandler:
    def __init__(self, core):
        self.core = core

    def Run(self, leagueCodes, weekNumber):
    
        yahooGameHtml = self.core.apiClient.getHtml(YAHOO_GAMES_URL + str(weekNumber))
        soup = BeautifulSoup(yahooGameHtml, "html.parser")

        yahooGameCount = 0

        for gameDiv in soup.find_all("div", class_="ysf-matchup-dist"):

            favoriteA = gameDiv.find_next("dd").find_next("a")
            favoriteHref = favoriteA['href']
            favoriteYahooShortCode = re.search("^.*\/([^\/]*)\/", favoriteHref).group(1)

            underdogA = favoriteA.find_next("dd").find_next("dd").find_next("a")
            underdogHref = underdogA['href']
            underdogYahooShortCode = re.search("^.*\/([^\/]*)\/", underdogHref).group(1)

            self.core.logger.info("Yahoo game (%s) - (%s)" % (favoriteYahooShortCode, underdogYahooShortCode))

            self.core.setLeagueGame(leagueCodes, weekNumber, None, None, favoriteYahooShortCode, 1)

            yahooGameCount += 1
            
        self.core.logger.info("Added (%d) games from yahoo" % (yahooGameCount))


