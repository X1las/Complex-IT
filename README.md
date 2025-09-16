# Complex IT Systems Repository

## Required Packages

### Latex
- texlive
- texlive-extras
- python3 pygments

### Generate SQL output file

psql -h newtlike.com -p 5432 -d rucdb -U testdb -f assignment2.sql -P pager=off > results.txt