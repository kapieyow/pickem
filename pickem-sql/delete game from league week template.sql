/*
    indexes are zero based
    - e.g. week 12 is 11 idx (usually)
    Mafia league in 20 didn't start until week 4 so
        - real week -4 = idx for Mafia
*/

-- Get Game Id
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
	g.data->>'SeasonCodeRef' = '20'
	AND
	g.data->>'WeekNumberRef' = '13'
ORDER BY 
	g.data->'HomeTeam'->>'TeamCodeRef'

-- e.g. 401236031

-- get league json to find where game id is (index in week)
SELECT 
	jsonb_pretty(l.data)
FROM 
	public.mt_doc_leaguedata l
WHERE   
    l.data->>'LeagueCode' = 'BUS-NCAAF-20'

/*
!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
 run deletes

    indexes are zero based
    - e.g. week 12 is 11 idx (usually)
    Mafia league in 20 didn't start until week 4 so
        - real week -4 = idx for Mafia
!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
*/

BEGIN;

UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,12,Games,14}'
WHERE
	l.data->>'LeagueCode' = 'BUS-NCAAF-20'
;

UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,12,Games,14}' 
WHERE
	l.data->>'LeagueCode' = 'NeOnYa-NCAAF-20'
;

UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,9,Games,14}' -- MAFIA weeks are 3 less than others because started later
WHERE
	l.data->>'LeagueCode' = 'BurlMafia-NCAAF-20'
;