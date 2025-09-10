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
SELECT takes.course_id, course.title
FROM takes, course
WHERE
    takes.course_id = course.course_id
    AND ID = '30397';

-- Assignment 4
SELECT takes.course_id, course.title, SUM(course.credits)
FROM takes, course
WHERE
    takes.course_id = course.course_id
    AND takes.id = '30397'
GROUP BY
    takes.course_id,
    course.title;

-- Assignment 5
SELECT takes.id, SUM(course.credits) as credits_sum
FROM takes, course
WHERE
    takes.course_id = course.course_id
GROUP BY
    takes.id
HAVING
    SUM(course.credits) > 85;

-- Assignment 6
SELECT student.name
FROM student
    JOIN takes ON student.id = takes.id
    JOIN course ON takes.course_id = course.course_id
WHERE
    course.dept_name = 'Languages'
    AND takes.grade = 'A+';

-- Assignment 7
SELECT id
FROM instructor
WHERE
    id NOT IN (
        SELECT id
        FROM teaches
    )
    AND dept_name = 'Marketing';

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