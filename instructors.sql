
SELECT * 
FROM instructor;

SELECT dept_name 
FROM instructor;

SELECT * 
FROM instructor
WHERE dept_name = 'Comp. Sci.'
AND salary > 65000;

SELECT DISTINCT dept_name
FROM instructor;