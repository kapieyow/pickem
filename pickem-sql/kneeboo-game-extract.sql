--SELECT '['
SELECT 
	'{ "sportCode": "ncaaf",'
	|| '"EspnGameId": ' || CAST(g.data->>'GameId' AS text) || ','
	|| '"SeasonCode": "' || CAST(g.data->>'SeasonCodeRef' AS text) || '",'
	|| '"LastUpdated": "' || CAST(g.data->>'LastUpdated' AS text) || '",'
	|| '"GameState": "' || 
		CASE
			WHEN g.data->>'GameState' = 'SpreadNotSet' THEN 'NotStarted'
			WHEN g.data->>'GameState' = 'SpreadLocked' THEN 'NotStarted'
			WHEN g.data->>'GameState' = 'Cancelled' THEN 'Canceled'
			ELSE g.data->>'GameState'
		END || '",'
	|| '"GameStart": "' || CAST(g.data->>'GameStart' AS text) || '",'
	|| '"CurrentPeriod": "' || COALESCE(CAST(g.data->>'CurrentPeriod' AS text), '') || '",'
	|| '"SpreadDirection": "' || CAST(g.data->'Spread'->>'SpreadDirection' AS text) || '",'
	|| '"SpreadPoints": ' || CAST(g.data->'Spread'->>'PointSpread' AS text) || ','
	|| '"GameTitle": "' || COALESCE(CAST(g.data->>'GameTitle' AS text), '') || '",'
	|| '"AwayCompetitorCode": "' || CAST(g.data->'AwayTeam'->>'TeamCodeRef' AS text) || '",'
	|| '"AwayScore": ' || CAST(g.data->'AwayTeam'->>'Score' AS text) || ','
	|| '"HomeCompetitorCode": "' || CAST(g.data->'HomeTeam'->>'TeamCodeRef' AS text) || '",'
	|| '"HomeScore": ' || CAST(g.data->'HomeTeam'->>'Score' AS text) || ','
	|| '"TimeClock": "' || CAST(g.data->>'TimeClock' AS text) || '" },'	
FROM 
	mt_doc_gamedata g
WHERE
	-- only 2020 season
	g.data->>'SeasonCodeRef' = '20'
--SELECT ']'
