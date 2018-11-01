-- Trash low worth low level web socket debug logs. Will be over 100K rows
DELETE FROM
	mt_doc_logdata
WHERE
	data->>'LogLevel' = 'Debug'
	AND
	data->>'Component' = 'PickEmServer.WebSockets.SuperWebSocketMiddleware'
;

-- pool manager WS logs
DELETE FROM 
	mt_doc_logdata
WHERE
	data->>'Component' = 'PickEmServer.WebSockets.SuperWebSocketPoolManager'
;

-- events
DELETE FROM 
	mt_doc_logdata
WHERE
	data->>'Component' = 'PickEmServer.Heart.PickemEventer'
;

-- Update NC State's long name so CFP will find them
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{LongName}', '"NC State"') WHERE id = 'north-carolina-st'