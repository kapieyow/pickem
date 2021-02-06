--SELECT '['
SELECT
	'{ "sportCode": "ncaaf",'
	|| '"competitorCode": "' || CAST(teams.data->>'TeamCode' AS text) || '",'
	|| '"statDate": "' || 
		CASE
			WHEN weekStats->>'WeekNumberRef' = '1' THEN '2020-09-01T00:00:00Z'
			WHEN weekStats->>'WeekNumberRef' = '2' THEN '2020-09-08T00:00:00Z'
			WHEN weekStats->>'WeekNumberRef' = '3' THEN '2020-09-15T00:00:00Z'
			WHEN weekStats->>'WeekNumberRef' = '4' THEN '2020-09-22T00:00:00Z'
			WHEN weekStats->>'WeekNumberRef' = '5' THEN '2020-09-29T00:00:00Z'
			WHEN weekStats->>'WeekNumberRef' = '6' THEN '2020-10-06T00:00:00Z'
			WHEN weekStats->>'WeekNumberRef' = '7' THEN '2020-10-13T00:00:00Z'
			WHEN weekStats->>'WeekNumberRef' = '8' THEN '2020-10-20T00:00:00Z'
			WHEN weekStats->>'WeekNumberRef' = '9' THEN '2020-10-27T00:00:00Z'
			WHEN weekStats->>'WeekNumberRef' = '10' THEN '2020-11-03T00:00:00Z'
			WHEN weekStats->>'WeekNumberRef' = '11' THEN '2020-11-10T00:00:00Z'
			WHEN weekStats->>'WeekNumberRef' = '12' THEN '2020-11-17T00:00:00Z'
			WHEN weekStats->>'WeekNumberRef' = '13' THEN '2020-11-24T00:00:00Z'
			WHEN weekStats->>'WeekNumberRef' = '14' THEN '2020-12-01T00:00:00Z'
			WHEN weekStats->>'WeekNumberRef' = '15' THEN '2020-12-08T00:00:00Z'
		END || '",'
	|| '"rank": "' || CAST(weekStats->>'FbsRank' AS text) || '",'
	|| '"wins": "' || CAST(weekStats->>'Wins' AS text) || '",'	
	|| '"losses": "' || CAST(weekStats->>'Losses' AS text) || '" }'	
FROM
	public.mt_doc_teamdata teams,
	jsonb_array_elements(data->'Seasons') seasons,
	jsonb_array_elements(seasons->'WeekStats') weekStats
WHERE
	-- limit to 2020
	seasons->>'SeasonCodeRef' = '20'
ORDER BY 
	teams.id
--SELECT ']'
