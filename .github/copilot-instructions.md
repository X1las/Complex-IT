## Quick orientation

This repository contains a mix of SQL datasets, import scripts and a small .NET WebApplication sample. Focus areas for an AI agent:

- Database/schema work: `create_tables.sql`, `university_large.sql`, `university_database.sql`, `university_database.sql` are the canonical SQL assets. The SQL is written for PostgreSQL (psql usage appears in `README.md`).
- Data imports assume staging/temp tables named like `title_basics`, `omdb_data`, `name_basics`, `title_akas`, etc. `create_tables.sql` creates permanent tables then inserts from temp tables.
- Web app: `WebApplication1/` is a minimal ASP.NET project targeting .NET 8.0 (`WebApplication1.csproj`, `Complex-IT.sln`). `Program.cs` currently contains no business logic in this snapshot — treat the webapp as low-priority unless the user requests feature work.

## Contract (what your edits should preserve)
- Inputs: SQL scripts and data files are intended for PostgreSQL `psql -f` imports. Do not change SQL dialect to another DB unless asked.
- Outputs: `create_tables.sql` produces normalized tables (titles, crew, episodes, etc.) and expects downstream ETL to populate temp tables.
- Error modes: large imports may fail on memory/timeouts. Keep changes minimal and add indexes only when requested.

## Important project-specific patterns and examples

- PostgreSQL-first SQL conventions
  - Files use psql-style imports. Example from `README.md`:
    psql -h newtlike.com -p 5432 -d rucdb -U rucdb -f university_large.sql
  - `create_tables.sql` frequently uses CREATE TABLE ... and then INSERT ... SELECT FROM temp_* tables. Avoid refactoring those temp-table flows without checking the upstream data sources.

- Schema conventions
  - Primary keys are frequently single columns named `id` or domain names such as `tconst`/`title_id`/`series_id` (see `create_tables.sql`).
  - Strings use VARCHAR with explicit lengths (e.g., `VARCHAR(256)`). Boolean columns use `BOOLEAN` and numeric ratings use `FLOAT`/`INT` with CHECK constraints (see `user_ratings` in `create_tables.sql`).

- Code layout
  - Web app: `WebApplication1/` holds the .NET project. Use `dotnet build` / `dotnet run` from the folder or solution root.

## Build, run and debug commands (Windows / PowerShell)

PostgreSQL import (small example):

```powershell
# Run a SQL file against a Postgres host (as used in README.md)
psql -h newtlike.com -p 5432 -d rucdb -U rucdb -f .\university_large.sql
```

.NET build / run (solution root):

```powershell
# restore & build
dotnet restore .\Complex-IT.sln ; dotnet build .\Complex-IT.sln -c Debug
# run the web project directly
dotnet run --project .\WebApplication1\WebApplication1.csproj
```

Notes: `Program.cs` is minimal in this snapshot. If asked to add endpoints, wire them into `WebApplication1` and update `appsettings.json` for configuration.

## What to look for when editing
- When modifying SQL: preserve FK relationships visible in `create_tables.sql` (titles -> crew, title_genres -> genres, etc.). If you add or rename columns, update all dependent CREATE/INSERT blocks.
- When adding code to the webapp: prefer small, isolated changes. The project uses default SDK-style .csproj and target `net8.0`.
- Avoid touching large dataset files (e.g. `university_large.sql`) unless explicitly requested — they are probably exports/backups.

## Integration and external dependencies
- PostgreSQL is the runtime DB. The README uses `psql` against host `newtlike.com`. No container or CI is present in the repo snapshot.
- LaTeX build artifacts and PDFs exist; LaTeX is used for reports (see README for texlive notes). This is separate from code.

## Examples of safe, useful tasks an AI can do now
- Improve `create_tables.sql` by adding defensive guards: explicit column types for INSERT ... SELECT flows and id format normalisation.
- Add small helper scripts (PowerShell/Bash) to run the common `psql` import commands from README.
- Add basic README sections describing how to run the .NET project locally and where the canonical SQL lives.

## Files to reference when making changes
- `create_tables.sql` — canonical ETL/import/schema script (primary source of truth for DB layout).
- `university_large.sql`, `university_database.sql` — large data dumps, avoid editing without consent.
- `WebApplication1/` — .NET 8 project; use it for demo endpoints or integration tests.
- `README.md` — contains example `psql` commands (used as authoritative run examples).

If anything here is unclear or you'd like the instructions to call out additional files or workflows, tell me what to include and I'll iterate.
