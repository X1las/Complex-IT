-- Drop all tables if they exist
DROP TABLE IF EXISTS users CASCADE;
DROP TABLE IF EXISTS user_history CASCADE;
DROP TABLE IF EXISTS user_ratings CASCADE;
DROP TABLE IF EXISTS bookmarks CASCADE;
DROP TABLE IF EXISTS titles CASCADE;
DROP TABLE IF EXISTS movie_ratings CASCADE;
DROP TABLE IF EXISTS attends CASCADE;
DROP TABLE IF EXISTS crew CASCADE;
DROP TABLE IF EXISTS alternate_titles CASCADE;
DROP TABLE IF EXISTS series CASCADE;
DROP TABLE IF EXISTS episodes CASCADE;
DROP TABLE IF EXISTS games CASCADE;
DROP TABLE IF EXISTS runtimes CASCADE;
DROP TABLE IF EXISTS boxoffice_title CASCADE;
DROP TABLE IF EXISTS productions CASCADE;
DROP TABLE IF EXISTS posters CASCADE;
DROP TABLE IF EXISTS maturity_ratings CASCADE;
DROP TABLE IF EXISTS genres CASCADE;
DROP TABLE IF EXISTS title_genres CASCADE;
DROP TABLE IF EXISTS title_awards CASCADE;
DROP TABLE IF EXISTS languages CASCADE;
DROP TABLE IF EXISTS title_regions CASCADE;
DROP TABLE IF EXISTS attributes CASCADE;
DROP TABLE IF EXISTS attribute_alts CASCADE;
DROP TABLE IF EXISTS dvd_release CASCADE;
DROP TABLE IF EXISTS regions CASCADE;
DROP TABLE IF EXISTS title_websites CASCADE;
DROP TABLE IF EXISTS title_posters CASCADE;
DROP TABLE IF EXISTS production_titles CASCADE;
DROP TABLE IF EXISTS title_maturity_ratings CASCADE;

-- Create tables
CREATE TABLE users (
    username VARCHAR(16) NOT NULL,
    pswd VARCHAR(64) NOT NULL,
    usertype VARCHAR(1),
    PRIMARY KEY (username)
);

CREATE TABLE titles (
    id VARCHAR(12) NOT NULL,
    title VARCHAR(256),
    titletype VARCHAR(20),
    plot TEXT,
    year VARCHAR(100),
    startyear VARCHAR(4),
    endyear VARCHAR(4),
    release_date VARCHAR(80),
    originaltitle TEXT,
    isadult BOOLEAN,
    PRIMARY KEY (id)
);

