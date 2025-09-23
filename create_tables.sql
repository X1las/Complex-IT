DROP TABLE IF EXISTS users CASCADE;
DROP TABLE IF EXISTS user_history CASCADE;
DROP TABLE IF EXISTS user_ratings CASCADE;
DROP TABLE IF EXISTS bookmarks CASCADE;
DROP TABLE IF EXISTS titles CASCADE;
DROP TABLE IF EXISTS movie_ratings CASCADE;
DROP TABLE IF EXISTS attends CASCADE;
DROP TABLE IF EXISTS crew CASCADE;  

CREATE TABLE users (
    username VARCHAR(16) NOT NULL,
    pswd VARCHAR(64) NOT NULL,
    usertype VARCHAR(1),
    PRIMARY KEY (username)
);

CREATE TABLE titles (
    id VARCHAR(12) NOT NULL,
    season VARCHAR(80),
    episode VARCHAR(80),
    release_year VARCHAR(100),
    genres VARCHAR(80),
    title VARCHAR(256),
    movie_description TEXT,
    series_id VARCHAR(80),
    awards VARCHAR(80),
    runtime VARCHAR(80),
    movie_language TEXT,
    released VARCHAR(80),
    response VARCHAR(80),
    country VARCHAR(256),
    dvd VARCHAR(80),
    production VARCHAR(80),
    boxoffice VARCHAR(100),
    poster VARCHAR(180),
    website VARCHAR(100),
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

