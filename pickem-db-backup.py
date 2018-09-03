#!/usr/bin/env python3

import argparse
import datetime

parser = argparse.ArgumentParser(description='Backup a Postgres db')
parser.add_argument('-db', '--database', required=True, help='Database name')
parser.add_argument('-U', '--database_user', required=True, help='Database user')
args = parser.parse_args()

now = datetime.datetime.now()

print(now.isoformat())