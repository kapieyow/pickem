BEGIN; -- COMMIT ROLLBACK
DELETE FROM mt_doc_leaguedata WHERE id NOT LIKE 'BUS%';
DELETE FROM mt_doc_logdata;
DELETE 
FROM 
	mt_doc_pickemuser u
WHERE 
	NOT EXISTS 
	( 
		SELECT 
			NULL
		FROM 
			public.mt_doc_leaguedata l 
			CROSS JOIN jsonb_array_elements(data->'Players') p
		WHERE 
			p->>'UserNameRef' = u.data->>'UserName' 
	)
	AND
	u.data->>'UserName' != 'root';
	
UPDATE public.mt_doc_pickemuser SET data = jsonb_set(data, '{DefaultLeagueCode}', '"BUS-NCAAF-19"') WHERE data->>'UserName' = 'kip';

UPDATE public.mt_doc_pickemuser SET data = jsonb_set(data, '{PasswordHash}', '"AQAAAAEAACcQAAAAEKGI6XRGDSzcMDsia2QsSjots1V0rIYQbztwvoaBaAO6JunoVpf0GbaBOV7GjmWlpA=="') WHERE data->>'UserName' = 'root';
