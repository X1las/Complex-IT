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
