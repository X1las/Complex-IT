-- Active: 1756753311901@@newtlike.com@5432@rucdb
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
    ROUND(((COALESCE(AVG(user_rating), 0) + COALESCE(AVG(rating), 0))/2)::NUMERIC, 1) AS combined_ratings, 
    COUNT(rating) + COALESCE(AVG(num_user_ratings), 0) AS overall_users_rated
  FROM user_ratings
  LEFT JOIN imdb_ratings on user_ratings.title_id = imdb_ratings.titles_id
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