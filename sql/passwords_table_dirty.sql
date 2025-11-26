-- Active: 1756753311901@@newtlike.com@5432@university
create table if not exists password (
 username varchar(50),
 salt char(16),
 hashed_password char(64),
 primary key (username)
 );