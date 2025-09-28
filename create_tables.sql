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
