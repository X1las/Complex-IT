DROP TABLE IF EXISTS users CASCADE;
DROP TABLE IF EXISTS user_history CASCADE;
DROP TABLE IF EXISTS bookmarks CASCADE;
DROP TABLE IF EXISTS user_ratings CASCADE;
DROP TABLE IF EXISTS rating_history CASCADE;

CREATE TABLE users (
    username VARCHAR(16) NOT NULL,
    pswd VARCHAR(64) NOT NULL,
    PRIMARY KEY (username)
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
    rating INTEGER CHECK (rating BETWEEN 1 AND 10) NOT NULL,
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

CREATE TABLE IF NOT EXISTS rating_history (
  title_id   TEXT        NOT NULL,
  username   VARCHAR(16) NOT NULL,
  rated_at   TIMESTAMPTZ NOT NULL DEFAULT now(),
  PRIMARY KEY (username, title_id, rated_at),
  FOREIGN KEY (username) REFERENCES users(username) ON DELETE CASCADE,
  FOREIGN KEY (title_id) REFERENCES titles(id) ON DELETE CASCADE
);

