-- Active: 1757069828755@@newtlike.com@5432@rucdb
-- Assignment 1
SELECT name FROM instructor where dept_name = 'Biology';

-- Assignment 2
SELECT title
FROM course
where
    dept_name = 'Comp. Sci.'
    and credits = 3;

-- Assignment 3
-- (Solution 1)
SELECT takes.course_id, course.title
FROM takes, course
WHERE
    takes.course_id = course.course_id
    AND ID = '30397';

-- (Solution 2)
SELECT DISTINCT t.course_id, c.title
FROM takes AS t
JOIN course AS c USING (course_id)
WHERE t.id = '30397'
ORDER BY t.course_id;


-- Assignment 4
-- (Solution 1)
SELECT takes.course_id, course.title, SUM(course.credits)
FROM takes, course
WHERE
    takes.course_id = course.course_id
    AND takes.id = '30397'
GROUP BY
    takes.course_id,
    course.title;

-- (Solution 2)
SELECT t.course_id, c.title, SUM(c.credits) AS sum
FROM takes AS t
JOIN course AS c USING (course_id)
WHERE t.id = '30397'
GROUP BY t.course_id, c.title
ORDER BY t.course_id;

-- Assignment 5
-- (Solution 1)
SELECT takes.id, SUM(course.credits) as credits_sum
FROM takes, course
WHERE
    takes.course_id = course.course_id
GROUP BY
    takes.id
HAVING
    SUM(course.credits) > 85;

-- (Solution 2)
SELECT t.id, SUM(c.credits) AS sum
FROM takes AS t
JOIN course AS c USING (course_id)
GROUP BY t.id
HAVING SUM(c.credits) > 85

-- Assignment 6
-- (Solution 1)
SELECT student.name
FROM student
JOIN takes ON student.id = takes.id
JOIN course ON takes.course_id = course.course_id
WHERE course.dept_name = 'Languages'
AND takes.grade = 'A+';

-- (Solution 2)
SELECT DISTINCT s.name
FROM takes AS t
JOIN course AS c USING (course_id)
JOIN student AS s ON s.id = t.id
WHERE c.dept_name = 'Languages'
AND (t.grade) = 'A+'
ORDER BY s.name;

-- Assignment 7
-- (Solution 1)
SELECT id
FROM instructor
WHERE
    id NOT IN (
        SELECT id
        FROM teaches
    )
    AND dept_name = 'Marketing';

-- (Solution 2)
SELECT i.id
FROM instructor AS i
WHERE i.dept_name = 'Marketing'
AND NOT EXISTS (
SELECT id FROM teaches AS te WHERE te.id = i.id)
ORDER BY i.id;

-- Assignment 8
SELECT id, name
FROM instructor
WHERE
    dept_name = 'Marketing'
    AND id NOT IN (
        SELECT id
        FROM teaches
    );

-- Assignment 9
SELECT course_id, sec_id, semester, year, count(id)
FROM takes
WHERE
    year = '2009'
GROUP BY
    sec_id,
    semester,
    year,
    course_id
ORDER BY count(id) DESC;

-- Assignment 10.a
SELECT MAX(sect), min(sect)
FROM (
        SELECT count(id) as sect
        FROM takes
        GROUP BY
            sec_id, course_id
    );

-- Assignment 10.b
WITH
    temp (sect) AS (
        SELECT count(id)
        FROM takes
        GROUP BY
            sec_id,
            course_id
    )
SELECT MAX(sect), min(sect)
FROM temp;

-- Assignment 11 
--11 a
-- Solution 1
SELECT
    course_id,
    sec_id,
    semester,
    year,
    COUNT(id) AS num
FROM takes
GROUP BY
    sec_id,
    course_id,
    semester,
    year
EXCEPT ALL
SELECT sq1.course_id, sq1.sec_id, sq1.semester, sq1.year, sq1.temp_num AS num
FROM (
        SELECT
            course_id, sec_id, semester, year, COUNT(id) AS temp_num
        FROM takes
        GROUP BY
            sec_id, course_id, semester, year
    ) AS sq1, (
        SELECT
            course_id, sec_id, semester, year, COUNT(id) AS temp_num2
        FROM takes
        GROUP BY
            sec_id, course_id, semester, year
    ) AS sq2
WHERE
    sq1.temp_num < sq2.temp_num2;

SELECT
    course_id,
    sec_id,
    semester,
    year,
    COUNT(id) AS num
FROM takes
GROUP BY
    course_id,
    sec_id,
    semester,
    year
HAVING
    COUNT(id) = (
        SELECT MAX(enrollmentCount)
        FROM (
                SELECT COUNT(id) AS enrollmentCount
                FROM takes
                GROUP BY
                    course_id, sec_id, semester, year
            ) AS sub
    );
--solution 2
SELECT course_id, sec_id, semester, year, COUNT(id) AS enroll
FROM takes
GROUP BY course_id, sec_id, semester, year
HAVING COUNT(id) = (SELECT MAX(enrollmentCount)
FROM (SELECT COUNT(id) AS enrollmentCount
        FROM takes
        GROUP BY course_id, sec_id, semester, year
    ) AS sub
); 
--11B

    WITH max_enrollment(id) AS
(SELECT MAX (id)
from takes)
select course_id, sec_id, semester, year
from takes, max_enrollment
where takes.id = max_enrollment.id;
-- Assignment 12
SELECT max(sect), min(sect)
FROM (
        SELECT count(id) as sect
        FROM section
        LEFT OUTER JOIN takes ON section.sec_id = takes.sec_id
        AND section.course_id = takes.course_id
        AND section.year = takes.year
        GROUP BY
            section.sec_id, section.course_id
    );

-- Assignment 13
SELECT DISTINCT
    id,
    course_id,
    sec_id,
    semester,
    year
FROM teaches
WHERE
    id = '19368';

-- Assignment 14
SELECT id 
FROM instructor as I
WHERE NOT EXISTS (
    SELECT course_id
    FROM teaches
    WHERE course_id = '581' OR course_id = '591' OR course_id = '581'
    EXCEPT
    SELECT course_id
    FROM teaches
    WHERE I.id = teaches.id
);

-- Assignment 15
INSERT INTO student (id, name, dept_name, tot_cred)
SELECT i.id, i.name, i.dept_name, 0
FROM instructor AS i
WHERE NOT EXISTS (
SELECT 1 FROM student AS s WHERE s.id = i.id
);

-- Assignment 16
DELETE FROM student s
WHERE EXISTS (SELECT id FROM instructor i WHERE i.id = s.id)
AND NOT EXISTS (
    SELECT id FROM takes t WHERE t.id = s.id
);

-- Assignment 17
SELECT student.id, tot_cred, sum
FROM student
LEFT OUTER JOIN (
    SELECT id, sum (credits) as sum
    FROM takes
    LEFT OUTER JOIN course ON takes.course_id = course.course_id
    GROUP BY id) as temp
ON student.id = temp.id
WHERE tot_cred = sum;

-- Assignment 18
UPDATE student
SET tot_cred = (
    SELECT SUM(credits)
    FROM takes
    LEFT OUTER JOIN course ON takes.course_id = course.course_id
    WHERE takes.id = student.id
    GROUP BY takes.id
);

-- Assignment 19
SELECT student.id, tot_cred, sum
FROM student
LEFT OUTER JOIN (
    SELECT id, sum (credits) as sum
    FROM takes
    LEFT OUTER JOIN course ON takes.course_id = course.course_id
    GROUP BY id) as temp
ON student.id = temp.id
WHERE tot_cred != sum;




