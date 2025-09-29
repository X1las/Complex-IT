-- Active: 1756753311901@@newtlike.com@5432@rucdb
INSERT INTO crew
SELECT DISTINCT
    nconst as id,
    primaryname as full_name,
    birthyear,
    deathyear
FROM name_basics;

INSERT INTO titles
SELECT
    title_basics.tconst,
    title_basics.primarytitle,
    title_basics.titletype,
    plot,
    year,
    startyear,
    endyear,
    released,
    originaltitle,
    isadult
FROM title_basics
FULL OUTER JOIN omdb_data ON title_basics.tconst = omdb_data.tconst;

INSERT INTO series
SELECT
    parenttconst AS series_id,
    COUNT(DISTINCT tconst) AS episodes,
    AVG(DISTINCT seasonnumber) AS seasons
FROM title_episode
WHERE
    parenttconst IS NOT NULL
GROUP BY
    parenttconst;

INSERT INTO episodes
SELECT DISTINCT
    tconst,
    parenttconst,
    seasonnumber,
    episodenumber
FROM title_episode;

INSERT INTO boxoffice_title
SELECT tconst as title_id, boxoffice
FROM omdb_data
WHERE boxoffice != 'N/A'
AND boxoffice IS NOT NULL;

INSERT INTO alternate_titles
SELECT
    titleid,
    ordering,
    title,
    types,
    isoriginaltitle
FROM title_akas;

INSERT INTO runtimes
SELECT 
    tconst, 
    runtimeminutes
FROM title_basics
WHERE runtimeminutes IS NOT NULL;

INSERT INTO dvd_release
SELECT
    tconst, 
    dvd
FROM omdb_data
WHERE dvd != 'N/A'
AND dvd IS NOT NULL
AND dvd != '';

INSERT INTO title_awards
SELECT 
    tconst, 
    awards
FROM omdb_data
WHERE awards != 'N/A'
AND awards IS NOT NULL
AND awards != '';

INSERT INTO regions (region)
SELECT DISTINCT
    region
FROM title_akas
WHERE region IS NOT NULL
AND region != 'N/A';

UPDATE regions
SET language = subquery.language
FROM (
    SELECT DISTINCT
        region, language
    FROM title_akas
    WHERE
        region IS NOT NULL
    AND region != 'N/A'
    ) AS subquery
WHERE regions.region = subquery.region;

INSERT INTO title_regions
SELECT DISTINCT
    titleid, 
    region
FROM title_akas
WHERE region IS NOT NULL
AND region != 'N/A'
AND region != '';

INSERT INTO maturity_ratings
SELECT DISTINCT
    rated
FROM omdb_data
WHERE rated IS NOT NULL
AND rated != 'N/A'
AND rated != '';

INSERT INTO title_maturity_ratings
SELECT DISTINCT
    tconst,
    rated
FROM omdb_data
WHERE rated IS NOT NULL
AND rated != 'N/A'
AND rated != '';

INSERT INTO attributes
SELECT DISTINCT
    attributes
FROM title_akas
WHERE attributes IS NOT NULL;

INSERT INTO
    attribute_alts
SELECT DISTINCT
    titleid,
    ordering,
    attributes
FROM title_akas 
WHERE attributes IS NOT NULL
AND attributes != 'N/A'
AND attributes != '';

INSERT INTO title_posters
SELECT DISTINCT
    tconst,
    poster
FROM omdb_data
WHERE poster IS NOT NULL
AND poster != 'N/A'
AND poster != '';

INSERT INTO productions (production)
SELECT DISTINCT
    production
FROM omdb_data
WHERE production IS NOT NULL
AND production != 'N/A'
AND production != '';

INSERT INTO production_titles (title_id, production)
SELECT DISTINCT
    tconst as title_id,
    production
FROM omdb_data
WHERE production IS NOT NULL
AND production != 'N/A'
AND production != '';