CREATE TABLE user_history (
    username VARCHAR(16) NOT NULL,
    date_time DATE NOT NULL,
    title_id TEXT NOT NULL,
    PRIMARY KEY (username, date_time),
    FOREIGN KEY (username) REFERENCES users (username),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE user_ratings (
    username VARCHAR(16) NOT NULL,
    title_id TEXT NOT NULL,
    rating INTEGER CHECK (rating BETWEEN 1 AND 10) NOT NULL,
    PRIMARY KEY (username, title_id),
    FOREIGN KEY (username) REFERENCES users (username),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE bookmarks (
    username VARCHAR(16) NOT NULL,
    title_id VARCHAR(10) NOT NULL,
    PRIMARY KEY (username, title_id),
    FOREIGN KEY (username) REFERENCES users (username),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE movie_ratings (
    titles_id VARCHAR NOT NULL,
    user_rating FLOAT,
    num_user_ratings INT,
    critics_rating FLOAT,
    num_critics_ratings INT,
    PRIMARY KEY (titles_id),
    FOREIGN KEY (titles_id) REFERENCES titles (id)
);

-- Needs to be ran to create first instance of this table
CREATE TABLE IF NOT EXISTS rating_history (
  username   VARCHAR(16) NOT NULL,
  title_id   TEXT        NOT NULL,
  rating     INTEGER     NOT NULL CHECK (rating BETWEEN 1 AND 10),
  rated_at   TIMESTAMPTZ NOT NULL DEFAULT now(),
  FOREIGN KEY (username) REFERENCES users (username),
  FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE crew (
    id VARCHAR(10) NOT NULL,
    full_name VARCHAR(256),
    birthyear VARCHAR(4),
    deathyear VARCHAR(4),
    PRIMARY KEY (id)
);

CREATE TABLE attends (
    title_id VARCHAR(10) NOT NULL,
    crew_id VARCHAR(10) NOT NULL,
    crew_role VARCHAR(50) NOT NULL,
    job VARCHAR(256),
    crew_character TEXT,
    PRIMARY KEY (title_id, crew_id, crew_role),
    FOREIGN KEY (title_id) REFERENCES titles (id),
    FOREIGN KEY (crew_id) REFERENCES crew (id)
);

CREATE TABLE alternate_titles (
    title_id VARCHAR(10) NOT NULL,
    alts_ordering INT NOT NULL,
    alts_title VARCHAR(256) NOT NULL,
    types VARCHAR(80),
    isoriginaltitle BOOLEAN,
    PRIMARY KEY (title_id, alts_ordering),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE attributes (
    attribute VARCHAR(80) NOT NULL,
    PRIMARY KEY (attribute)
);

CREATE TABLE attribute_alts (
    title_id VARCHAR(10) NOT NULL,
    alts_ordering INT NOT NULL,
    attribute VARCHAR(80) NOT NULL,
    PRIMARY KEY (title_id, alts_ordering),
    FOREIGN KEY (title_id,alts_ordering) REFERENCES alternate_titles (title_id, alts_ordering),
    FOREIGN KEY (attribute) REFERENCES attributes (attribute)
);

CREATE TABLE series (
    series_id VARCHAR(10) NOT NULL,
    episodes INT,
    seasons INT,
    FOREIGN KEY (series_id) REFERENCES titles (id),
    PRIMARY KEY (series_id)
);

CREATE TABLE episodes (
    episode_id VARCHAR(10) NOT NULL,
    series_id VARCHAR(10) NOT NULL,
    season_number INT,
    episode_number INT,
    PRIMARY KEY (episode_id),
    FOREIGN KEY (series_id) REFERENCES series (series_id)
);

CREATE TABLE runtimes (
    title_id VARCHAR(10) NOT NULL,
    runtime VARCHAR(80) NOT NULL,
    PRIMARY KEY (title_id),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE boxoffice_title (
    title_id VARCHAR(10) NOT NULL,
    box_office VARCHAR(80) NOT NULL,
    PRIMARY KEY (title_id, box_office),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE dvd_release (
    title_id VARCHAR(10) NOT NULL,
    dvd VARCHAR(80) NOT NULL,
    PRIMARY KEY (title_id, dvd),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE productions (
    production VARCHAR(80),
    PRIMARY KEY (production)
);

CREATE TABLE production_titles (
    title_id VARCHAR(10) NOT NULL,
    production VARCHAR(80) NOT NULL,
    PRIMARY KEY (title_id, production),
    FOREIGN KEY (title_id) REFERENCES titles (id),
    FOREIGN KEY (production) REFERENCES productions (production)
);

-- RUN MÆHHHHH
CREATE TABLE title_posters (
    title_id VARCHAR(10) NOT NULL,
    poster VARCHAR(180),
    PRIMARY KEY (poster, title_id)
);

-- findes ikke atm sep 26 12:22 skal bruges og website skal droppes
CREATE TABLE title_websites (
    title_id VARCHAR(10),
    website VARCHAR(100),
    poster VARCHAR(180),
    PRIMARY KEY (website, title_id)
);

CREATE TABLE maturity_ratings (
    maturity_rating VARCHAR(10),
    PRIMARY KEY (maturity_rating)
);

CREATE TABLE title_maturity_ratings (
    title_id VARCHAR(10) NOT NULL,
    maturity_rating VARCHAR(10) NOT NULL,
    PRIMARY KEY (title_id, maturity_rating),
    FOREIGN KEY (title_id) REFERENCES titles (id),
    FOREIGN KEY (maturity_rating) REFERENCES maturity_ratings (maturity_rating)
);

CREATE TABLE genres (
    genre VARCHAR(30) NOT NULL,
    PRIMARY KEY (genre)
);

CREATE TABLE title_genres (
    title_id VARCHAR(10) NOT NULL,
    genre VARCHAR(30) NOT NULL,
    PRIMARY KEY (title_id, genre),
    FOREIGN KEY (title_id) REFERENCES titles (id),
    FOREIGN KEY (genre) REFERENCES genres (genre)
)

CREATE TABLE title_awards (
    title_id VARCHAR(10) NOT NULL,
    awards VARCHAR(64),
    PRIMARY KEY (title_id),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE regions (
    region VARCHAR(30) NOT NULL,
    language VARCHAR(30),
    PRIMARY KEY (region)
);

CREATE TABLE title_regions (
    title_id VARCHAR(10) NOT NULL,
    region VARCHAR(30) NOT NULL,
    PRIMARY KEY (title_id, region),
    FOREIGN KEY (title_id) REFERENCES titles (id),
    FOREIGN KEY (region) REFERENCES regions (region)
);

-- Insert into tables
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
