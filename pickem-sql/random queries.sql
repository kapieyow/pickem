/* 	THIS WILL DROP ALL DATA !!!!!!!!!!!!!!

-- to reset a database WILL DROP ALL TABLES / FUNCS etc in public schema
DROP SCHEMA public CASCADE;
CREATE SCHEMA public AUTHORIZATION postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO public;

*/



-- =========================================================
-- Users with related leagues
-- =========================================================
SELECT 
	u.data->>'UserName' AS UserName,
	u.data->>'Email' AS Email,
	_leaguePlayers.PlayerTag,
	_leaguePlayers.LeagueCode,
	_leaguePlayers.LeagueTitle,
	CASE WHEN ( u.data->>'DefaultLeagueCode' = _leaguePlayers.LeagueCode ) THEN 'Yes' ELSE NULL END AS DefaultLeague
FROM 
	public.mt_doc_pickemuser u
	LEFT OUTER JOIN 
	(
		SELECT 
			p->>'UserNameRef' AS UserNameRef,
			p->>'PlayerTag' AS PlayerTag,
			l.data->>'LeagueCode' AS LeagueCode,
			l.data->>'LeagueTitle' AS LeagueTitle
		FROM 
			public.mt_doc_leaguedata l 
			CROSS JOIN jsonb_array_elements(data->'Players') p
	) AS _leaguePlayers ON _leaguePlayers.UserNameRef = u.data->>'UserName'
ORDER BY
	u.data->>'UserName'
;
-- =/=/=/=/=/=/=/=/=/=/=/=/=/=/=/=/=/=/=/=/=/=/=/=/=/=/=/=/=/






-- logs newest first 
SELECT
	logs.data->>'Id' AS Id,
	logs.mt_last_modified,
	logs.data->>'LogLevel' AS LogLevel,
	logs.data->>'Component' AS Component,
	logs.data->>'LogMessage' AS LogMessage,
	logs.data
FROM
	public.mt_doc_logdata logs
ORDER BY 
	logs.mt_last_modified DESC

	
-- games by week
SELECT
	g.data->>'GameId' AS GameId,
	g.data->>'GameState' AS GameState,
	g.data->'HomeTeam'->>'TeamCodeRef' AS HomeTeam,
	homet.data->>'theSpreadName' AS HomeTheSpreadName,
	g.data->'AwayTeam'->>'TeamCodeRef' AS AwayTeam,
	awayt.data->>'theSpreadName' AS AwayTheSpreadName,
	g.data->'Spread'->>'PointSpread' AS Spread,
	g.data->'Spread'->>'SpreadDirection' AS SpreadDirection,
	* 
FROM
	public.mt_doc_gamedata g
	INNER JOIN public.mt_doc_teamdata awayt ON g.data->'AwayTeam'->>'TeamCodeRef' = awayt.id
	INNER JOIN public.mt_doc_teamdata homet ON g.data->'HomeTeam'->>'TeamCodeRef' = homet.id
WHERE
	g.data->>'SeasonCodeRef' = '18'
	AND
	g.data->>'WeekNumberRef' = '12'
ORDER BY 
	g.data->'HomeTeam'->>'TeamCodeRef'
	
-- games by week (for week sets)
SELECT
	g.data->>'GameId' AS GameId,
	g.data->>'GameState' AS GameState,
	g.data->'HomeTeam'->>'TeamCodeRef' AS HomeTeam,
	homet.data->>'LongName' AS HomeLongName,
	homet.data->>'ShortName' AS HomeShortName,
	homet.data->>'theSpreadName' AS HomeTheSpreadName,
	g.data->'AwayTeam'->>'TeamCodeRef' AS AwayTeam,
	awayt.data->>'theSpreadName' AS AwayTheSpreadName,
	g.data->'Spread'->>'PointSpread' AS Spread,
	g.data->'Spread'->>'SpreadDirection' AS SpreadDirection,
	'{ ''gameId'': '::text || CAST(g.data->>'GameId' AS text) || ' }'::text AS PostPayload,
	* 
