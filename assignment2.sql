-- Active: 1757069828755@@newtlike.com@5432@rucdb
-- Group 3
-- Jonas, Malthe, Joachim

-- Question 1 solution
CREATE OR REPLACE FUNCTION course_count (tempid VARCHAR(5))
RETURNS int
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN (SELECT COUNT(DISTINCT course_id) FROM takes WHERE id = tempid);
END;
$$;

-- Alternative solution
create or replace function course_count_alt(sid varchar(8))
returns integer as $$
declare
    cnt integer;
begin
    select count(*) into cnt
    from takes
    where id = sid;
    return cnt;
end;
$$ language plpgsql;

-- Test Queries
-- Student id 12345 exists in the small dataset, but not the large one
SELECT course_count ('12345');
-- Test query from assignment
SELECT id, course_count (id) FROM student;

SELECT id, course_count_alt (id) FROM student;

-- Question 2 solution
CREATE OR REPLACE FUNCTION course_count2(student_id VARCHAR(5), department_id VARCHAR(12))
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

-- Test Queries
SELECT course_count2 ('12345', 'Comp. Sci.');

SELECT id, name, course_count2 (id, 'Comp. Sci.') FROM student;

-- Question 3 solution
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

-- Alternative solution using IF statements
CREATE OR REPLACE FUNCTION course_count_both2("student_id" VARCHAR(20), "dept" VARCHAR(20) DEFAULT NULL)
RETURNS int AS 
$BODY$
    BEGIN
        IF dept IS NULL THEN
            RETURN (course_count(student_id));
        ELSE RETURN(course_count2(student_id, dept));
        END if;
    END;
$BODY$
LANGUAGE plpgsql;

-- Test Queries
SELECT id, name, course_count_both (id, 'Comp. Sci.') FROM student;

SELECT id, name, course_count_both (id) FROM student;

SELECT id, name, course_count_both2 (id) FROM student;

-- Seeing as we have already defined methods of getting the course count with and without a department,
-- we can use those functions to simplify our code.
-- This also avoids code duplication, however the first solution is not reliant on any functions being defined beforehand

-- Question 4 solution
CREATE OR REPLACE FUNCTION department_activities(department_id VARCHAR(12))
RETURNS TABLE(
    instructor_name VARCHAR(20),
    course_title VARCHAR(50),
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

-- Test Queries
SELECT * FROM department_activities ('Comp. Sci.');

-- Question 5 solution
CREATE OR REPLACE FUNCTION activities(input VARCHAR(20))
RETURNS TABLE (
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

-- Test Queries
SELECT * FROM activities ('Comp. Sci.') LIMIT 10;

SELECT * FROM department LIMIT 10;

SELECT * FROM activities ('Watson') LIMIT 10;

-- Question 6 solution
CREATE OR REPLACE FUNCTION followed_courses_by(student_name VARCHAR(20))
RETURNS TEXT
LANGUAGE plpgsql
AS
$$
DECLARE 
rec TEXT DEFAULT '';
temp record;
cur CURSOR FOR 
    SELECT DISTINCT instructor.name 
    FROM instructor
    JOIN teaches ON instructor.id = teaches.id
    JOIN takes ON takes.course_id = teaches.course_id
    JOIN student ON student.id = takes.id
    WHERE student.name = student_name;
BEGIN
    OPEN cur;
    LOOP FETCH cur INTO temp;
        EXIT WHEN NOT FOUND;
        IF rec = '' THEN
            rec := temp;
        ELSE
            rec := rec || ', ' || temp;
        END IF;
    END LOOP;
    CLOSE cur;
    RETURN rec;
END;
$$;

-- Test Queries
SELECT followed_courses_by ('Shankar');

select name, followed_courses_by (name) from student;

-- Question 7 solution
CREATE OR REPLACE FUNCTION followed_courses_by_2(student_name VARCHAR(20))
RETURNS TEXT
LANGUAGE plpgsql
AS
$$ 
DECLARE 
rec record;
ret text := '';
BEGIN
    FOR rec IN SELECT DISTINCT instructor.name as instructor_name
        FROM instructor
        JOIN teaches ON instructor.id = teaches.id
        JOIN takes ON takes.course_id = teaches.course_id
        JOIN student ON student.id = takes.id
        WHERE student.name = student_name
    LOOP
        IF ret = '' THEN
            ret := rec.instructor_name;
        ELSE
            ret := ret || ', ' || rec.instructor_name;
        END IF;
    END LOOP;

    RETURN ret;
END;
$$;

-- Test Queries
select followed_courses_by_2 ('Shankar');

select name, followed_courses_by_2 (name) from student;

-- Question 8 solution
CREATE OR REPLACE FUNCTION followed_courses_by_3(student_name VARCHAR(20))
RETURNS TEXT
LANGUAGE plpgsql
AS
$$
BEGIN
    RETURN (SELECT string_agg(instructor_name, ', ') FROM (SELECT DISTINCT instructor.name as instructor_name
        FROM instructor
        JOIN teaches ON instructor.id = teaches.id
        JOIN takes ON takes.course_id = teaches.course_id
        JOIN student ON student.id = takes.id
        WHERE student.name = student_name));
END;
$$;

-- Test Queries
select followed_courses_by_3 ('Shankar');

select name, followed_courses_by_3 (name) from student;

-- Question 9 solution
CREATE OR REPLACE FUNCTION followed_courses_by_4(student_name VARCHAR(20))
RETURNS TEXT
LANGUAGE plpgsql
AS
$$
BEGIN
    RETURN (
        SELECT string_agg(instructor_name, ', ') 
        FROM (
            (
                SELECT DISTINCT instructor.name as instructor_name
                FROM advisor
                JOIN student ON advisor.s_id = student.id
                JOIN instructor ON advisor.i_id = instructor.id
                WHERE student.name = student_name
            )
            UNION 
            (
                SELECT DISTINCT instructor.name as instructor_name
                FROM instructor
                JOIN teaches ON instructor.id = teaches.id
                JOIN takes ON takes.course_id = teaches.course_id
                JOIN student ON student.id = takes.id
                WHERE student.name = student_name
            )
        )
    ); 
END;
$$;

-- Test Queries
SELECT followed_courses_by_3 ('Levy');

SELECT followed_courses_by_4 ('Glaho');

-- Question 10
ALTER TABLE student ADD teachers TEXT;

UPDATE student SET teachers = followed_courses_by_4 (name);

SELECT * FROM student;

CREATE OR REPLACE FUNCTION update_student()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE student
    SET teachers = followed_courses_by_4(name) 
    WHERE id = NEW.id;
    RETURN NEW;
END;
$$;

CREATE OR REPLACE FUNCTION update_student_advisors()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE student
    SET teachers = followed_courses_by_4(name) 
    WHERE id = NEW.s_id;
    RETURN NEW;
END;
$$;

CREATE OR REPLACE TRIGGER update_student_teachers
AFTER INSERT ON takes
FOR EACH ROW
EXECUTE PROCEDURE update_student();

CREATE OR REPLACE TRIGGER update_student_teachers_on_advisor
AFTER INSERT ON advisor
FOR EACH ROW
EXECUTE PROCEDURE update_student_advisors();

-- Test Queries
insert into
    takes
values (
        '12345',
        'BIO-101',
        '1',
        'Summer',
        '2017',
        'A'
    );

insert into
    takes
values (
        '12345',
        'HIS-351',
        '1',
        'Spring',
        '2018',
        'B'
    );

insert into advisor values ('54321', '32343');

insert into advisor values ('55739', '76543');

select id, name, teachers, followed_courses_by_4 (name) from student;