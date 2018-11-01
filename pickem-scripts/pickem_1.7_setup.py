#!/usr/bin/env python3
import os
import subprocess
 
for i in range(1, 9):
    if (os.name == 'nt'):
        # windoze no ./
        subprocess.call("pickem.py setup_week -w " + str(i), shell=True)
        subprocess.call("pickem.py synch_games -a update", shell=True)
    else:
        #nix
        subprocess.call("./pickem.py setup_week -w " + str(i), shell=True)
        subprocess.call("./pickem.py synch_games -a update", shell=True)
