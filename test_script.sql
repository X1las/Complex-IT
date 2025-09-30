\o test_results.txt
\pset pager off
\timing on

-- Pick a sample movie id
SELECT id FROM titles WHERE titletype='movie' LIMIT 1 \gset

-- Ensure clean start
DELETE FROM user_ratings WHERE username='test_user_1' AND title_id=:'id';
DELETE FROM user_history WHERE username='test_user_1';
DELETE FROM bookmarks WHERE username='0' OR username='test_user_1';
DELETE FROM users WHERE username IN ('0','test_user_1');

-- 1) add_user
SELECT * FROM users WHERE username='test_user_1' LIMIT 10;
SELECT add_user('test_user_1','secret_pw');
SELECT * FROM users WHERE username='test_user_1' LIMIT 10;

-- 2) add_bookmark / remove_bookmark  (function takes INT username => use shim "0")
INSERT INTO users(username, pswd) VALUES('0','shim') ON CONFLICT DO NOTHING;
SELECT * FROM bookmarks WHERE title_id=:'id' ORDER BY username LIMIT 10;
SELECT add_bookmark(0, :'id');
SELECT * FROM bookmarks WHERE title_id=:'id' ORDER BY username LIMIT 10;
SELECT remove_bookmark(0, :'id');
SELECT * FROM bookmarks WHERE title_id=:'id' ORDER BY username LIMIT 10;

-- 3) add_rating (fires trigger updating titles.rating/votes)
SELECT id, title, rating, votes FROM titles WHERE id=:'id' LIMIT 10;
SELECT * FROM user_ratings WHERE username='test_user_1' AND title_id=:'id' LIMIT 10;
SELECT add_rating('test_user_1', :'id', 7);
SELECT id, title, rating, votes FROM titles WHERE id=:'id' LIMIT 10;
SELECT * FROM user_ratings WHERE username='test_user_1' AND title_id=:'id' LIMIT 10;

-- 4) update_rating
SELECT * FROM user_ratings WHERE username='test_user_1' AND title_id=:'id' LIMIT 10;
SELECT update_rating('test_user_1', :'id', 9);
SELECT * FROM user_ratings WHERE username='test_user_1' AND title_id=:'id' LIMIT 10;
SELECT id, title, rating, votes FROM titles WHERE id=:'id' LIMIT 10;

-- 5) add_history
SELECT * FROM user_history WHERE username='test_user_1' ORDER BY date_time DESC LIMIT 10;
SELECT add_history('test_user_1', now(), :'id');
SELECT * FROM user_history WHERE username='test_user_1' ORDER BY date_time DESC LIMIT 10;

-- 6) Read-only functions: print small samples
SELECT * FROM string_search('') LIMIT 10;

DO $$
BEGIN
  BEGIN
    PERFORM 1 FROM structured_string_search(NULL,NULL,NULL,NULL) LIMIT 1;
    RAISE NOTICE 'structured_string_search returned rows';
  EXCEPTION WHEN OTHERS THEN
    RAISE NOTICE 'structured_string_search failed: %', SQLERRM;
  END;
END$$;

SELECT * FROM actor_in_movie_search('john') LIMIT 10;

SELECT * FROM movie_search('the') LIMIT 10;

SELECT * FROM co_players('john', 10) LIMIT 10;

SELECT id, full_name, average_rating
FROM crew
WHERE average_rating IS NOT NULL
ORDER BY average_rating DESC NULLS LAST, full_name
LIMIT 10;

SELECT * FROM popular_search('the') LIMIT 10;

SELECT * FROM similar_movie('the') LIMIT 10;

SELECT * FROM person_words('john', 10) LIMIT 10;

SELECT * FROM exact_match_query(ARRAY['love','war']) LIMIT 10;

SELECT * FROM best_match_query(ARRAY['love','war'], 10) LIMIT 10;

SELECT * FROM word_to_words_query(ARRAY['love','war'], 10, TRUE) LIMIT 10;

-- Cleanup shim user
DELETE FROM bookmarks WHERE username='0';
DELETE FROM users WHERE username='0';

\o