-- Active: 1757069828755@@newtlike.com@5432@rucdb
SELECT name FROM instructor where dept_name = 'Biology';

SELECT title FROM course where dept_name = 'Comp. Sci.' and credits = 3;

SELECT takes.course_id,course.title FROM takes,course WHERE takes.course_id = course.course_id AND ID = '30397';

SELECT takes.course_id,course.title,SUM (course.credits) 
FROM takes,course 
WHERE takes.course_id = course.course_id 
AND takes.id = '30397' 
GROUP BY takes.course_id,course.title;

SELECT takes.id, SUM (course.credits) as credits_sum
FROM takes,course
WHERE takes.course_id = course.course_id
GROUP BY takes.id
HAVING SUM (course.credits) > 85;

SELECT student.name
FROM student 
JOIN takes ON student.id = takes.id 
JOIN course ON takes.course_id = course.course_id
WHERE course.dept_name = 'Languages' AND takes.grade = 'A+';

SELECT id
FROM instructor
WHERE id 
NOT IN (SELECT id FROM teaches)
AND dept_name = 'Marketing';

SELECT id, name
FROM instructor
WHERE dept_name = 'Marketing'
AND id NOT IN (SELECT id FROM teaches);

SELECT course_id, sec_id, semester, year, count (id)
FROM takes
WHERE year = '2009'
GROUP BY sec_id, semester, year, course_id
ORDER BY count (id) DESC;

SELECT MAX (sect), min (sect)
FROM (SELECT count(id) as sect FROM takes GROUP BY sec_id, course_id);

WITH temp (sect) AS (
    SELECT count(id) 
    FROM takes 
    GROUP BY sec_id, course_id
)
SELECT MAX (sect), min (sect)
FROM temp;