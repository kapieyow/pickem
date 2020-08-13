BEGIN;
UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,14,Games,6}' -- ZERO based indexes, delete 7th game from 15th week ... which was army/navy and not really in the same week
WHERE
	l.data->>'LeagueCode' = 'BUS-NCAAF-19'
;
UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,14,Games,6}' -- ZERO based indexes, delete 7th game from 15th week ... which was army/navy and not really in the same week
WHERE
	l.data->>'LeagueCode' = 'BurlMafia-NCAAF-19'
;
UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,14,Games,6}' -- ZERO based indexes, delete 7th game from 15th week ... which was army/navy and not really in the same week
WHERE
	l.data->>'LeagueCode' = 'NeOnYa-NCAAF-19'
;
UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,6,Games,6}' -- ZERO based indexes, delete 7th game from 15th week which is only the 6th in this league ... which was army/navy and not really in the same week
WHERE
	l.data->>'LeagueCode' = 'BOTS-NCAAF-19'
;