INSERT INTO title_websites
SELECT 
    tconst, 
    website
FROM omdb_data
WHERE website != 'N/A'
AND website IS NOT NULL
AND website != '';

INSERT INTO genres (genre)
SELECT DISTINCT genre
FROM (
    SELECT unnest(string_to_array(genres, ',')) AS genre
    FROM title_basics
) AS split
WHERE genre IS NOT NULL
AND genre != 'N/A'
AND genre != '';

INSERT INTO title_genres (title_id, genre)
SELECT tconst, genre
FROM (
    SELECT tconst, unnest(string_to_array(genres, ',')) AS genre
    FROM title_basics
) AS title_split
WHERE genre IS NOT NULL
AND genre != 'N/A'
AND genre != '';

DO $$
DECLARE
    r RECORD;
    dir VARCHAR(10)[];
    wri VARCHAR(10)[];
    d VARCHAR(10);
    w VARCHAR(10);
BEGIN
    FOR r IN 
        SELECT DISTINCT
            tconst as title_id,
            writers,
            directors
        FROM title_crew
    LOOP
        wri := string_to_array(r.writers, ',');
        dir := string_to_array(r.directors, ',');

        FOREACH d IN ARRAY dir
        LOOP
            IF NOT EXISTS (SELECT id FROM crew WHERE id = trim(d)) THEN
                INSERT INTO crew (id)
                VALUES (trim(d));
            END IF;
            INSERT INTO attends (title_id, crew_id, crew_role)
            VALUES (r.title_id, trim(d), 'director')
            ON CONFLICT DO NOTHING;
        END LOOP;

        FOREACH w IN ARRAY wri
        LOOP
            IF NOT EXISTS (SELECT id FROM crew WHERE id = trim(w)) THEN
                INSERT INTO crew (id)
                VALUES (trim(w));
            END IF;
            INSERT INTO attends (title_id, crew_id, crew_role)
            VALUES (r.title_id, trim(w), 'writer')
            ON CONFLICT DO NOTHING;
        END LOOP;
    END LOOP;

END
$$ language plpgsql;

DROP TABLE IF EXISTS temp_attends;
CREATE TEMP TABLE temp_attends AS
SELECT
    tconst as title_id,
    nconst as crew_id,
    category as crew_role,
    title_principals.job as job,
    characters as crew_character
FROM title_principals
FULL OUTER JOIN attends
ON title_principals.tconst = attends.title_id
AND title_principals.nconst = attends.crew_id
AND title_principals.category = attends.crew_role;

INSERT INTO crew (id)
SELECT DISTINCT ta.crew_id
FROM temp_attends ta
LEFT JOIN crew c ON ta.crew_id = c.id
WHERE ta.crew_id IS NOT NULL
AND c.id IS NULL
ON CONFLICT (id) DO NOTHING;

TRUNCATE TABLE attends;

INSERT INTO attends (title_id, crew_id, crew_role)
SELECT DISTINCT 
    title_id,
    crew_id,
    crew_role
FROM temp_attends
WHERE title_id IS NOT NULL
AND crew_id IS NOT NULL
AND crew_role IS NOT NULL;

UPDATE attends
SET job = ta.job,
    crew_character = ta.crew_character
FROM temp_attends AS ta
WHERE attends.title_id = ta.title_id
AND attends.crew_id = ta.crew_id
AND attends.crew_role = ta.crew_role;

INSERT INTO movie_ratings
SELECT
    tr.tconst as titles_id,
    tr.averagerating as user_rating,
    tr.numvotes as num_user_ratings,
    CAST(om.imdbrating AS double precision) AS critics_rating,
    CAST(replace(om.imdbvotes, ',', '') AS integer) AS num_critics_ratings
FROM title_ratings tr
JOIN omdb_data om ON tr.tconst = om.tconst
WHERE om.imdbrating != 'N/A'
  AND om.imdbvotes != 'N/A';
