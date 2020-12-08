DELETE FROM public.mt_doc_gamedata g WHERE g.id = '401234572';

UPDATE 
	public.mt_doc_leaguedata
SET 
	data = jsonb_set(data, array['Weeks'], data->'Weeks' || '{"Games": [], "WeekNumberRef": 15, "PlayerWeekScores": [ {"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Virgil"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Poopa"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Brodie"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Ali"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Pickbot 9000 👾"}]}')
WHERE 
	id = 'NeOnYa-NCAAF-20'
;

UPDATE 
	public.mt_doc_leaguedata
SET 
	data = jsonb_set(data, array['Weeks'], data->'Weeks' || '{"Games": [], "WeekNumberRef": 15, "PlayerWeekScores": [ {"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Kip"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Tony"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Pickbot 9000"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Cary"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Frank"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Toby"}]}')
WHERE 
	id = 'BurlMafia-NCAAF-20'
;

UPDATE 
	public.mt_doc_leaguedata
SET 
	data = jsonb_set(data, array['Weeks'], data->'Weeks' || '{"Games": [], "WeekNumberRef": 15, "PlayerWeekScores": [ {"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Kip"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Laine"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Ethan"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Tony"},{"Points": 0,"GamesWon": 0,"GamesLost": 0,"GamesPicked": 0,"GamesPending": 0,"PlayerTagRef": "Pickbot 9000 👾"}]}')
WHERE 
	id = 'BUS-NCAAF-20'
;




