SELECT name FROM instructor where dept_name = 'Biology';

SELECT title FROM course where dept_name = 'Comp. Sci.' and credits = 3;

SELECT takes.course_id,course.title FROM takes,course WHERE takes.course_id = course.course_id AND ID = '30397';

