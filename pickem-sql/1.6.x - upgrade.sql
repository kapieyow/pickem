--
-- Name: mt_insert_systemsettingsdata(jsonb, character varying, integer, uuid); Type: FUNCTION; Schema: public; Owner: kip
--

CREATE FUNCTION public.mt_insert_systemsettingsdata(doc jsonb, docdotnettype character varying, docid integer, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
BEGIN
INSERT INTO public.mt_doc_systemsettingsdata ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp());

  RETURN docVersion;
END;
$$;


ALTER FUNCTION public.mt_insert_systemsettingsdata(doc jsonb, docdotnettype character varying, docid integer, docversion uuid) OWNER TO kip;

--
-- Name: mt_update_systemsettingsdata(jsonb, character varying, integer, uuid); Type: FUNCTION; Schema: public; Owner: kip
--

CREATE FUNCTION public.mt_update_systemsettingsdata(doc jsonb, docdotnettype character varying, docid integer, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
  UPDATE public.mt_doc_systemsettingsdata SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp() where id = docId;

  SELECT mt_version FROM public.mt_doc_systemsettingsdata into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


ALTER FUNCTION public.mt_update_systemsettingsdata(doc jsonb, docdotnettype character varying, docid integer, docversion uuid) OWNER TO kip;


--
-- Name: mt_upsert_systemsettingsdata(jsonb, character varying, integer, uuid); Type: FUNCTION; Schema: public; Owner: kip
--

CREATE FUNCTION public.mt_upsert_systemsettingsdata(doc jsonb, docdotnettype character varying, docid integer, docversion uuid) RETURNS uuid
    LANGUAGE plpgsql
    AS $$
DECLARE
  final_version uuid;
BEGIN
INSERT INTO public.mt_doc_systemsettingsdata ("data", "mt_dotnet_type", "id", "mt_version", mt_last_modified) VALUES (doc, docDotNetType, docId, docVersion, transaction_timestamp())
  ON CONFLICT ON CONSTRAINT pk_mt_doc_systemsettingsdata
  DO UPDATE SET "data" = doc, "mt_dotnet_type" = docDotNetType, "mt_version" = docVersion, mt_last_modified = transaction_timestamp();

  SELECT mt_version FROM public.mt_doc_systemsettingsdata into final_version WHERE id = docId ;
  RETURN final_version;
END;
$$;


ALTER FUNCTION public.mt_upsert_systemsettingsdata(doc jsonb, docdotnettype character varying, docid integer, docversion uuid) OWNER TO kip;


--
-- Name: mt_doc_systemsettingsdata; Type: TABLE; Schema: public; Owner: kip
--

CREATE TABLE public.mt_doc_systemsettingsdata (
    id integer NOT NULL,
    data jsonb NOT NULL,
    mt_last_modified timestamp with time zone DEFAULT transaction_timestamp(),
    mt_version uuid DEFAULT (md5(((random())::text || (clock_timestamp())::text)))::uuid NOT NULL,
    mt_dotnet_type character varying
);


ALTER TABLE public.mt_doc_systemsettingsdata OWNER TO kip;

--
-- Name: TABLE mt_doc_systemsettingsdata; Type: COMMENT; Schema: public; Owner: kip
--

COMMENT ON TABLE public.mt_doc_systemsettingsdata IS 'origin:Marten.IDocumentStore, Marten, Version=2.8.2.0, Culture=neutral, PublicKeyToken=null';

--
-- Data for Name: mt_doc_systemsettingsdata; Type: TABLE DATA; Schema: public; Owner: kip
--

COPY public.mt_doc_systemsettingsdata (id, data, mt_last_modified, mt_version, mt_dotnet_type) FROM stdin;
1	{"Id": 1, "SeasonCodeRef": "18", "CurrentWeekRef": 5, "NcaaSeasonCodeRef": "2018"}	2018-09-29 21:00:07.94688-04	016627fd-596a-4e6d-afc8-f0df389b80da	PickEmServer.Data.Models.SystemSettingsData
\.


--
-- Name: pk_mt_doc_systemsettingsdata; Type: CONSTRAINT; Schema: public; Owner: kip
--

ALTER TABLE ONLY public.mt_doc_systemsettingsdata
    ADD CONSTRAINT pk_mt_doc_systemsettingsdata PRIMARY KEY (id);


-- give root "god mode"
UPDATE public.mt_doc_pickemuser SET data = jsonb_set(data, '{IsAGod}', 'true') WHERE data->>'UserName' = 'root';
-- everyone else is NOT a god
UPDATE public.mt_doc_pickemuser SET data = jsonb_set(data, '{IsAGod}', 'false') WHERE data->>'UserName' != 'root';


-- Synch Miami (FL) with NCAA long name 'Miami' instead of 'Miama Florida'
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{LongName}', '"Miami"') WHERE id = 'miami-fl'