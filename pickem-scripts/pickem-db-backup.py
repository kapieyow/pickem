#!/usr/bin/env python3
import argparse
import datetime
import subprocess
 
parser = argparse.ArgumentParser(description='Backup a Postgres db')
parser.add_argument('-db', '--database', required=True, help='Database name')
args = parser.parse_args()

now = datetime.datetime.now()
output_filename = now.strftime('%Y%m%d-%H%M%S') + "-" + args.database + ".sql"
subprocess.call("pg_dump " + args.database + " > " + output_filename, shell=True)
print("DB dumped to file: " + output_filename)
