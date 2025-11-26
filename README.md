# Complex IT Systems Repository

## Dependencies

- Node.js
- npm
- serve
- git (for linux AutoPull service)
- dotnet-sdk 9
- xunit (included in dotnet 9)
- react-router-dom

## Nuget Packages

- Mapster
    - Version 7.4.0
- Mapster.DependencyInjection
    - Version 1.0.1
- AspNetCore.Authentication.JwtBearer 
    - Version 9.0.10
- AspNetCore.JsonPatch 
    - Version 9.0.10
- AspNetCore.Mvc.NewtonsoftJson 
    Version 9.0.10
- EntityFrameworkCore 
    - Version 9.0.10
- Npgsql.EntityFrameworkCore.PostgreSQL 
    - Version 9.0.4

## Handy SQL commands

Generates output:
psql -h newtlike.com -p 5432 -d rucdb -U rucdb -f DOCUMENT.sql -P pager=off > results.txt 

Creates database:
psql -h newtlike.com -p 5432 -d rucdb -U rucdb -f university_large.sql

Create movies db:
psql -h newtlike.com -p 5432 -d rucdb -U rucdb -f wi_backup.sql
psql -h newtlike.com -p 5432 -d rucdb -U rucdb -f imdb_backup.sql
psql -h newtlike.com -p 5432 -d rucdb -U rucdb -f omdb_data_backup.sql

## Services Overview

- AutoPull: Automatically fetches updates from GitHub and restarts services depending on them
- BackendService: Runs C# server automatically and on startup
- FrontendService: Runs React Server automatically on startup and installs dependencies
- Install/Uninstall: Installs and removes the afforementioned scripts