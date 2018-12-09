BEGIN;

/* move all user to new bowl leagues as their default */

UPDATE 
	public.mt_doc_pickemuser
SET
	data = jsonb_set(data, '{DefaultLeagueCode}', '"BUS-Bowl-18"')
WHERE
	data->>'DefaultLeagueCode' = 'BUS'
;


UPDATE 
	public.mt_doc_pickemuser
SET
	data = jsonb_set(data, '{DefaultLeagueCode}', '"BurlMafia-Bowl-18"')
WHERE
	data->>'DefaultLeagueCode' = 'BurlMafia'
;


UPDATE 
	public.mt_doc_pickemuser
SET
	data = jsonb_set(data, '{DefaultLeagueCode}', '"NeOnYa-Bowl-18"')
WHERE
	data->>'DefaultLeagueCode' = 'NeOnYa'
;