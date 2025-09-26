-- Active: 1756753311901@@newtlike.com@5432@rucdb
DROP TABLE IF EXISTS temp_basics;

CREATE TEMP TABLE temp_basics AS
SELECT
    tconst as id,
    titletype,
    primarytitle as title,
    originaltitle,
    isadult::BOOLEAN,
    startyear,
    endyear,
    runtimeminutes as runtime,
    genres
FROM title_basics;

DROP TABLE IF EXISTS temp_omdb;

CREATE TEMP TABLE temp_omdb AS
SELECT
    tconst as id,
    episode as episode_number,
    awards,
    plot,
    seriesid as series_id,
    rated as maturity_rating,
    imdbrating as user_rating,
    runtime,
    language,
    released as release_date,
    response,
    writer as writers,
    genre as genres,
    title,
    country,
    dvd,
    production,
    season as season_number,
    type as titletype,
    poster,
    ratings,
    imdbvotes as num_user_ratings,
    boxoffice,
    actors,
    director as directors,
    year,
    website,
    metascore,
    totalseasons as seasons
FROM omdb_data;

INSERT INTO
    crew
SELECT DISTINCT
    nconst as id,
    primaryname as full_name,
    birthyear,
    deathyear
FROM name_basics;

INSERT INTO
    titles
SELECT
    temp_basics.id,
    temp_basics.title,
    temp_basics.titletype,
    plot,
    year,
    startyear,
    endyear,
    release_date,
    originaltitle,
    isadult
FROM temp_basics
    Full OUTER JOIN temp_omdb ON temp_basics.id = temp_omdb.id;

INSERT INTO
    title_websites
SELECT id as title_id, website
FROM temp_omdb
where
    website != 'N/A';

INSERT INTO
    series
SELECT
    parenttconst AS series_id,
    COUNT(DISTINCT tconst) AS episodes,
    AVG(DISTINCT seasonnumber) AS seasons
FROM title_episode
WHERE
    parenttconst IS NOT NULL
GROUP BY
    parenttconst;

INSERT INTO
    episodes
SELECT DISTINCT
    tconst,
    parenttconst,
    seasonnumber,
    episodenumber
FROM title_episode;

INSERT INTO
    boxoffice_title
SELECT tconst as title_id, boxoffice
FROM omdb_data
WHERE
    boxoffice != 'N/A'
    AND boxoffice IS NOT NULL;

INSERT INTO
    alternate_titles
SELECT
    titleid,
    ordering,
    title,
    types,
    isoriginaltitle
FROM title_akas;

insert INTO
    runtimes
SELECT tconst, runtimeminutes
FROM title_basics
WHERE
    runtimeminutes IS NOT NULL;

insert INTO
    dvd_release
SELECT id, dvd
FROM temp_omdb
WHERE
    dvd != 'N/A'
    OR dvd IS NOT NULL;

-- TEST MÆHHH
INSERT INTO
    regions (region)
SELECT DISTINCT
    region
FROM title_akas
WHERE
    region IS NOT NULL
    AND region != 'N/A';

UPDATE regions
SET
    language = subquery.language
FROM (
        SELECT DISTINCT
            region, language
        FROM title_akas
        WHERE
            region IS NOT NULL
            AND region != 'N/A'
    ) AS subquery
WHERE
    regions.region = subquery.region;

INSERT INTO
    attributes
SELECT DISTINCT
    attributes
FROM title_akas
WHERE
    attributes IS NOT NULL;

INSERT INTO
    title_awards
SELECT tconst as title_id, awards
FROM omdb_data
WHERE
    awards != 'N/A'
    AND awards IS NOT NULL;

INSERT INTO
    genres
SELECT DISTINCT g AS genre
FROM omdb_data o,
    regexp_split_to_table(o.genres, '\s*,\s*') AS g
WHERE o.genres IS NOT NULL
AND o.genres <> 'N/A'
AND g <> ''
AND g <> 'N/A';

-- TEST MÆÆHHHHHH (also also also)
INSERT INTO
    title_regions
SELECT 
    titleid as title_id, 
    region as region
FROM title_akas
where
    region IS NOT NULL
    AND genres != 'N/A';

-- TEST MÆÆHÆÆHH
INSERT INTO
    maturity_ratings
SELECT rated as maturity_rating
FROM omdb_data
where
    maturity_rating IS NOT NULL
    AND maturity_rating != 'N/A';

-- TEST MÆÆHÆÆHHHHHHHHH (also also also also)
INSERT INTO
    title_awards
SELECT tconst as title_id, awards
FROM omdb_data
WHERE
    awards != 'N/A'
    AND awards IS NOT NULL;

-- TEST MÆÆHÆÆHHHHHHHHH (also also also also also)
INSERT INTO
    attribute_alts
SELECT DISTINCT
    attributes AS attribute,
    title_id as titleid,
    alts_ordering as ordering
FROM titles_akas
WHERE
    attribute IS NOT NULL;

-- TEST MÆÆHÆÆHHHHHHHHHhhh....Watson (also also also also also also also also also)

INSERT INTO
    title_posters
SELECT DISTINCT
    id as title_id,
    poster,
FROM titles, omdb_data

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
            title_principals.tconst as title_id,
            writers,
            directors,
            category,
            nconst,
            job,
            characters
        FROM title_crew
        FULL OUTER JOIN title_principals
        ON title_crew.tconst = title_principals.tconst
    LOOP
        IF NOT EXISTS (SELECT category FROM title_principals WHERE tconst = r.title_id) THEN
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
        ELSE
            INSERT INTO attends (title_id, crew_id, crew_role, job, crew_character)
            VALUES (
                r.title_id, 
                r.nconst, 
                r.category, 
                r.job, 
                r.characters
            )
            ON CONFLICT DO NOTHING;
        END IF;
    END LOOP;
END
$$ language plpgsql;

SELECT * FROM crew 
WHERE full_name IS NULL;

SELECT crew.full_name
FROM crew
JOIN attends ON crew.id = attends.crew_id
WHERE attends.crew_role = 'director'


