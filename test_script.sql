\o 1D_tests_output.txt
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
SELECT * FROM users WHERE username='test_user_1';
SELECT add_user('test_user_1','secret_pw');
SELECT * FROM users WHERE username='test_user_1';

-- 2) add_bookmark / remove_bookmark  (function takes INT username => use shim "0")
INSERT INTO users(username, pswd) VALUES('0','shim') ON CONFLICT DO NOTHING;
SELECT * FROM bookmarks WHERE title_id=:'id' ORDER BY username;
SELECT add_bookmark(0, :'id');
SELECT * FROM bookmarks WHERE title_id=:'id' ORDER BY username;
SELECT remove_bookmark(0, :'id');
SELECT * FROM bookmarks WHERE title_id=:'id' ORDER BY username;

-- 3) add_rating (fires trigger updating titles.rating/votes)
SELECT id, title, rating, votes FROM titles WHERE id=:'id';
SELECT * FROM user_ratings WHERE username='test_user_1' AND title_id=:'id';
SELECT add_rating('test_user_1', :'id', 7);
SELECT id, title, rating, votes FROM titles WHERE id=:'id';
SELECT * FROM user_ratings WHERE username='test_user_1' AND title_id=:'id';

-- 4) update_rating
SELECT * FROM user_ratings WHERE username='test_user_1' AND title_id=:'id';
SELECT update_rating('test_user_1', :'id', 9);
SELECT * FROM user_ratings WHERE username='test_user_1' AND title_id=:'id';
SELECT id, title, rating, votes FROM titles WHERE id=:'id';

-- 5) add_history
SELECT * FROM user_history WHERE username='test_user_1' ORDER BY date_time DESC;
SELECT add_history('test_user_1', now(), :'id');
SELECT * FROM user_history WHERE username='test_user_1' ORDER BY date_time DESC;

-- 6) Read-only functions: print small samples
SELECT * FROM string_search('') LIMIT 5;

DO $$
BEGIN
  BEGIN
    PERFORM 1 FROM structured_string_search(NULL,NULL,NULL,NULL) LIMIT 1;
    RAISE NOTICE 'structured_string_search returned rows';
  EXCEPTION WHEN OTHERS THEN
    RAISE NOTICE 'structured_string_search failed: %', SQLERRM;
  END;
END$$;

SELECT * FROM actor_in_movie_search('john') LIMIT 5;

SELECT * FROM movie_search('the') LIMIT 5;

SELECT * FROM co_players('john', 10);

SELECT id, full_name, average_rating
FROM crew
WHERE average_rating IS NOT NULL
ORDER BY average_rating DESC NULLS LAST, full_name
LIMIT 5;

SELECT * FROM popular_search('the') LIMIT 5;

SELECT * FROM similar_movie('the') LIMIT 5;

SELECT * FROM person_words('john', 10);

SELECT * FROM exact_match_query(ARRAY['love','war']) LIMIT 5;

SELECT * FROM best_match_query(ARRAY['love','war'], 10);

SELECT * FROM word_to_words_query(ARRAY['love','war'], 10, TRUE);

-- Cleanup shim user
DELETE FROM bookmarks WHERE username='0';
DELETE FROM users WHERE username='0';
\o