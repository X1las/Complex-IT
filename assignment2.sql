-- Active: 1757069828755@@newtlike.com@5432@rucdb

-- Question 1
CREATE OR REPLACE FUNCTION course_count (tempid VARCHAR(5))
RETURNS int
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN (SELECT COUNT(DISTINCT course_id) FROM takes WHERE id = tempid);
END;
$$;

SELECT course_count('12345');
SELECT course_count('35');
SELECT id, course_count(id) FROM student;

-- Question 2
CREATE OR REPLACE FUNCTION course_count2 (student_id VARCHAR(5), department_id VARCHAR(12))
RETURNS int
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN (
        SELECT COUNT(DISTINCT course_id) 
        FROM takes 
        NATURAL JOIN course 
        WHERE id = student_id AND dept_name = department_id);
END;
$$;

SELECT course_count2('12345', 'Comp. Sci.');
SELECT id, name, course_count2(id, 'Comp. Sci.') 
FROM student;

-- Question 3
CREATE OR REPLACE FUNCTION course_count_both(student_id VARCHAR(5), department_id VARCHAR(12) DEFAULT NULL)
RETURNS int
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN (
        SELECT COUNT(DISTINCT course_id)
        FROM takes
        NATURAL JOIN course 
        WHERE student_id = id 
        AND (
            department_id IS NULL OR department_id = dept_name
        )
    );
END;
$$;

SELECT id, name, course_count_both(id, 'Comp. Sci.') 
FROM student;

SELECT id, name, course_count_both(id) 
FROM student;

-- Could potentially also be done using IF statements
-- but this is more concise and efficient as it requires one query

-- Question 4
CREATE OR REPLACE FUNCTION department_activities(department_id VARCHAR(12))
RETURNS TABLE(
    instructor_name VARCHAR(100),
    course_title VARCHAR(100),
    semester VARCHAR(6),
    year NUMERIC(4,0)
)
LANGUAGE sql
AS $$
    SELECT instructor.name, course.title, teaches.semester, teaches.year
    FROM instructor
    JOIN teaches ON instructor.id = teaches.id
    JOIN course ON teaches.course_id = course.course_id
    WHERE course.dept_name = department_id;
$$;

SELECT * FROM department_activities('Comp. Sci.');

-- Question 5
CREATE OR REPLACE FUNCTION activities(input VARCHAR(20))
RETURNs TABLE (
    department_name VARCHAR(20),
    instructor_name VARCHAR(100),
    course_title VARCHAR(100),
    semester VARCHAR(6),
    year NUMERIC(4,0)
)
LANGUAGE sql
AS $$
    SELECT department.dept_name, instructor.name, course.title, teaches.semester, teaches.year
    FROM instructor
    JOIN teaches ON instructor.id = teaches.id
    JOIN course ON teaches.course_id = course.course_id
    JOIN department ON course.dept_name = department.dept_name
    WHERE department.dept_name = input OR department.building = input;
$$;

SELECT * FROM activities('Comp. Sci.');
SELECT * FROM department;
SELECT * FROM activities('Chandler');

