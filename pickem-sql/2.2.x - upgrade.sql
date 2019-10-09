-- "add" YahooCode "column" to team json data with empty string values for now
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '""'); 
--
--
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"abilene-christian"') WHERE id = 'abilene-christian';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"air-force"') WHERE id = 'air-force';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"akron"') WHERE id = 'akron';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"alabama"') WHERE id = 'alabama';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"alabama-am"') WHERE id = 'alabama-am';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"alabama-st"') WHERE id = 'alabama-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"albany"') WHERE id = 'albany-ny';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"alcorn-st"') WHERE id = 'alcorn-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"appalachian-st"') WHERE id = 'appalachian-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"arizona"') WHERE id = 'arizona';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"arizona-st"') WHERE id = 'arizona-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"arkansas"') WHERE id = 'arkansas';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"arkansas-st"') WHERE id = 'arkansas-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"ar-pine-bluff"') WHERE id = 'ark-pine-bluff';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"army"') WHERE id = 'army';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"auburn"') WHERE id = 'auburn';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"austin-peay"') WHERE id = 'austin-peay';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"ball-st"') WHERE id = 'ball-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"baylor"') WHERE id = 'baylor';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"bethune-cookman"') WHERE id = 'bethune-cookman';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"boise-st"') WHERE id = 'boise-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"boston-coll"') WHERE id = 'boston-college';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"bowling-green"') WHERE id = 'bowling-green';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"bryant"') WHERE id = 'bryant';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"bucknell"') WHERE id = 'bucknell';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"buffalo"') WHERE id = 'buffalo';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"butler"') WHERE id = 'butler';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"byu"') WHERE id = 'byu';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"california"') WHERE id = 'california';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"cal-poly"') WHERE id = 'cal-poly';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"campbell"') WHERE id = 'campbell';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"central-arkansas"') WHERE id = 'central-ark';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"central-conn-st"') WHERE id = 'central-conn-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"cent-michigan"') WHERE id = 'central-mich';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"charleston-sou"') WHERE id = 'charleston-so';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"charlotte"') WHERE id = 'charlotte';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"chattanooga"') WHERE id = 'chattanooga';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"cincinnati"') WHERE id = 'cincinnati';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"citadel"') WHERE id = 'citadel';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"clemson"') WHERE id = 'clemson';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"coastal-car"') WHERE id = 'coastal-caro';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"colgate"') WHERE id = 'colgate';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"colorado"') WHERE id = 'colorado';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"colorado-st"') WHERE id = 'colorado-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"davidson"') WHERE id = 'davidson';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"dayton"') WHERE id = 'dayton';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"delaware"') WHERE id = 'delaware';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"delaware-st"') WHERE id = 'delaware-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"drake"') WHERE id = 'drake';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"duke"') WHERE id = 'duke';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"duquesne"') WHERE id = 'duquesne';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"east-carolina"') WHERE id = 'east-carolina';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"eastern-illinois"') WHERE id = 'eastern-ill';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"e-kentucky"') WHERE id = 'eastern-ky';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"east-michigan"') WHERE id = 'eastern-mich';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"eastern-wash"') WHERE id = 'eastern-wash';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"e-tennessee-st"') WHERE id = 'east-tenn-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"elon"') WHERE id = 'elon';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"florida-intl"') WHERE id = 'fiu';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"florida-atlantic"') WHERE id = 'fla-atlantic';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"florida"') WHERE id = 'florida';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"florida-am"') WHERE id = 'florida-am';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"florida-st"') WHERE id = 'florida-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"fordham"') WHERE id = 'fordham';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"fresno-st"') WHERE id = 'fresno-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"furman"') WHERE id = 'furman';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"gardner-webb"') WHERE id = 'gardner-webb';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"georgia-southern"') WHERE id = 'ga-southern';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"georgetown"') WHERE id = 'georgetown';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"georgia"') WHERE id = 'georgia';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"georgia-st"') WHERE id = 'georgia-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"georgia-tech"') WHERE id = 'georgia-tech';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"grambling-st"') WHERE id = 'grambling';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"hampton"') WHERE id = 'hampton';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"hawaii"') WHERE id = 'hawaii';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"holy-cross"') WHERE id = 'holy-cross';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"houston"') WHERE id = 'houston';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"houston-baptist"') WHERE id = 'houston-baptist';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"howard"') WHERE id = 'howard';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"idaho"') WHERE id = 'idaho';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"idaho-st"') WHERE id = 'idaho-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"illinois"') WHERE id = 'illinois';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"illinois-st"') WHERE id = 'illinois-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"incarnate-word"') WHERE id = 'incarnate-word';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"indiana"') WHERE id = 'indiana';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"indiana-st"') WHERE id = 'indiana-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"iowa"') WHERE id = 'iowa';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"iowa-st"') WHERE id = 'iowa-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"jackson-st"') WHERE id = 'jackson-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"jacksonville"') WHERE id = 'jacksonville';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"jacksonville-st"') WHERE id = 'jacksonville-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"james-madison"') WHERE id = 'james-madison';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"kansas"') WHERE id = 'kansas';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"kansas-st"') WHERE id = 'kansas-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"kennesaw-st"') WHERE id = 'kennesaw-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"kent-st"') WHERE id = 'kent-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"kentucky"') WHERE id = 'kentucky';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"la-lafayette"') WHERE id = 'la-lafayette';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"lamar"') WHERE id = 'lamar';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"la-monroe"') WHERE id = 'la-monroe';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"lehigh"') WHERE id = 'lehigh';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"liberty"') WHERE id = 'liberty';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"louisiana-tech"') WHERE id = 'louisiana-tech';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"louisville"') WHERE id = 'louisville';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"lsu"') WHERE id = 'lsu';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"maine"') WHERE id = 'maine';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"marist"') WHERE id = 'marist';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"marshall"') WHERE id = 'marshall';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"maryland"') WHERE id = 'maryland';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"umass"') WHERE id = 'massachusetts';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"mcneese-st"') WHERE id = 'mcneese-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"memphis"') WHERE id = 'memphis';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"mercer"') WHERE id = 'mercer';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"miami-(fl)"') WHERE id = 'miami-fl';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"miami-(oh)"') WHERE id = 'miami-oh';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"michigan"') WHERE id = 'michigan';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"michigan-st"') WHERE id = 'michigan-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"middle-tenn-st"') WHERE id = 'middle-tenn';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"minnesota"') WHERE id = 'minnesota';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"mississippi-st"') WHERE id = 'mississippi-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"miss-valley-st"') WHERE id = 'mississippi-val';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"missouri"') WHERE id = 'missouri';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"missouri-st"') WHERE id = 'missouri-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"monmouth"') WHERE id = 'monmouth';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"montana"') WHERE id = 'montana';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"montana-st"') WHERE id = 'montana-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"morehead-st"') WHERE id = 'morehead-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"morgan-st"') WHERE id = 'morgan-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"murray-st"') WHERE id = 'murray-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"navy"') WHERE id = 'navy';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"nc-at"') WHERE id = 'nc-at';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"nc-central"') WHERE id = 'nc-central';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"nebraska"') WHERE id = 'nebraska';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"nevada"') WHERE id = 'nevada';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"new-hampshire"') WHERE id = 'new-hampshire';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"new-mexico"') WHERE id = 'new-mexico';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"n-mex-st"') WHERE id = 'new-mexico-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"nicholls"') WHERE id = 'nicholls-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"norfolk-st"') WHERE id = 'norfolk-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"north-alabama"') WHERE id = 'north-ala';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"n-carolina"') WHERE id = 'north-carolina';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"nc-state"') WHERE id = 'north-carolina-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"north-dakota"') WHERE id = 'north-dakota';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"n-dak-st"') WHERE id = 'north-dakota-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"northern-arizona"') WHERE id = 'northern-ariz';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"northern-colorado"') WHERE id = 'northern-colo';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"north-texas"') WHERE id = 'north-texas';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"northwestern"') WHERE id = 'northwestern';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"nwestern-st"') WHERE id = 'northwestern-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"notre-dame"') WHERE id = 'notre-dame';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"ohio"') WHERE id = 'ohio';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"ohio-st"') WHERE id = 'ohio-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"oklahoma"') WHERE id = 'oklahoma';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"oklahoma-st"') WHERE id = 'oklahoma-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"old-dominion"') WHERE id = 'old-dominion';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"mississippi"') WHERE id = 'ole-miss';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"oregon"') WHERE id = 'oregon';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"oregon-st"') WHERE id = 'oregon-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"penn-st"') WHERE id = 'penn-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"pittsburgh"') WHERE id = 'pittsburgh';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"portland-st"') WHERE id = 'portland-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"prairie-view-am"') WHERE id = 'prairie-view';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"presbyterian"') WHERE id = 'presbyterian';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"purdue"') WHERE id = 'purdue';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"rhode-island"') WHERE id = 'rhode-island';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"rice"') WHERE id = 'rice';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"richmond"') WHERE id = 'richmond';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"robert-morris"') WHERE id = 'robert-morris';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"rutgers"') WHERE id = 'rutgers';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"sacramento-st"') WHERE id = 'sacramento-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"sacred-heart"') WHERE id = 'sacred-heart';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"samford"') WHERE id = 'samford';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"sam-houston-st"') WHERE id = 'sam-houston-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"san-diego"') WHERE id = 'san-diego';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"s-diego-st"') WHERE id = 'san-diego-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"san-jose-st"') WHERE id = 'san-jose-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"smu"') WHERE id = 'smu';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"south-alabama"') WHERE id = 'south-ala';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"south-carolina"') WHERE id = 'south-carolina';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"s-carolina-st"') WHERE id = 'south-carolina-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"south-dakota"') WHERE id = 'south-dakota';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"s-dakota-st"') WHERE id = 'south-dakota-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"se-louisiana"') WHERE id = 'southeastern-la';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"se-missouri-st"') WHERE id = 'southeast-mo-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"usc"') WHERE id = 'southern-california';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"southern-ill"') WHERE id = 'southern-ill';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"southern-miss"') WHERE id = 'southern-miss';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"southern"') WHERE id = 'southern-u';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"southern-utah"') WHERE id = 'southern-utah';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"south-florida"') WHERE id = 'south-fla';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"stanford"') WHERE id = 'stanford';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"sf-austin"') WHERE id = 'stephen-f-austin';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"stetson"') WHERE id = 'stetson';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"st-francis-(pa)"') WHERE id = 'st-francis-pa';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"stony-brook"') WHERE id = 'stony-brook';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"syracuse"') WHERE id = 'syracuse';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"tcu"') WHERE id = 'tcu';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"temple"') WHERE id = 'temple';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"tennessee"') WHERE id = 'tennessee';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"tennessee-st"') WHERE id = 'tennessee-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"tennessee-tech"') WHERE id = 'tennessee-tech';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"texas"') WHERE id = 'texas';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"texas-am"') WHERE id = 'texas-am';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"texas-southern"') WHERE id = 'texas-southern';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"texas-st"') WHERE id = 'texas-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"texas-tech"') WHERE id = 'texas-tech';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"toledo"') WHERE id = 'toledo';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"towson"') WHERE id = 'towson';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"troy"') WHERE id = 'troy';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"tulane"') WHERE id = 'tulane';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"tulsa"') WHERE id = 'tulsa';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"uab"') WHERE id = 'uab';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"uc-davis"') WHERE id = 'uc-davis';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"ucf"') WHERE id = 'ucf';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"ucla"') WHERE id = 'ucla';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"connecticut"') WHERE id = 'uconn';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"northern-iowa"') WHERE id = 'uni';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"unlv"') WHERE id = 'unlv';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"utah"') WHERE id = 'utah';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"utah-st"') WHERE id = 'utah-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"utep"') WHERE id = 'utep';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"tenn-martin"') WHERE id = 'ut-martin';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"utsa"') WHERE id = 'utsa';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"valparaiso"') WHERE id = 'valparaiso';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"vanderbilt"') WHERE id = 'vanderbilt';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"villanova"') WHERE id = 'villanova';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"virginia"') WHERE id = 'virginia';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"virginia-tech"') WHERE id = 'virginia-tech';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"virginia-military"') WHERE id = 'vmi';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"wagner"') WHERE id = 'wagner';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"wake-forest"') WHERE id = 'wake-forest';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"washington"') WHERE id = 'washington';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"washington-st"') WHERE id = 'washington-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"weber-st"') WHERE id = 'weber-st';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"western-carolina"') WHERE id = 'western-caro';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"western-ill"') WHERE id = 'western-ill';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"w-kentucky"') WHERE id = 'western-ky';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"w-michigan"') WHERE id = 'western-mich';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"west-virginia"') WHERE id = 'west-virginia';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"william-mary"') WHERE id = 'william-mary';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"wisconsin"') WHERE id = 'wisconsin';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"wofford"') WHERE id = 'wofford';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"wyoming"') WHERE id = 'wyoming';
UPDATE public.mt_doc_teamdata SET data = jsonb_set(data, '{YahooCode}', '"youngstown-st"') WHERE id = 'youngstown-st';