FROM
	public.mt_doc_gamedata g
	INNER JOIN public.mt_doc_teamdata awayt ON g.data->'AwayTeam'->>'TeamCodeRef' = awayt.id
	INNER JOIN public.mt_doc_teamdata homet ON g.data->'HomeTeam'->>'TeamCodeRef' = homet.id
WHERE
	g.data->>'SeasonCodeRef' = '18'
	AND
	g.data->>'WeekNumberRef' = '11'
	AND
	-- hack at Yahoo games
	g.data->'HomeTeam'->>'TeamCodeRef' IN 
	(
		'north-carolina-st'
	)
ORDER BY 
	g.data->'HomeTeam'->>'TeamCodeRef'

-- league game by week
SELECT 
	g.data->>'GameId' AS GameId,
	weeks->>'WeekNumberRef' AS Week,
	g.data->>'GameState' AS GameState,
	g.data->>'GameStart' AS GameStart,
	g.data->'HomeTeam'->>'TeamCodeRef' AS HomeTeam,
	g.data->'HomeTeam'->>'Score' AS HomeScore,
	g.data->'AwayTeam'->>'TeamCodeRef' AS AwayTeam,
	g.data->'AwayTeam'->>'Score' AS AwayScore,
	g.data->'Spread'->>'PointSpread' AS Spread,
	g.data->'Spread'->>'SpreadDirection' AS SpreadDirection,
	g.data->>'Leader' AS Leader,
	g.data->>'LeaderAfterSpread' AS LeaderAfterSpread,
	'{ ''gameId'': '::text || CAST(g.data->>'GameId' AS text) || ' }'::text AS PostPayload,
	g.*
FROM 
	public.mt_doc_leaguedata l,
	SELECT 
	g.data->>'GameId' AS GameId,
	weeks->>'WeekNumberRef' AS Week,
	g.data->>'GameState' AS GameState,
	g.data->>'GameStart' AS GameStart,
	g.data->'HomeTeam'->>'TeamCodeRef' AS HomeTeam,
	g.data->'HomeTeam'->>'Score' AS HomeScore,
	g.data->'AwayTeam'->>'TeamCodeRef' AS AwayTeam,
	g.data->'AwayTeam'->>'Score' AS AwayScore,
	g.data->'Spread'->>'PointSpread' AS Spread,
	g.data->'Spread'->>'SpreadDirection' AS SpreadDirection,
	g.data->>'Leader' AS Leader,
	g.data->>'LeaderAfterSpread' AS LeaderAfterSpread,
	'{ ''gameId'': '::text || CAST(g.data->>'GameId' AS text) || ' }'::text AS PostPayload,
	g.*
FROM 
	public.mt_doc_leaguedata l,
	jsonb_array_elements(data->'Weeks') weeks,
	jsonb_array_elements(weeks->'Games') gameRefs
	INNER JOIN public.mt_doc_gamedata g ON g.Id::text = gameRefs->>'GameIdRef'
WHERE 
	l.data->>'LeagueCode' = 'NeOnYa'
	AND
	weeks->>'WeekNumberRef' IN ('7')
ORDER BY
	weeks->>'WeekNumberRef',
	g.data->>'GameStart',
	g.data->'AwayTeam'->>'TeamCodeRef'
	
-- league game by week (with players)
SELECT 
	g.data->>'GameId' AS GameId,
	g.data->>'GameState' AS GameState,
	g.data->'HomeTeam'->>'TeamCodeRef' AS HomeTeam,
	g.data->'HomeTeam'->>'Score' AS HomeScore,
	g.data->'AwayTeam'->>'TeamCodeRef' AS AwayTeam,
	g.data->'AwayTeam'->>'Score' AS AwayScore,
	g.data->'Spread'->>'PointSpread' AS Spread,
	g.data->'Spread'->>'SpreadDirection' AS SpreadDirection,
	playerPicks->>'PlayerTagRef' AS PlayerTag,
	playerPicks->>'Pick' AS Pick,
	playerPicks->>'PickStatus' AS PickStatus,
	playerPicks.*
