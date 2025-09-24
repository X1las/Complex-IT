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
DROP TABLE IF EXISTS boxoffice CASCADE;
DROP TABLE IF EXISTS dvds CASCADE;
DROP TABLE IF EXISTS productions CASCADE;
DROP TABLE IF EXISTS posters CASCADE;
DROP TABLE IF EXISTS websites CASCADE;
DROP TABLE IF EXISTS maturity_ratings CASCADE;
DROP TABLE IF EXISTS genres CASCADE;
DROP TABLE IF EXISTS title_genres CASCADE;
DROP TABLE IF EXISTS title_awards CASCADE;
DROP TABLE IF EXISTS languages CASCADE;
DROP TABLE IF EXISTS title_languages CASCADE;
DROP TABLE IF EXISTS region CASCADE;
DROP TABLE IF EXISTS title_region CASCADE;

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
    rating INTEGER CHECK (
        rating >= 0
        AND rating <= 10
    ) NOT NULL,
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

CREATE TABLE crew (
    id VARCHAR(10) NOT NULL,
    full_name VARCHAR(256) NOT NULL,
    birthyear VARCHAR(4),
    deathyear VARCHAR(4),
    PRIMARY KEY (id)
);

CREATE TABLE attends (
    title_id VARCHAR(10) NOT NULL,
    crew_id VARCHAR(10) NOT NULL,
    crew_role VARCHAR(50) NOT NULL,
    job VARCHAR(80),
    crew_character VARCHAR(80),
    PRIMARY KEY (title_id, crew_id, crew_role),
    FOREIGN KEY (title_id) REFERENCES titles (id),
    FOREIGN KEY (crew_id) REFERENCES crew (id)
);

CREATE TABLE region (
    region VARCHAR (10) NOT NULL,
    PRIMARY KEY (region)
);

CREATE TABLE alternate_titles (
    title_id VARCHAR(10) NOT NULL,
    alts_ordering INT NOT NULL,
    alts_title VARCHAR(256) NOT NULL,
    region VARCHAR(10),
    title_language VARCHAR(10),
    types VARCHAR(80),
    isoriginaltitle BOOLEAN,
    PRIMARY KEY (title_id, alts_ordering),
    FOREIGN KEY (title_id) REFERENCES titles (id),
    FOREIGN KEY (region) REFERENCES region (region)
);

CREATE TABLE series (
    series_id VARCHAR(10) NOT NULL,
    series_title VARCHAR(256) NOT NULL,
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
    PRIMARY KEY (runtime_id, runtime),
    FOREIGN KEY (runtime_id) REFERENCES titles (id)
);

CREATE TABLE boxoffice (
    title_id VARCHAR(10) NOT NULL,
    box_office VARCHAR(80),
    PRIMARY KEY (title_id),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE dvds (
    title_id VARCHAR(10) NOT NULL,
    dvd VARCHAR(80),
    PRIMARY KEY (title_id),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE productions (
    title_id VARCHAR(10) NOT NULL,
    production VARCHAR(80),
    PRIMARY KEY (title_id),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE posters (
    title_id VARCHAR(10) NOT NULL,
    poster VARCHAR(180),
    PRIMARY KEY (title_id),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE websites (
    title_id VARCHAR(10) NOT NULL,
    website VARCHAR(100),
    PRIMARY KEY (title_id),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE maturity_ratings (
    title_id VARCHAR(10) NOT NULL,
    maturity_rating VARCHAR(10),
    PRIMARY KEY (title_id),
    FOREIGN KEY (title_id) REFERENCES titles (id)
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

CREATE TABLE languages (
    language VARCHAR(30) NOT NULL,
    PRIMARY KEY (language)
);

CREATE TABLE title_languages (
    title_id VARCHAR(10) NOT NULL,
    language VARCHAR(30) NOT NULL,
    PRIMARY KEY (title_id, language),
    FOREIGN KEY (title_id) REFERENCES titles (id),
    FOREIGN KEY (language) REFERENCES languages (language)
);

CREATE TABLE title_region (
    title_id VARCHAR (10) NOT NULL,
    region VARCHAR (10) NOT NULL,
    PRIMARY KEY (title_id, region),
    FOREIGN KEY (title_id) REFERENCES titles (id),
    FOREIGN KEY (region) REFERENCES region (region)
);


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

INSERT INTO crew 
SELECT DISTINCT
    nconst as id,
    primaryname as full_name,
    birthyear,
    deathyear
FROM name_basics;

INSERT INTO titles
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
LEFT JOIN temp_omdb 
ON temp_basics.id = temp_omdb.id;

INSERT INTO websites
SELECT 
    tconst as title_id
    website
FROM temp_omdb
where website != 'N/A';

INSERT INTO dvds
SELECT DISTINCT
    title_id,
    dvd
FROM omdb_data

INSERT INTO episodes
SELECT DISTINCT
    tconst,
    parenttconst,
    seasonnumber,
    episodenumber
FROM title_episode

INSERT INTO boxoffice
SELECT DISTINCT
    title_id,
    boxoffice
FROM omdb_data

INSERT INTO alternate_titles
SELECT 
    tconst as id,
    ordering as alts_ordering,
    title as alts_title,
    region,
    language as title_language,
    types
FROM title_akas;

SELECT attributes
FROM title_akas
WHERE attributes IS NOT NULL AND attributes != '';

--INSERT INTO genres lave en do bock med forloob--

insert INTO runtimes
SELECT title_id, runtime
FROM temp_basics;