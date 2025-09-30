-- Active: 1756753311901@@newtlike.com@5432@rucdb
-- Drop all tables if they exist
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
DROP TABLE IF EXISTS imdb_ratings CASCADE;
DROP TABLE IF EXISTS word_index CASCADE;

-- Create tables
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
    rating DOUBLE PRECISION,
    votes INT,
    PRIMARY KEY (id)
);

CREATE TABLE imdb_ratings (
    titles_id VARCHAR NOT NULL,
    user_rating DOUBLE PRECISION,
    num_user_ratings INT,
    PRIMARY KEY (titles_id),
    FOREIGN KEY (titles_id) REFERENCES titles (id)
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

CREATE TABLE title_posters (
    title_id VARCHAR(10) NOT NULL,
    poster VARCHAR(180) NOT NULL,
    PRIMARY KEY (poster, title_id),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

-- findes ikke atm sep 26 12:22 skal bruges og website skal droppes
CREATE TABLE title_websites (
    title_id VARCHAR(10) NOT NULL,
    website VARCHAR(100) NOT NULL,
    PRIMARY KEY (website, title_id),
    FOREIGN KEY (title_id) REFERENCES titles (id)
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

CREATE TABLE word_index (
    title_id VARCHAR(10) NOT NULL,
    word TEXT NOT NULL,
    field VARCHAR(1) NOT NULL,
    lexeme TEXT,
    PRIMARY KEY (word, title_id, field),
    FOREIGN KEY (title_id) REFERENCES titles (id)
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

INSERT INTO word_index (title_id, word, field, lexeme)
SELECT
    tconst,
    word,
    field,
    lexeme
FROM wi
WHERE tconst IN (SELECT id FROM titles);

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

INSERT INTO imdb_ratings (titles_id, user_rating, num_user_ratings)
SELECT
    tconst,
    averagerating,
    numvotes
FROM title_ratings;

-- Create Functions and Triggers

-- 1.D-1
CREATE OR REPLACE FUNCTION add_user (username VARCHAR(16), password VARCHAR(64)) 
RETURNS VOID AS
$$
BEGIN
    INSERT INTO users (username, pswd)
    VALUES (username, password);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION remove_user (username VARCHAR(16))
RETURNS VOID AS
$$
BEGIN
    DELETE FROM users CASCADE
    WHERE users.username = username;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION add_bookmark (username INT, title_id VARCHAR(10))
RETURNS VOID AS
$$
BEGIN
    INSERT INTO bookmarks (username, title_id)
    VALUES (username, title_id);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION remove_bookmark (username INT, title_id VARCHAR(10))
RETURNS VOID AS
$$
BEGIN
    DELETE FROM bookmarks
    WHERE bookmarks.username = username
      AND bookmarks.title_id = title_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION add_rating (username VARCHAR(16), title_id VARCHAR(10), rating INT)
RETURNS VOID AS
$$
BEGIN
    INSERT INTO user_ratings (username, title_id, rating)
    VALUES (username, title_id, rating);
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION update_rating (username VARCHAR(16), title_id VARCHAR(10), new_rating INT)
RETURNS VOID AS
$$
BEGIN
    UPDATE user_ratings
    SET rating = new_rating
    WHERE user_ratings.username = username
      AND user_ratings.title_id = title_id;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION add_history (username VARCHAR(16), date_time TIMESTAMP, title_id VARCHAR(10))
RETURNS VOID AS
$$
BEGIN
    INSERT INTO user_history (username, date_time, title_id)
    VALUES (username, date_time, title_id);
END;
$$ LANGUAGE plpgsql;

-- 1.D- 2
CREATE OR REPLACE FUNCTION string_search (search_text TEXT) 
RETURNS TABLE (
  id VARCHAR, 
  title VARCHAR
  ) 
AS $$
BEGIN
  RETURN QUERY SELECT
    t.id,
    t.title
  FROM
    titles AS t
  WHERE
    title LIKE'%' || search_text || '%'
    OR plot LIKE'%' || search_text || '%'
  ORDER BY
    t.title ASC;
END;
$$ LANGUAGE plpgsql;

-- 1.D-3
CREATE OR REPLACE FUNCTION update_title_ratings()
RETURNS TRIGGER
LANGUAGE plpgsql AS $$
DECLARE
  v_title TEXT;
BEGIN
  v_title := COALESCE(NEW.title_id, OLD.title_id);
  
  -- Create temporary table
  CREATE TEMP TABLE IF NOT EXISTS avg_ratings (
    title_id VARCHAR PRIMARY KEY,
    combined_ratings DOUBLE PRECISION,
    overall_users_rated INT
  ) ON COMMIT DROP;

  -- Insert calculated ratings
  INSERT INTO avg_ratings (title_id, combined_ratings, overall_users_rated)
  SELECT 
    title_id, 
    ROUND((AVG(rating) + AVG(user_rating))/2,1) AS combined_ratings, 
    COUNT(rating) + AVG(num_user_ratings) AS overall_users_rated
  FROM user_ratings
  LEFT JOIN imdb_ratings on title_id = titles_id
  WHERE title_id = v_title
  GROUP BY title_id;

  -- Update the titles table
  UPDATE titles
  SET rating = combined_ratings,
      votes = overall_users_rated
  FROM avg_ratings
  WHERE titles.id = avg_ratings.title_id
    AND titles.id = v_title;

  RETURN NULL;
END;
$$;
CREATE OR REPLACE TRIGGER update_title_ratings_on_rating
AFTER INSERT ON user_ratings
FOR EACH ROW
EXECUTE PROCEDURE update_title_ratings();

-- 1.D-4
CREATE OR REPLACE FUNCTION structured_string_search ("ti" TEXT, "p" TEXT, "ch" TEXT, "pn" TEXT) 
RETURNS TABLE ("id" VARCHAR, "title" VARCHAR) AS $BODY$
BEGIN
  RETURN QUERY SELECT
    t.id,
    t.title
  FROM
    titles AS t
    JOIN attends a ON t.id = a.title_id
    JOIN crew c ON a.crew_id = c.id
  WHERE
    t IS NULL
    OR t.title ILIKE'%' || ti || '%'
    AND p IS NULL
    OR t.plot ILIKE'%' || p || '%'
    AND ch IS NULL
    OR a.crew_character LIKE'%' || ch || '%'
    AND pn IS NULL
    OR c.full_name ILIKE'%' || pn || '%';
END;
$BODY$ LANGUAGE plpgsql VOLATILE

-- 1.D-5

CREATE
OR REPLACE FUNCTION actor_in_movie_search ("actor" TEXT) RETURNS TABLE ("t.title" VARCHAR) AS $BODY$
BEGIN
  RETURN QUERY SELECT DISTINCT
    t.title
  FROM
    titles AS t
    JOIN attends a ON t.id = a.title_id
    JOIN crew c ON a.crew_id = c.id
  WHERE
    (crew_role = 'actor' OR crew_role = 'actress')
    AND c.full_name LIKE'%' || actor || '%';
END;
$BODY$ LANGUAGE plpgsql VOLATILE

-- 1.D-5

CREATE
OR REPLACE FUNCTION movie_search (movie TEXT) RETURNS TABLE (Actors VARCHAR) 
AS $$
BEGIN
  RETURN QUERY SELECT
    c.full_name
  FROM
    titles AS t
    JOIN attends a ON t.id = a.title_id
    JOIN crew c ON a.crew_id = c.id
  WHERE
    (a.crew_role = 'actor' OR a.crew_role = 'actress')
    AND t.title ILIKE'%' || movie || '%';
END;
$$ LANGUAGE plpgsql VOLATILE

-- 1-D.6
-- view: one row per cast member (actor/actress) per title
CREATE OR REPLACE VIEW title_cast AS
SELECT
    a.title_id,
    a.crew_id,
    c.full_name,
    LOWER(a.crew_role) IN ('actor', 'actress')
    AS is_actor
FROM attends a
JOIN crew c ON a.crew_id = c.id;

CREATE OR REPLACE FUNCTION co_players(actor_name TEXT, max_results INT DEFAULT 20)
RETURNS TABLE (
  n_const VARCHAR, 
  primaryname VARCHAR, 
  frequency INT)
AS $$
WITH target_people AS (
  -- find all crew ids that match the provided name (ILIKE for partial/case-insentitive)
  SELECT id AS crew_id
  FROM crew
  WHERE full_name ILIKE '%' || actor_name || '%'
), 
target_titles AS (
  -- all titles featuring any matching target person
  SELECT DISTINCT title_id
  FROM title_cast
  WHERE crew_id IN (SELECT crew_id FROM target_people)
),
co_actors AS (
  SELECT
    tc.crew_id AS nconst,
    tc.full_name AS primaryname,
    COUNT(DISTINCT tc.title_id) AS frequency
    FROM title_cast tc
    JOIN target_titles tt ON tc.title_id = tt.title_id
    WHERE tc.crew_id NOT IN (SELECT crew_id FROM target_people)
    GROUP BY tc.crew_id, tc.full_name
)
SELECT nconst, primaryname, frequency
FROM co_actors
ORDER BY frequency DESC, primaryname
LIMIT max_results;
$$ LANGUAGE sql;

-- 1.D-7
ALTER TABLE crew ADD COLUMN IF NOT EXISTS average_rating DOUBLE PRECISION;
UPDATE crew
SET average_rating = subquery.avg_rating
FROM (
    SELECT 
        a.crew_role, 
        c.id,
        ROUND(CAST(AVG(mr.user_rating) AS NUMERIC), 2) AS avg_rating
    FROM attends a
    JOIN crew c ON a.crew_id = c.id
    JOIN imdb_ratings mr ON a.title_id = mr.titles_id
    GROUP BY a.crew_role,c.id
    ORDER BY avg_rating DESC
) AS subquery
WHERE crew.id = subquery.id;

-- 1.D-8
CREATE OR REPLACE FUNCTION popular_search (movie TEXT) 
RETURNS TABLE (
  Actors VARCHAR, 
  rating DOUBLE PRECISION
  ) 
AS $$
BEGIN
  RETURN QUERY SELECT
    c.full_name,
    c.average_rating
  FROM
    titles AS t
    JOIN attends a ON t.id = a.title_id
    JOIN crew c ON a.crew_id = c.id
  WHERE
    (a.crew_role = 'actor' OR a.crew_role = 'actress')
    AND t.title ILIKE'%' || movie || '%'
  ORDER BY
    c.average_rating DESC;
END;
$$ LANGUAGE plpgsql VOLATILE

-- 1D-9
CREATE OR REPLACE FUNCTION similar_movie("movie" text)
  RETURNS TABLE("title" varchar, "shared_genres" int4) AS $BODY$
BEGIN
  RETURN QUERY
  SELECT t2.title, COUNT(DISTINCT g2.genre)::integer AS shared_genres
  FROM titles t1
  JOIN title_genres g1 ON t1.id = g1.title_id
  JOIN title_genres g2 ON g1.genre = g2.genre
  JOIN titles t2 ON g2.title_id = t2.id
  WHERE t1.title ILIKE '%' || movie || '%'
  and t1.titletype = 'movie'
  GROUP BY t2.title
  ORDER BY shared_genres DESC
  LIMIT 20;
 
END;
$BODY$
  LANGUAGE plpgsql VOLATILE

-- 1D-10
CREATE OR REPLACE FUNCTION person_words(person_name TEXT, max_results INT DEFAULT 20)
RETURNS TABLE (word TEXT, frequency INT)
LANGUAGE sql AS $$
WITH persons AS (
  SELECT id
  FROM crew
  WHERE full_name ILIKE '%' || person_name || '%'
),
person_titles AS (
  SELECT DISTINCT title_id
  FROM attends
  WHERE crew_id IN (SELECT id FROM persons)
),
title_words AS (
  SELECT LOWER(word_index.word) AS word
  FROM word_index
  JOIN person_titles pt ON word_index.title_id = pt.title_id
)
SELECT word, COUNT(*)::int AS frequency
FROM title_words
GROUP BY word
ORDER BY frequency DESC, word
LIMIT max_results;
$$;

-- 1D-11
CREATE OR REPLACE FUNCTION exact_match_query(keywords TEXT[])
RETURNS TABLE (
  id VARCHAR, 
  title VARCHAR)
AS $$
  WITH kw AS (
    SELECT DISTINCT LOWER(UNNEST(keywords)) AS word
    FROM UNNEST(keywords) as k
    WHERE TRIM(k) IS NOT NULL AND TRIM(k) <> ''
  ),
  kw_count AS (
    SELECT COUNT(*) AS cnt FROM kw
  ),
  matching_titles AS (  
  -- find titles (wi.tconst) that contain all provided keywords (using inverted index)
    SELECT word_index.title_id AS title_id
    FROM word_index
    JOIN kw ON LOWER(word_index.word) = kw.word
    GROUP BY word_index.title_id
    HAVING COUNT(DISTINCT kw.word) = (SELECT cnt FROM kw_count)
  )
  SELECT 
    t.id, 
    t.title
  FROM titles t
  JOIN matching_titles mt ON t.id = mt.title_id
  WHERE (SELECT COUNT(*) FROM kw_count) > 0
  ORDER BY t.title;
$$ LANGUAGE sql;


-- 1D-12
CREATE OR REPLACE FUNCTION best_match_query(keywords TEXT[], max_results INT DEFAULT 20)
RETURNS TABLE (
  id VARCHAR, 
  title VARCHAR, 
  matches INT, 
  total_keywords INT, 
  score DOUBLE PRECISION
)
AS $$
WITH keywords AS (
  -- normalize, trim and deduplicate keywords
  SELECT DISTINCT LOWER(TRIM(k)) AS word
  FROM UNNEST(keywords) as k
  WHERE k is NOT NULL AND TRIM(k) <> ''
),
keywords_count AS (
  SELECT COUNT(*) AS cnt FROM keywords
),
title_matches AS (
  -- count how many distinct keywords match each title via the inverted index
  SELECT word_index.title_id, COUNT(DISTINCT LOWER (keywords.word)) AS matches
  FROM word_index
  JOIN keywords ON LOWER(word_index.word) = keywords.word
  GROUP BY word_index.title_id
)
SELECT
  t.id,
  t.title,
  tm.matches,
  (SELECT cnt FROM keywords_count) AS total_keywords,
  (tm.matches::DOUBLE PRECISION / GREATEST((SELECT cnt FROM keywords_count),1)) AS score
  FROM title_matches tm
  JOIN titles t ON t.id = tm.title_id
  WHERE (SELECT cnt FROM keywords_count) > 0
  ORDER BY tm.matches DESC, score DESC, t.title ASC
  LIMIT max_results;
$$ LANGUAGE sql;

-- 1D-13
CREATE OR REPLACE FUNCTION word_to_words_query(keywords TEXT[], max_results INT DEFAULT 50,
  exclude_query BOOLEAN DEFAULT TRUE
)
RETURNS TABLE (word TEXT, frequency INT)
AS $$
WITH keywords AS (
  -- normalize, trimmed, deduplicated query keywords
  SELECT DISTINCT LOWER(TRIM(k)) AS word
  FROM UNNEST(keywords) as k
  WHERE k is NOT NULL AND TRIM(k) <> ''
),
keywords_count AS (
  SELECT COUNT(*) AS cnt FROM keywords
),
-- titles that contain all provided 
matching_titles AS (
  SELECT word_index.title_id
  FROM word_index
  JOIN keywords ON LOWER(word_index.word) = keywords.word
  GROUP BY word_index.title_id
  HAVING COUNT(DISTINCT keywords.word) = (SELECT cnt FROM keywords_count)
    AND (SELECT cnt FROM keywords_count) > 0
),
-- collect words for all matching titles
words_for_matches AS (
  SELECT LOWER(word_index.word) AS word, word_index.title_id
  FROM word_index
  JOIN matching_titles mt ON word_index.title_id = mt.title_id
)
SELECT 
  words_for_matches.word, 
  COUNT(DISTINCT words_for_matches.title_id)::INT AS frequency
FROM words_for_matches
WHERE NOT (exclude_query AND words_for_matches.word IN (SELECT word FROM keywords))
GROUP BY words_for_matches.word
ORDER BY frequency DESC, word
LIMIT max_results;
$$ LANGUAGE sql;