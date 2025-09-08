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
FROM student join takes on student.id = takes.id join course on takes.course_id = course.course_id
WHERE course.dept_name = 'Languages' AND takes.grade = 'A+';

