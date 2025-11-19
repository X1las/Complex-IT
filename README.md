# Complex IT Systems Repository

## Notes

### SQL Output Files

Generates output:
psql -h newtlike.com -p 5432 -d rucdb -U rucdb -f DOCUMENT.sql -P pager=off > results.txt 

Creates database:
psql -h newtlike.com -p 5432 -d rucdb -U rucdb -f university_large.sql

Create movies db:
psql -h newtlike.com -p 5432 -d rucdb -U rucdb -f wi_backup.sql
psql -h newtlike.com -p 5432 -d rucdb -U rucdb -f imdb_backup.sql
psql -h newtlike.com -p 5432 -d rucdb -U rucdb -f omdb_data_backup.sql

### C-sharp
dotnet-sdk 9
Xunit

Githook Filler Text
