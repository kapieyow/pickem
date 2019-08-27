# Pick'em
Pick'em is a questionable system that allows the picking of NCAAF games based on the spread.

### Development Environment Setup
Here is some incomplete documentation on how to setup a development environment. Feel free to contact me if you have questions.

Pick'em is comprised of:
- a [postgresql](https://www.postgresql.org/) database
- a server built on [.NET Core](https://docs.microsoft.com/en-us/dotnet/core/)
- The server uses [Marten](http://jasperfx.github.io/marten/) to make postgres look like a NoSQL db
- a web site built with [Angular](https://angular.io/)
- a CLI built with [python](https://www.python.org/)
- There is also the start of an Android app

#### Development Tools
I do development with the following
- Windows 10
- VS.NET Community 2017 for server development
- Postgres 9.5 in an ubuntu VM on the same Win 10 PC
- pgAdmin for postgres admin
- VS Code for web and CLI development
- Android Studio for android app
- Postman for API runs

#### Deployed Environment Types
I don't describe the detail yet, but this runs on linux (Ubuntu 16) on ARM based HW for upper environments. Should run on full windows or other *nix combinations.

#### Database Setup
- Install postgres (9.5+) and pgAdmin
- Modify postgres config `pg_hba.conf` to allow LAN access. This example assumes a subnet of `198.168.0.x`. Add the following line to the config file using your subnet
    `host all all 192.168.0.0/24 md5`
- Add the following line to the postgres config `postgresql.conf` so postgres will listen for all clients on the network.
    `listen_addresses = '*'`
- Change password on "postgres" user. Windows does on install. For ubuntu
    - `sudo -u postgres psql postgres`
    - in psql `\password postgres`
    - To get out of psql: `\q`
- Add a database named **pickem_dev** for dev
- Restore db backup into it. Using a backup file like `PRETEND_PICKEM_BACKUP.sql` run the following from the command line `psql -U kip pickem_dev < PRETEND_PICKEM_BACKUP.sql`

**Database Tips**
- All tables are built by Marten and so follow a similar column pattern
- Basically there are a few columns for keys like id, but the bulk of the data in is JSON in the data column
- To the server the database looks like a NoSQL database even though it’s a RDBMS
- See [postgres documentation for json/jsonb](https://www.postgresql.org/docs/9.5/functions-json.html) functions as there are several to read/write JSON within the data column.

#### Server Setup
- Clone repo
- Server is here from repo root /pickem-server
- Open in VS.NET (can run in VS Code to)
In appsettings.Developement.json modify PostgresConnection/ConnectionString to whatever your connection string is.
- Run/debug in VS.NET
- It should open browser with `http://localhost:51890/api/status` with results something like 
```sh
{"authenticatedUserName":null,"database":"pickem_dev","databaseHost":"xyz.ps.lan","product":"Pick'em - Server","productVersion":"0.0.0-local","runtimeEnvironment":"Development"}
```
- Swagger is here `http://localhost:51890/swagger/`

**Server Tips**
- Server is where most of the interesting stuff happens.
- Uses Marten to make postgres look like a NoSQL db.
- Will create tables if they aren't already there.
- Server is the only sub-system that interacts with the db directly
- Server exposes a REST API for reads / writes etc
- Server exposes a web socket "server" that is used to emit events to any web socket client in real time. The web is a client and droid will be a client. Example events are game changes (score), spread updated etc. Full list here `PickEmServer.App.PickemSystemEventTypes`.

#### Web Setup
- Web is in the repo root /pickem-web
- Install Angular 6. I am using CLI 6.0.8, Angular 6.0.9 and node 8.11.3 right now
- In VS Code, modify src/environments/environment.ts to use your dev server. May be the same if you are on localhost.

**Web Tips**
- Chrome console will log a bunch of stuff
- The logs "above" debug will go to the server too
- Clicking the "NCAAF - Pickem" title will show a status dialog
- The hyphen in "NCAAF - Pickem" is orange if the web socket connection is down
- The single quote in "NCAAF - Pickem" is orange when the site is pulling scores
- Clicking your user id in the header logs off
- The server web socket events trigger a scoreboard repull (after being debounced). The socket receipts are logged to the console logger.

#### Scripts Setup
- Scripts are in the repo root /pickem-scripts
- All of them will run on windows or ubuntu
- Note the file has to be CR and not CRLF for ubuntu. - Can set in VS Code to to that.
- They run on python 3.5 + (I have 3.5.2 on ubuntu and 3.7 on windows)
- Install python 3.5+
- In VS.NET modify pickem-settings.ini to point to your server url and the root password
- In the /pickem-scripts dir you can see if python is cool by running: python --version should get something like Python 3.7.0
- The main script is pickem.py. It uses several others to run. The first argument to pickem.py is the action. Each action has different params. See below

|Action|Example|Notes
| ------ | ------ |------ |
|extract_games|`./pickem.py extract_games -psc 19 -w 1`|Reads all games (may not be in any pickem league) for the season and week and outputs them 
|set_league_game|`./pickem.py set_league_game -w 14 -lcs TESTConfChamps -gid 3251144 -gwp 4`|Adds a single game to one or more leagues. NOTE: this action can take one of three keys, -gid (game id), -ptc (pickem team code) or -ytc (yahoo team code)|
|set_league_games|`./pickem.py set_league_games -lcs BUS BurlMafia NeOnYa -w 12 -gids 2953176 3251144`|Adds a one or more games to one or more leagues. This allows bulk game adds, but it cannot set win points so will default to 1 win point|
|set_league_games_from_yahoo|`./pickem.py set_league_games_from_yahoo -lcs League-A League-B League-C -w 1`|Reads games from Yahoo and adds all to each league. This allows bulk game adds, but it cannot set win points so will default to 1 win point|
|set_league_week|`./pickem.py set_league_week -lcs League1 League2 -w 13`|Updates the current week for one or more leagues. The GUIs will default to the week.|
|synch_games|`./pickem.py synch_games -a u -psc 19 -esc 2019 -w 1 -les 180`|Synchronizes game data from external source. The -a option may be `u` for update or `i` for insert.|
|update_teams|`./pickem.py update_teams -rs cfp -psc 18 -w 12`|Updates win/loss team records and rankings. Can get rankings from AP or CFP|
|update_spreads|`./pickem.py update_spreads -a u -psc 18 -w 12`|Can update spreads `-a u` from external source. Will lock spreads using `-a l`|

**Script Tips**
- Most CLI params have long names e.g. `-w` is the same as `-week`
Help sucks from the CLI right now. TODO for later. See pickem.py setupArgumentParsers for some detail

#### Android Setup
TBD 
