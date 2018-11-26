-- fix up the ragin' cajens
BEGIN;
UPDATE
	public.mt_doc_teamdata t
SET
	id = 'la-lafayette',
	data = jsonb_set(jsonb_set(jsonb_set(jsonb_set(data, '{theSpreadName}', '"UL Lafayette"'), '{ShortName}', '"Louisiana"'), '{LongName}', '"Louisiana Lafayette"'), '{TeamCode}', '"la-lafayette"')
WHERE
	id = 'lafayette';