FROM 
	public.mt_doc_leaguedata l,
	jsonb_array_elements(data->'Weeks') weeks,
	jsonb_array_elements(weeks->'Games') gameRefs
		INNER JOIN public.mt_doc_gamedata g ON g.Id::text = gameRefs->>'GameIdRef',
	jsonb_array_elements(gameRefs->'PlayerPicks') playerPicks
WHERE 
	l.data->>'LeagueCode' = 'NeOnYa'
	AND
	weeks->>'WeekNumberRef' = '1'




-- league game by week (with players)
SELECT 
	l.id,
	g.data->>'GameId' AS GameId,
	g.data->>'GameState' AS GameState,
	g.data->'HomeTeam'->>'TeamCodeRef' AS HomeTeam,
	g.data->'HomeTeam'->>'Score' AS HomeScore,
	g.data->'AwayTeam'->>'TeamCodeRef' AS AwayTeam,
	g.data->'AwayTeam'->>'Score' AS AwayScore,
	g.data->'Spread'->>'PointSpread' AS Spread,
	g.data->'Spread'->>'SpreadDirection' AS SpreadDirection,
	playerPicks->>'PlayerTagRef' AS PlayerTag,
	playerPicks->>'Pick' AS Pick,
	playerPicks->>'PickStatus' AS PickStatus,
	playerPicks.*
FROM 
	public.mt_doc_leaguedata l,
	jsonb_array_elements(data->'Weeks') weeks,
	jsonb_array_elements(weeks->'Games') gameRefs
		INNER JOIN public.mt_doc_gamedata g ON g.Id::text = gameRefs->>'GameIdRef',
	jsonb_array_elements(gameRefs->'PlayerPicks') playerPicks
WHERE 
	g.data->>'GameState' = 'Cancelled'


SELECT 
	p->>'UserNameRef' AS UserNameRef,
	p->>'PlayerTag' AS PlayerTag,
	l.data->>'LeagueCode' AS LeagueCode,
	l.data->>'LeagueTitle' AS LeagueTitle
FROM 
	public.mt_doc_leaguedata l 
	CROSS JOIN jsonb_array_elements(data->'Players') p
--WHERE 
--	p->>'UserNameRef' = 'gumanchew'
ORDER BY
	p->>'UserNameRef',
	l.data->>'LeagueCode'
; 


-- users
SELECT 
	u.data->>'UserName' AS UserName,
	u.data->>'Email' AS Email,
	u.data->>'DefaultLeagueCode' AS DefaultLeagueCode,
	_leaguePlayers.PlayerTag,
	_leaguePlayers.LeagueCode,
	_leaguePlayers.LeagueTitle,
	CASE WHEN ( u.data->>'DefaultLeagueCode' = _leaguePlayers.LeagueCode ) THEN 'Yes' ELSE NULL END AS DefaultLeague
FROM 
	public.mt_doc_pickemuser u
	LEFT OUTER JOIN 
	(
		SELECT 
			p->>'UserNameRef' AS UserNameRef,
			p->>'PlayerTag' AS PlayerTag,
			l.data->>'LeagueCode' AS LeagueCode,
			l.data->>'LeagueTitle' AS LeagueTitle
		FROM 
			public.mt_doc_leaguedata l 
			CROSS JOIN jsonb_array_elements(data->'Players') p
	) AS _leaguePlayers ON _leaguePlayers.UserNameRef = u.data->>'UserName'
ORDER BY
	u.data->>'UserName'
;

-- to reset a database WILL DROP ALL TABLES / FUNCS etc in public schema
DROP SCHEMA public CASCADE;
CREATE SCHEMA public AUTHORIZATION postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO public;


-- pretty teams
SELECT 
	jsonb_pretty(t.data)
FROM 
	public.mt_doc_teamdata t

SELECT 
	*
FROM 
	public.mt_doc_teamdata t
WHERE
	t.id = 'south-carolina'

--BEGIN; -- COMMIT ROLLBACK
--UPDATE public.mt_doc_teamdata SET data = '{"LongName": "UCF", "TeamCode": "ucf", "ShortName": "", "NcaaNameSeo": "ucf", "theSpreadName": "Central Florida", "icon24FileName": "ucf.24.png"}' WHERE id = 'ucf';


