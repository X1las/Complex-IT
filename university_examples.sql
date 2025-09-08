
-- Getting everything
SELECT * 
FROM instructor;

-- Distinct keyword example
SELECT DISTINCT dept_name
FROM instructor;

-- Specific select
SELECT dept_name 
FROM instructor;

-- Specific select with conditions
SELECT * 
FROM instructor
WHERE dept_name = 'Comp. Sci.'
AND salary > 65000;

-- Join operation without keyword, inner join by default
SELECT name,course_id
FROM instructor, teaches
WHERE instructor.ID = teaches.ID;

-- Join operation with DISTINCT keyword
SELECT DISTINCT name,course_id
FROM instructor, teaches
WHERE instructor.ID = teaches.ID;

-- Natural joing operation
SELECT *
FROM instructor
NATURAL JOIN teaches;

-- Multi-part natural join
SELECT *
FROM student 
NATURAL JOIN takes
NATURAL JOIN course;

-- Example of self-join
SELECT DISTINCT T.NAME
FROM instructor AS T, instructor as S
WHERE T.salary > S.salary
AND S.dept_name = 'Comp. Sci.';

-- Rewrite of rename
SELECT s2.supervisor 
FROM super s1, super s2
WHERE s1.person = 'Bob'
AND s1.supervisor = s2.person;

-- Like example, uses regular expressions
SELECT name
FROM instructor
WHERE name LIKE '%r%';

SELECT name
FROM instructor
WHERE name LIKE '_i%';

-- Tuple comparison example
SELECT name, course_id
FROM instructor, teaches
WHERE (instructor.ID, teaches.course_id) = ('10101', 'CS-101');

SELECT name FROM instructor where name LIKE '%A%'
INTERSECT
SELECT name FROM instructor WHERE name LIKE '%a%';

SELECT dept_name, avg_salary
FROM (
    SELECT dept_name, AVG(salary) AS avg_salary
    FROM instructor
    GROUP BY dept_name
)
WHERE avg_salary > 42000;

-- Database modification keywords
-- DELETE
-- INSERT
-- UPDATE
-- DROP

SELECT *
FROM course
NATURAL JOIN takes;

SELECT *
FROM course
INNER JOIN takes ON course.course_id = takes.course_id;

SELECT *
FROM course
FULL OUTER JOIN takes ON course.course_id = takes.course_id;

SELECT *
FROM course
LEFT JOIN takes ON course.course_id = takes.course_id;

SELECT * 
FROM course
RIGHT JOIN takes ON course.course_id = takes.course_id;

SELECT *
FROM course
NATURAL LEFT OUTER JOIN prereq;

SELECT *
FROM course
NATURAL RIGHT OUTER JOIN prereq;

SELECT *
FROM course
NATURAL FULL OUTER JOIN prereq;

SELECT title, prereq_id
FROM course
JOIN prereq USING (course_id);