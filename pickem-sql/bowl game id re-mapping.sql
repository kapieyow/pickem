BEGIN;

-- GAME updates
UPDATE
	public.mt_doc_gamedata g
SET
	id = _gameIdMap.NewGameId::int,
	data = jsonb_set(data, '{GameId}', '71711'::jsonb)
FROM 
	(
		-- This the map between old and new game ids
		-- ====================================================
		SELECT '1' AS OldGameId, '71001' AS NewGameId
		UNION SELECT '2', '71002'
		UNION SELECT '3', '71003'
		UNION SELECT '4', '71004'
		UNION SELECT '5', '71005'
		UNION SELECT '6', '71006'
		UNION SELECT '7', '71007'
		UNION SELECT '8', '71008'
		UNION SELECT '9', '71009'
		UNION SELECT '10', '71010'
	) _gameIdMap
WHERE
	g.data->>'GameId' = _gameIdMap.OldGameId


	

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
						SELECT '1' AS OldGameId, '71001' AS NewGameId
						UNION SELECT '2', '71002'
						UNION SELECT '3', '71003'
						UNION SELECT '4', '71004'
						UNION SELECT '5', '71005'
						UNION SELECT '6', '71006'
						UNION SELECT '7', '71007'
						UNION SELECT '8', '71008'
						UNION SELECT '9', '71009'
						UNION SELECT '10', '71010'
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
	)
WHERE
	l.id = _updatedLeague.id
;






