-- Active: 1756753311901@@newtlike.com@5432@rucdb
DROP TABLE IF EXISTS users CASCADE;
DROP TABLE IF EXISTS user_ratings CASCADE;
DROP TABLE IF EXISTS user_history CASCADE;
DROP TABLE IF EXISTS bookmarks CASCADE;

CREATE TABLE users (
    username VARCHAR(50) NOT NULL,
    password VARCHAR(256) NOT NULL,
    salt VARCHAR(256) NOT NULL,
    token VARCHAR(512),
    last_login TIMESTAMP,
    PRIMARY KEY (username)
);

CREATE TABLE user_ratings (
    username VARCHAR(50) NOT NULL,
    title_id VARCHAR(12) NOT NULL,
    rating DOUBLE PRECISION NOT NULL,
    PRIMARY KEY (username, title_id),
    FOREIGN KEY (username) REFERENCES users (username),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE user_history (
    username VARCHAR(50) NOT NULL,
    title_id VARCHAR(12) NOT NULL,
    viewed_at TIMESTAMP NOT NULL,
    PRIMARY KEY (username, title_id, viewed_at),
    FOREIGN KEY (username) REFERENCES users (username),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);

CREATE TABLE bookmarks (
    username VARCHAR(50) NOT NULL,
    title_id VARCHAR(12) NOT NULL,
    bookmarked_at TIMESTAMP NOT NULL,
    PRIMARY KEY (username, title_id),
    FOREIGN KEY (username) REFERENCES users (username),
    FOREIGN KEY (title_id) REFERENCES titles (id)
);