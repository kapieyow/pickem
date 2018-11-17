BEGIN;

-- move system settings to all leagues
UPDATE 
    public.mt_doc_leaguedata 
SET 
    data = jsonb_set(jsonb_set(jsonb_set(data, '{PickemScoringType}', '"AllWinsOnePoint"'), '{CurrentWeekRef}', '12'), '{NcaaSeasonCodeRef}', '"2018"');

DROP FUNCTION public.mt_insert_systemsettingsdata(doc jsonb, docdotnettype character varying, docid integer, docversion uuid);
DROP FUNCTION public.mt_upsert_systemsettingsdata(doc jsonb, docdotnettype character varying, docid integer, docversion uuid);
DROP TABLE public.mt_doc_systemsettingsdata;