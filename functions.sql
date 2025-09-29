-- 1.D- 2
CREATE
OR REPLACE FUNCTION string_search ("search_text" TEXT) RETURNS TABLE ("t.id" VARCHAR, "t.title" VARCHAR) AS $BODY$
BEGIN
  --     INSERT INTO search_history (search_string)
  --     VALUES (search_text);
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
$BODY$ LANGUAGE plpgsql

-- 1.D-3

CREATE OR REPLACE FUNCTION compute_movie_user_rating()
RETURNS TRIGGER
LANGUAGE plpgsql AS $$
DECLARE
    v_title TEXT := (NEW.title_id, OLD.title_id);
    v_avg_rating FLOAT;
    v_cnt_rating INT;
BEGIN
    -- Ensure an aggregrate row exists for this title
    INSERT INTO movie_ratings (titles_id)
    VALUES (v_title)
    ON CONFLICT (titles_id) DO NOTHING;

    -- Compute fresh aggregates from user_ratings
    SELECT AVG(rating)::FLOAT, COUNT(*)
      INTO v_avg_rating, v_cnt_rating
      FROM user_ratings
    WHERE title_id = v_title;

    -- Update the aggregates in movie_ratings
    UPDATE movie_ratings
    SET rating = v_avg_rating,
        num_votes = (v_cnt_rating, 0),
    WHERE titles_id = v_title;

RETURN NULL;
END;
$$;

-- 1.D-4
CREATE
OR REPLACE FUNCTION structured_string_search ("ti" TEXT, "p" TEXT, "ch" TEXT, "pn" TEXT) RETURNS TABLE ("id" VARCHAR, "title" VARCHAR) AS $BODY$
BEGIN
  --     INSERT INTO search_history (search_string)
  --     VALUES (search_text);
  
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
  --     INSERT INTO search_history (search_string)
  --     VALUES (search_text);
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
OR REPLACE FUNCTION movie_search ("movie" TEXT) RETURNS TABLE ("Actors" VARCHAR) AS $BODY$
BEGIN
  --     INSERT INTO search_history (search_string)
  --     VALUES (search_text);
  --     VALUES timestamp;
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
$BODY$ LANGUAGE plpgsql VOLATILE

-- 1-D.6
-- NEEDS A TEST
-- NOT DONE YET
-- CREATE OR REPLACE FUNCTION most_frequent_coplayers_by_name(
--     actor_name VARCHAR
-- )


-- 1.d-8

CREATE
OR REPLACE FUNCTION popular_search ("movie" TEXT) RETURNS TABLE ("Actors" VARCHAR, "rating" FLOAT8) AS $BODY$
BEGIN
  --     INSERT INTO search_history (search_string)
  --     VALUES (search_text);
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
$BODY$ LANGUAGE plpgsql VOLATILE


-- 1D-9

CREATE OR REPLACE FUNCTION similar_movie("movie" text)
  RETURNS TABLE("title" varchar, "shared_genres" int4) AS $BODY$
BEGIN
  --     INSERT INTO search_history (search_string)
  --     VALUES (search_text);

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