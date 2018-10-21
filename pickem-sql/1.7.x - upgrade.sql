BEGIN;

-- cut week 15
UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,14}' -- 15th week (zero based) ordinally
;

-- flip week status so synchs will recalc all
UPDATE
	public.mt_doc_gamedata g
SET
	data = jsonb_set(data, '{GameState}', '"InGame"')
WHERE
	g.data->>'GameState' = 'Final'
;
