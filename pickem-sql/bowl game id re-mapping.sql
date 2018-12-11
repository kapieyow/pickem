BEGIN;

-- GAME updates
UPDATE
	public.mt_doc_gamedata g
SET
	id = _gameIdMap.NewGameId::int,
	data = jsonb_set(data, '{GameId}', _gameIdMap.NewGameId::jsonb)
FROM 
	(
		-- This the map between old and new game ids
		-- ====================================================
		SELECT '1' AS OldGameId, '401035253' AS NewGameId
		UNION SELECT '2', '401032054'
		UNION SELECT '3', '401032055'
		UNION SELECT '4', '401032056'
		UNION SELECT '5', '401032057'
		UNION SELECT '6', '401032058'
		UNION SELECT '7', '401032059'
		UNION SELECT '8', '401032060'
		UNION SELECT '9', '401032061'
		UNION SELECT '10', '401032062'
		UNION SELECT '11', '401032063'
		UNION SELECT '12', '401032064'
		UNION SELECT '13', '401032065'
		UNION SELECT '14', '401032066'
		UNION SELECT '15', '401032067'
		UNION SELECT '16', '401032068'
		UNION SELECT '17', '401032069'
		UNION SELECT '18', '401032070'
		UNION SELECT '19', '401032071'
		UNION SELECT '20', '401032072'
		UNION SELECT '21', '401032073'
		UNION SELECT '22', '401032074'
		UNION SELECT '24', '401032075'
		UNION SELECT '25', '401032076'
		UNION SELECT '26', '401032077'
		UNION SELECT '27', '401032079'
		UNION SELECT '28', '401032078'
		UNION SELECT '29', '401032080'
		UNION SELECT '30', '401035251'
		UNION SELECT '31', '401035254'
		UNION SELECT '32', '401032081'
		UNION SELECT '33', '401035250'
		UNION SELECT '34', '401032082'
		UNION SELECT '35', '401032083'
		UNION SELECT '36', '401032088'
		UNION SELECT '37', '401032084'
		UNION SELECT '38', '401032085'
		UNION SELECT '39', '401032086'
		
	) _gameIdMap
WHERE
	g.data->>'GameId' = _gameIdMap.OldGameId
;

	

-- league game REF updates.
UPDATE
	public.mt_doc_leaguedata l
SET
	data = _updatedLeague.LeagueJson
FROM
	(

		SELECT
			_l.id,
			jsonb_set(_l.data, '{Weeks}', jsonb_agg(jsonb_set(weeks.value, '{Games}', _updatedGamesByWeek.GamesJson))) AS LeagueJson
		FROM
			public.mt_doc_leaguedata _l
			CROSS JOIN jsonb_array_elements(_l.data->'Weeks') weeks
			INNER JOIN 
			(
				SELECT 
					__l.id,
					weeks->>'WeekNumberRef' AS WeekNumberRef,
					jsonb_agg(jsonb_set(games.value, '{GameIdRef}', COALESCE(_gameIdMap.NewGameId::jsonb, games->'GameIdRef'))) AS GamesJson
				FROM 
					public.mt_doc_leaguedata __l,
					jsonb_array_elements(data->'Weeks') weeks,
					jsonb_array_elements(weeks->'Games') games
					LEFT OUTER JOIN 
					(
						-- This the map between old and new game ids
						-- ====================================================
						SELECT '1' AS OldGameId, '401035253' AS NewGameId
						UNION SELECT '2', '401032054'
						UNION SELECT '3', '401032055'
						UNION SELECT '4', '401032056'
						UNION SELECT '5', '401032057'
						UNION SELECT '6', '401032058'
						UNION SELECT '7', '401032059'
						UNION SELECT '8', '401032060'
						UNION SELECT '9', '401032061'
						UNION SELECT '10', '401032062'
						UNION SELECT '11', '401032063'
						UNION SELECT '12', '401032064'
						UNION SELECT '13', '401032065'
						UNION SELECT '14', '401032066'
						UNION SELECT '15', '401032067'
						UNION SELECT '16', '401032068'
						UNION SELECT '17', '401032069'
						UNION SELECT '18', '401032070'
						UNION SELECT '19', '401032071'
						UNION SELECT '20', '401032072'
						UNION SELECT '21', '401032073'
						UNION SELECT '22', '401032074'
						UNION SELECT '24', '401032075'
						UNION SELECT '25', '401032076'
						UNION SELECT '26', '401032077'
						UNION SELECT '27', '401032079'
						UNION SELECT '28', '401032078'
						UNION SELECT '29', '401032080'
						UNION SELECT '30', '401035251'
						UNION SELECT '31', '401035254'
						UNION SELECT '32', '401032081'
						UNION SELECT '33', '401035250'
						UNION SELECT '34', '401032082'
						UNION SELECT '35', '401032083'
						UNION SELECT '36', '401032088'
						UNION SELECT '37', '401032084'
						UNION SELECT '38', '401032085'
						UNION SELECT '39', '401032086'
					) _gameIdMap ON games->>'GameIdRef' = _gameIdMap.OldGameId
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
	) _updatedLeague
WHERE
	l.id = _updatedLeague.id
	AND
	l.id IN ('BurlMafia-Bowl-18','BUS-Bowl-18','NeOnYa-Bowl-18')
;