-- pretty specfic game
SELECT
	jsonb_pretty(g.data)
FROM
	public.mt_doc_gamedata g
WHERE
	g.Id = 2953222

-- pretty DEFAULT league	
SELECT 
	jsonb_pretty(l.data)
FROM 
	public.mt_doc_leaguedata l 
WHERE 
	l.data->>'LeagueCode' = 'Default'

	
SELECT 
	*
FROM 
	public.mt_doc_leaguedata l 
WHERE 
	l.data->>'LeagueCode' = 'Default'


SELECT 
	r.*
FROM 
	public.mt_doc_leaguedata l
	CROSS JOIN LATERAL jsonb_to_recordset(l.data#>'{Weeks,0,Games}') as r("GameIdRef" int)
WHERE
	l.data->>'LeagueCode' = 'BUS'





/* == week 5 (1.4.x) 

-- set default league for BUS crew
UPDATE public.mt_doc_pickemuser SET data = jsonb_set(data, '{DefaultLeagueCode}', '"BUS"') WHERE data->>'UserName' IN ('Chris', 'CusheGoblin', 'gumanchew', 'GingerNinja');

-- BUS league data
DELETE FROM public.mt_doc_leaguedata WHERE data->>'LeagueCode' = 'BUS'
INSERT INTO public.mt_doc_leaguedata(id, data, mt_last_modified, mt_version, mt_dotnet_type) VALUES ('BUS','{"Weeks": [{"Games": [], "WeekNumberRef": 1, "PlayerWeekScores": [{"Points": 0, "PlayerTagRef": "Chris"}, {"Points": 0, "PlayerTagRef": "CusheGoblin"}, {"Points": 0, "PlayerTagRef": "gumanchew"}, {"Points": 0, "PlayerTagRef": "McClucky"}, {"Points": 0, "PlayerTagRef": "GingerNinja"}]}, {"Games": [], "WeekNumberRef": 2, "PlayerWeekScores": [{"Points": 0, "PlayerTagRef": "Chris"}, {"Points": 0, "PlayerTagRef": "CusheGoblin"}, {"Points": 0, "PlayerTagRef": "gumanchew"}, {"Points": 0, "PlayerTagRef": "McClucky"}, {"Points": 0, "PlayerTagRef": "GingerNinja"}]}, {"Games": [], "WeekNumberRef": 3, "PlayerWeekScores": [{"Points": 0, "PlayerTagRef": "Chris"}, {"Points": 0, "PlayerTagRef": "CusheGoblin"}, {"Points": 0, "PlayerTagRef": "gumanchew"}, {"Points": 0, "PlayerTagRef": "McClucky"}, {"Points": 0, "PlayerTagRef": "GingerNinja"}]}, {"Games": [{"GameIdRef": 2954204, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "McClucky"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2954362, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "gumanchew"}, {"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "McClucky"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2953851, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "gumanchew"}, {"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "McClucky"}, {"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2953549, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "gumanchew"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "McClucky"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2953959, "PlayerPicks": [{"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "Chris"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "gumanchew"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "McClucky"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2954297, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "McClucky"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2954359, "PlayerPicks": [{"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "gumanchew"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "McClucky"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2953210, "PlayerPicks": [{"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "McClucky"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2953717, "PlayerPicks": [{"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "McClucky"}, {"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2953429, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "Chris"}, {"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "McClucky"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2954018, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "gumanchew"}, {"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "McClucky"}, {"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2953898, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Pushed", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Pushed", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Away", "PickStatus": "Pushed", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Pushed", "PlayerTagRef": "McClucky"}, {"Pick": "Away", "PickStatus": "Pushed", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2953961, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "Chris"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "McClucky"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2953561, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "McClucky"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2954147, "PlayerPicks": [{"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "Chris"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "gumanchew"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "McClucky"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2953259, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "McClucky"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2953041, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "Chris"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "gumanchew"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "McClucky"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2954050, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "Chris"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "gumanchew"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "McClucky"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2954222, "PlayerPicks": [{"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "Chris"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "McClucky"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2954415, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "gumanchew"}, {"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "McClucky"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2954482, "PlayerPicks": [{"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "Chris"}, {"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Home", "PickStatus": "Won", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "McClucky"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2954450, "PlayerPicks": [{"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "McClucky"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2953060, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "McClucky"}, {"Pick": "Away", "PickStatus": "Lost", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2953956, "PlayerPicks": [{"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "McClucky"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "GingerNinja"}]}, {"GameIdRef": 2953116, "PlayerPicks": [{"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "Chris"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "CusheGoblin"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "gumanchew"}, {"Pick": "Away", "PickStatus": "Won", "PlayerTagRef": "McClucky"}, {"Pick": "Home", "PickStatus": "Lost", "PlayerTagRef": "GingerNinja"}]}], "WeekNumberRef": 4, "PlayerWeekScores": [{"Points": 10, "PlayerTagRef": "Chris"}, {"Points": 11, "PlayerTagRef": "CusheGoblin"}, {"Points": 11, "PlayerTagRef": "gumanchew"}, {"Points": 13, "PlayerTagRef": "McClucky"}, {"Points": 10, "PlayerTagRef": "GingerNinja"}]}, {"Games": [], "WeekNumberRef": 5, "PlayerWeekScores": [{"Points": 0, "PlayerTagRef": "Chris"}, {"Points": 0, "PlayerTagRef": "CusheGoblin"}, {"Points": 0, "PlayerTagRef": "gumanchew"}, {"Points": 0, "PlayerTagRef": "McClucky"}, {"Points": 0, "PlayerTagRef": "GingerNinja"}]}, {"Games": [], "WeekNumberRef": 6, "PlayerWeekScores": [{"Points": 0, "PlayerTagRef": "Chris"}, {"Points": 0, "PlayerTagRef": "CusheGoblin"}, {"Points": 0, "PlayerTagRef": "gumanchew"}, {"Points": 0, "PlayerTagRef": "McClucky"}, {"Points": 0, "PlayerTagRef": "GingerNinja"}]}, {"Games": [], "WeekNumberRef": 7, "PlayerWeekScores": [{"Points": 0, "PlayerTagRef": "Chris"}, {"Points": 0, "PlayerTagRef": "CusheGoblin"}, {"Points": 0, "PlayerTagRef": "gumanchew"}, {"Points": 0, "PlayerTagRef": "McClucky"}, {"Points": 0, "PlayerTagRef": "GingerNinja"}]}, {"Games": [], "WeekNumberRef": 8, "PlayerWeekScores": [{"Points": 0, "PlayerTagRef": "Chris"}, {"Points": 0, "PlayerTagRef": "CusheGoblin"}, {"Points": 0, "PlayerTagRef": "gumanchew"}, {"Points": 0, "PlayerTagRef": "McClucky"}, {"Points": 0, "PlayerTagRef": "GingerNinja"}]}, {"Games": [], "WeekNumberRef": 9, "PlayerWeekScores": [{"Points": 0, "PlayerTagRef": "Chris"}, {"Points": 0, "PlayerTagRef": "CusheGoblin"}, {"Points": 0, "PlayerTagRef": "gumanchew"}, {"Points": 0, "PlayerTagRef": "McClucky"}, {"Points": 0, "PlayerTagRef": "GingerNinja"}]}, {"Games": [], "WeekNumberRef": 10, "PlayerWeekScores": [{"Points": 0, "PlayerTagRef": "Chris"}, {"Points": 0, "PlayerTagRef": "CusheGoblin"}, {"Points": 0, "PlayerTagRef": "gumanchew"}, {"Points": 0, "PlayerTagRef": "McClucky"}, {"Points": 0, "PlayerTagRef": "GingerNinja"}]}, {"Games": [], "WeekNumberRef": 11, "PlayerWeekScores": [{"Points": 0, "PlayerTagRef": "Chris"}, {"Points": 0, "PlayerTagRef": "CusheGoblin"}, {"Points": 0, "PlayerTagRef": "gumanchew"}, {"Points": 0, "PlayerTagRef": "McClucky"}, {"Points": 0, "PlayerTagRef": "GingerNinja"}]}, {"Games": [], "WeekNumberRef": 12, "PlayerWeekScores": [{"Points": 0, "PlayerTagRef": "Chris"}, {"Points": 0, "PlayerTagRef": "CusheGoblin"}, {"Points": 0, "PlayerTagRef": "gumanchew"}, {"Points": 0, "PlayerTagRef": "McClucky"}, {"Points": 0, "PlayerTagRef": "GingerNinja"}]}, {"Games": [], "WeekNumberRef": 13, "PlayerWeekScores": [{"Points": 0, "PlayerTagRef": "Chris"}, {"Points": 0, "PlayerTagRef": "CusheGoblin"}, {"Points": 0, "PlayerTagRef": "gumanchew"}, {"Points": 0, "PlayerTagRef": "McClucky"}, {"Points": 0, "PlayerTagRef": "GingerNinja"}]}, {"Games": [], "WeekNumberRef": 14, "PlayerWeekScores": [{"Points": 0, "PlayerTagRef": "Chris"}, {"Points": 0, "PlayerTagRef": "CusheGoblin"}, {"Points": 0, "PlayerTagRef": "gumanchew"}, {"Points": 0, "PlayerTagRef": "McClucky"}, {"Points": 0, "PlayerTagRef": "GingerNinja"}]}, {"Games": [], "WeekNumberRef": 15, "PlayerWeekScores": [{"Points": 0, "PlayerTagRef": "Chris"}, {"Points": 0, "PlayerTagRef": "CusheGoblin"}, {"Points": 0, "PlayerTagRef": "gumanchew"}, {"Points": 0, "PlayerTagRef": "McClucky"}, {"Points": 0, "PlayerTagRef": "GingerNinja"}]}], "Players": [{"PlayerTag": "Chris", "UserNameRef": "Chris"}, {"PlayerTag": "CusheGoblin", "UserNameRef": "CusheGoblin"}, {"PlayerTag": "gumanchew", "UserNameRef": "gumanchew"}, {"PlayerTag": "McClucky", "UserNameRef": "kip"}, {"PlayerTag": "GingerNinja", "UserNameRef": "GingerNinja"}], "LeagueCode": "BUS", "LeagueTitle": "BUS", "SeasonCodeRef": "18", "CurrentWeekRef": 4, "PlayerSeasonScores": [{"Points": 10, "PlayerTagRef": "Chris"}, {"Points": 11, "PlayerTagRef": "CusheGoblin"}, {"Points": 11, "PlayerTagRef": "gumanchew"}, {"Points": 13, "PlayerTagRef": "McClucky"}, {"Points": 10, "PlayerTagRef": "GingerNinja"}]}','2018-09-23 01:48:05.745282-04','016604f8-7cec-4e27-ae4a-cf2d1af6ac99','PickEmServer.Data.Models.LeagueData')

*/	



-- any league with a specific game
SELECT 
	jsonb_pretty(l.data)
FROM 
	public.mt_doc_leaguedata l,
	jsonb_array_elements(data->'Weeks') weeks,
	jsonb_array_elements(weeks->'Games') games
WHERE 
	games->>'GameIdRef' = '2953222'

-- all games by state
SELECT
	jsonb_pretty(g.data)

BEGIN; -- COMMIT ROLLBACK
UPDATE
	public.mt_doc_gamedata g
SET
	data = jsonb_set(jsonb_set(data, '{HomeTeam, Score}', '-1'), '{GameState}', '"InGame"')
WHERE
	--g.data->>'SeasonCodeRef' = '18'
	--AND
	--g.data->>'WeekNumberRef' = '7'
	--AND
	-- hack at Yahoo games
	g.id = '2953974'
	--g.data->>'GameState' = 'Cancelled'


--UPDATE public.mt_doc_teamdata SET data = '{"LongName": "James Madison", "TeamCode": "james-madison", "ShortName": "", "NcaaNameSeo": "james-madison", "theSpreadName": "", "icon24FileName": "james-madison.24.png"}' WHERE id = 'james-madison';
--UPDATE public.mt_doc_teamdata SET data = '{"LongName": "South Dakota State", "TeamCode": "south-dakota-st", "ShortName": "", "NcaaNameSeo": "south-dakota-st", "theSpreadName": "", "icon24FileName": "south-dakota-st.24.png"}' WHERE id = 'south-dakota-st';	
	
--SELECT data FROM public.mt_doc_pickemuser WHERE data->>'UserName' = 'flemijim';

--UPDATE public.mt_doc_teamdata SET data = '{"LongName": "Washington State", "TeamCode": "washington-st", "ShortName": "", "NcaaNameSeo": "washington-st", "theSpreadName": "Washington State", "icon24FileName": "washington-st.24.png"}' WHERE id = 'washington-st';



SELECT 
	l.data#>>'{Weeks,14}' -- 15th week ordinally
FROM 
	public.mt_doc_leaguedata l





SELECT
	_l.id,
	jsonb_set(_l.data, '{Weeks}', jsonb_agg(jsonb_set(weeks.value, '{Games}', COALESCE(_updatedGamesByWeek.GamesJson, '[]')))) AS LeagueJson
FROM
	public.mt_doc_leaguedata _l
	CROSS JOIN jsonb_array_elements(_l.data->'Weeks') weeks
	LEFT OUTER JOIN 
	(
		SELECT 
			__l.id,
			weeks->>'WeekNumberRef' AS WeekNumberRef,
			jsonb_agg(jsonb_set(gameRefs.value, '{WinPoints}', '1')) AS GamesJson
		FROM 
			public.mt_doc_leaguedata __l,
			jsonb_array_elements(data->'Weeks') weeks,
			jsonb_array_elements(weeks->'Games') gameRefs
		GROUP BY
			__l.id,
			weeks->>'WeekNumberRef'
		ORDER BY 
			__l.id,
			to_number(weeks->>'WeekNumberRef', '999990')
	) _updatedGamesByWeek ON 
		(
			weeks->>'WeekNumberRef' = _updatedGamesByWeek.WeekNumberRef 
			AND 
			_l.id = _updatedGamesByWeek.id
		)
GROUP BY
	_l.id,
	_l.data
	

SELECT
	_l.id,
	jsonb_set(_l.data, '{Weeks}', jsonb_agg(jsonb_set(weeks.value, '{PlayerWeekScores}', _updatedGamesByWeek.PlayerWeekScoresJson))) AS LeagueJson
FROM
	public.mt_doc_leaguedata _l
	CROSS JOIN jsonb_array_elements(_l.data->'Weeks') weeks
	INNER JOIN 
	(
		SELECT 
			__l.id,
			weeks->>'WeekNumberRef' AS WeekNumberRef,
			jsonb_agg(jsonb_set(playerWeekScores.value, '{GamesWon}', playerWeekScores->'Points')) AS PlayerWeekScoresJson
		FROM 
			public.mt_doc_leaguedata __l,
			jsonb_array_elements(data->'Weeks') weeks,
			jsonb_array_elements(weeks->'PlayerWeekScores') playerWeekScores
		GROUP BY
			__l.id,
			weeks->>'WeekNumberRef'
		ORDER BY 
			__l.id,
			to_number(weeks->>'WeekNumberRef', '999990')
	) _updatedGamesByWeek ON 
		(
			weeks->>'WeekNumberRef' = _updatedGamesByWeek.WeekNumberRef 
			AND 
			_l.id = _updatedGamesByWeek.id
		)
GROUP BY
	_l.id,
	_l.data



SELECT 
	__l.id,
	weeks->>'WeekNumberRef' AS WeekNumberRef
	--,
	--jsonb_agg(jsonb_set(gameRefs.value, '{WinPoints}', '1')) AS GamesJson
FROM 
	public.mt_doc_leaguedata __l
	CROSS JOIN jsonb_array_elements(data->'Weeks') weeks
	CROSS JOIN jsonb_array_elements(weeks->'Games') gameRefs
WHERE
	__l.data->>'LeagueCode' = 'NeOnYa'
GROUP BY
	__l.id,
	weeks->>'WeekNumberRef'
ORDER BY 
	__l.id,
	to_number(weeks->>'WeekNumberRef', '999990')
	

SELECT data FROM public.mt_doc_leaguedata



SELECT
	_teamLongNames.TeamLongName,
	t.data->>'TeamCode' AS TeamCode
FROM
	(
		SELECT 'Alabama' AS TeamLongName
		UNION SELECT 'Appalachian State'
		UNION SELECT 'Arizona State'
		-- UNION SELECT 'Arkansas State'
		UNION SELECT 'Army'
		UNION SELECT 'Auburn'
		UNION SELECT 'Baylor'
		UNION SELECT 'Boise State'
		UNION SELECT 'Boston College'
		UNION SELECT 'Buffalo'
		UNION SELECT 'BYU'
		UNION SELECT 'California'
		UNION SELECT 'Cincinnati'
		UNION SELECT 'Clemson'
		UNION SELECT 'Duke'
		UNION SELECT 'Eastern Michigan'
		UNION SELECT 'Florida Intl'
		UNION SELECT 'Florida'
		UNION SELECT 'Fresno State'
		UNION SELECT 'Georgia Tech'
		UNION SELECT 'Georgia'
		UNION SELECT 'Georgia Southern'
		UNION SELECT 'Hawaii'
		UNION SELECT 'Houston'
		UNION SELECT 'Iowa'
		UNION SELECT 'Iowa State'
		UNION SELECT 'Kentucky'
		UNION SELECT 'Louisiana Tech'
		UNION SELECT 'Louisiana Lafayette'
		UNION SELECT 'LSU'
		UNION SELECT 'Marshall'
		UNION SELECT 'Memphis'
		UNION SELECT 'Miami'
		UNION SELECT 'Michigan'
		UNION SELECT 'Michigan State'
		UNION SELECT 'Middle Tenn St'
		UNION SELECT 'Minnesota'
		UNION SELECT 'Mississippi State'
		UNION SELECT 'Missouri'
		UNION SELECT 'NC State'
		-- UNION SELECT 'Nevada'
		UNION SELECT 'North Texas'
		UNION SELECT 'Northern Illinois'
		UNION SELECT 'Northwestern'
		UNION SELECT 'Notre Dame'
		UNION SELECT 'Ohio'
		UNION SELECT 'Ohio State'
		UNION SELECT 'Oklahoma'
		UNION SELECT 'Oklahoma State'
		UNION SELECT 'Oregon'
		UNION SELECT 'Penn State'
		UNION SELECT 'Pittsburgh'
		UNION SELECT 'Purdue'
		UNION SELECT 'San Diego State'
		UNION SELECT 'South Carolina'
		UNION SELECT 'South Florida'
		UNION SELECT 'Stanford'
		UNION SELECT 'Syracuse'
		UNION SELECT 'TCU'
		UNION SELECT 'Temple'
		UNION SELECT 'Texas'
		UNION SELECT 'Texas A&M'
		UNION SELECT 'Toledo'
		UNION SELECT 'Troy'
		UNION SELECT 'Tulane'
		UNION SELECT 'UAB'
		UNION SELECT 'UCF'
		UNION SELECT 'Utah'
		UNION SELECT 'Utah State'
		UNION SELECT 'Vanderbilt'
		UNION SELECT 'Virginia'
		UNION SELECT 'Virginia Tech'
		UNION SELECT 'Wake Forest'
		UNION SELECT 'Washington'
		UNION SELECT 'Washington State'
		UNION SELECT 'West Virginia'
		UNION SELECT 'Western Michigan'
		UNION SELECT 'Wisconsin'
	) _teamLongNames
	LEFT OUTER JOIN public.mt_doc_teamdata t ON _teamLongNames.TeamLongName = t.data->>'LongName'
ORDER BY 
	_teamLongNames.TeamLongName


SELECT data->>'LongName', * FROM mt_doc_teamdata WHERE data->>'LongName' LIKE '%Tenn%'

