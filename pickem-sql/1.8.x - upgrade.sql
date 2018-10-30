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