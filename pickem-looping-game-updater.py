#!/usr/bin/env python3
import subprocess
import time

SLEEP_SECONDS = 300

print("----------------------------------------")
print("  Pick'em game updating looper")
print("----------------------------------------")

while(True):
    subprocess.call(["python.exe", "pickem-game-syncher.py", "-ns", "2018", "-ps", "18", "-w", "1", "-a", "update"])
    print("-- snoozing " + str(SLEEP_SECONDS) + " seconds")
    time.sleep(SLEEP_SECONDS)

