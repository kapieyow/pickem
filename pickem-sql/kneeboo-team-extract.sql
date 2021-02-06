--SELECT '['
SELECT
	'{ "sportCode": "ncaaf",'
	|| '"competitorCode": "' || CAST(teams.data->>'TeamCode' AS text) || '",'
	|| '"shortName": "' || CAST(teams.data->>'ShortName' AS text) || '",'
	|| '"longName": "' || CAST(teams.data->>'LongName' AS text) || '",'
	|| '"icon24FileName": "' || CAST(teams.data->>'icon24FileName' AS text) || '",'	
	|| '"espnAbbreviation": "' || CAST(teams.data->>'EspnAbbreviation' AS text) || '",'	
	|| '"espnDisplayName": "' || CAST(teams.data->>'EspnDisplayName' AS text) || '",'	
	|| '"ncaaNameSeo": "' || CAST(teams.data->>'NcaaNameSeo' AS text) || '",'	
	|| '"yahooCode": "' || CAST(teams.data->>'YahooCode' AS text) || '" }'	
FROM
	public.mt_doc_teamdata teams
ORDER BY 
	teams.id
--SELECT ']'