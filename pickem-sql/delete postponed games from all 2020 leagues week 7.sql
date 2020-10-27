BEGIN;

UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,3,Games,14}' -- ZERO based indexes, delete 15th game from 7th week
WHERE
	l.data->>'LeagueCode' = 'BurlMafia-NCAAF-20'
;
UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,3,Games,11}' -- ZERO based indexes, delete 12th game from 7th week 
WHERE
	l.data->>'LeagueCode' = 'BurlMafia-NCAAF-20'
;
UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,3,Games,6}' -- ZERO based indexes, delete 7th game from 7th week 
WHERE
	l.data->>'LeagueCode' = 'BurlMafia-NCAAF-20'
;
UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,3,Games,1}' -- ZERO based indexes, delete 2nd game from 7th week 
WHERE
	l.data->>'LeagueCode' = 'BurlMafia-NCAAF-20'
;


UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,6,Games,14}' -- ZERO based indexes, delete 15th game from 7th week
WHERE
	l.data->>'LeagueCode' = 'BUS-NCAAF-20'
;
UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,6,Games,11}' -- ZERO based indexes, delete 12th game from 7th week 
WHERE
	l.data->>'LeagueCode' = 'BUS-NCAAF-20'
;
UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,6,Games,6}' -- ZERO based indexes, delete 7th game from 7th week 
WHERE
	l.data->>'LeagueCode' = 'BUS-NCAAF-20'
;
UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,6,Games,1}' -- ZERO based indexes, delete 2nd game from 7th week 
WHERE
	l.data->>'LeagueCode' = 'BUS-NCAAF-20'
;


UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,6,Games,14}' -- ZERO based indexes, delete 15th game from 7th week
WHERE
	l.data->>'LeagueCode' = 'NeOnYa-NCAAF-20'
;
UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,6,Games,11}' -- ZERO based indexes, delete 12th game from 7th week 
WHERE
	l.data->>'LeagueCode' = 'NeOnYa-NCAAF-20'
;
UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,6,Games,6}' -- ZERO based indexes, delete 7th game from 7th week 
WHERE
	l.data->>'LeagueCode' = 'NeOnYa-NCAAF-20'
;
UPDATE
	public.mt_doc_leaguedata l
SET 
	data = l.data #- '{Weeks,6,Games,1}' -- ZERO based indexes, delete 2nd game from 7th week 
WHERE
	l.data->>'LeagueCode' = 'NeOnYa-NCAAF-20'
;