UPDATE 
	public.mt_doc_leaguedata
SET 
	data = jsonb_set(data, array['Weeks'], data->'Weeks' || '{ "Games": [], "WeekNumberRef": 20, "PlayerWeekScores": [{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Cary"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "fvedder"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "msciole"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "kip"}]}')
WHERE 
	id = 'BurlMafia-Bowl-18'
;

UPDATE 
	public.mt_doc_leaguedata
SET 
	data = jsonb_set(data, array['Weeks'], data->'Weeks' || '{ "Games": [], "WeekNumberRef": 20, "PlayerWeekScores": [{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Chris"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "CusheGoblin"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Dharsan"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "DrewWright"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "GingerNinja"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "gumanchew"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Trash Panda"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Beverly Hills Ninja"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Tolowe"}]}')
WHERE 
	id = 'BUS-Bowl-18'
;

UPDATE 
	public.mt_doc_leaguedata
SET 
	data = jsonb_set(data, array['Weeks'], data->'Weeks' || '{ "Games": [], "WeekNumberRef": 20, "PlayerWeekScores": [{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "ali"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Brodie"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "chuckles"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Pop Pop"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "kip"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "poopa"}]}')
WHERE 
	id = 'NeOnYa-Bowl-18'
;
