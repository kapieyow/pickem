BEGIN;

-- move system settings to all leagues
UPDATE 
    public.mt_doc_leaguedata 
SET 
    data = jsonb_set(jsonb_set(jsonb_set(data, '{PickemScoringType}', '"AllWinsOnePoint"'), '{CurrentWeekRef}', '14'), '{NcaaSeasonCodeRef}', '"2018"');

DROP FUNCTION public.mt_insert_systemsettingsdata(doc jsonb, docdotnettype character varying, docid integer, docversion uuid);
DROP FUNCTION public.mt_upsert_systemsettingsdata(doc jsonb, docdotnettype character varying, docid integer, docversion uuid);
DROP TABLE public.mt_doc_systemsettingsdata;

DROP FUNCTION public.mt_insert_seasondata(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid);
DROP FUNCTION public.mt_upsert_seasondata(doc jsonb, docdotnettype character varying, docid character varying, docversion uuid);
DROP TABLE public.mt_doc_seasondata;

-- update all games in all weeks all leagues to have WinPoints = 1
UPDATE
	public.mt_doc_leaguedata l
SET
	data = _updatedLeague.LeagueJson
FROM
	(
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
	) _updatedLeague
WHERE
	l.id = _updatedLeague.id
;


-- update all weekly player scores to in all weeks all leagues to have GamesWon = current points (this works because all existing games are 1 win point)
UPDATE
	public.mt_doc_leaguedata l
SET
	data = _updatedLeague.LeagueJson
FROM
	(
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
	) _updatedLeague
WHERE
	l.id = _updatedLeague.id
;

-- update all season player scores to all leagues to have GamesWon = current points (this works because all existing games are 1 win point)
UPDATE
	public.mt_doc_leaguedata l
SET
	data = _updatedLeague.LeagueJson
FROM
	(
		SELECT
			_l.id,
			jsonb_set(_l.data, '{PlayerSeasonScores}', jsonb_agg(jsonb_set(playerSeasonScores.value, '{GamesWon}', playerSeasonScores->'Points'))) AS LeagueJson
		FROM
			public.mt_doc_leaguedata _l
			CROSS JOIN jsonb_array_elements(_l.data->'PlayerSeasonScores') playerSeasonScores
		GROUP BY
			_l.id,
			_l.data
	) _updatedLeague
WHERE
	l.id = _updatedLeague.id
;