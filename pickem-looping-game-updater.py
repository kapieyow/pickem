#!/usr/bin/env python3
import os
import subprocess
import time

SLEEP_SECONDS = 120

print("----------------------------------------")
print("  Pick'em game updating looper")
print("----------------------------------------")

while(True):
    if (os.name == 'nt'):
        # windoze no ./
        subprocess.call("pickem-game-syncher.py -ns 2018 -ps 18 -w 5 -a update -s ncaaCasablanca", shell=True)
    else:
        #nix
        subprocess.call("./pickem-game-syncher.py -ns 2018 -ps 18 -w 5 -a update -s ncaaCasablanca", shell=True)
        
    print("-- snoozing " + str(SLEEP_SECONDS) + " seconds")
    time.sleep(SLEEP_SECONDS)

