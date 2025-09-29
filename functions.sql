CREATE
OR REPLACE FUNCTION "public"."actor_in_movie_search" ("actor" TEXT) RETURNS TABLE ("t.title" VARCHAR) AS $BODY$
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



  
CREATE
OR REPLACE FUNCTION "public"."movie_search" ("movie" TEXT) RETURNS TABLE ("Actors" VARCHAR) AS $BODY$
BEGIN
  --     INSERT INTO search_history (search_string)
  --     VALUES (search_text);
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


CREATE OR REPLACE FUNCTION "public"."similar_movie"("movie" text)
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




CREATE
OR REPLACE FUNCTION "public"."string_search" ("search_text" TEXT) RETURNS TABLE ("t.id" VARCHAR, "t.title" VARCHAR) AS $BODY$
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
