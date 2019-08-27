# Deployment Steps

### 2.1.x
- deploy server
- restart server (kestrel)
- deploy scripts

#### If not production
- drop all db
- restore from 2.0.x backup

#### Always
- run /pickem-sql/2.1.x - upgrade.sql
- run /pickem-scripts/run-once-setup-2019-NCAAF.py


### 2.2.x
- deploy server
- restart server (kestrel)
- deploy scripts

#### If not production
- drop all db
- restore from 2.1.x backup

#### Always
- run /pickem-sql/2.2.x - upgrade.sql
