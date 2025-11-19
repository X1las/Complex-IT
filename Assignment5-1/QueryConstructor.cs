// QueryConstructor.cs
#nullable enable
using System;

public class QueryConstructor
{

  public QueryConstructor()
  {
    client = new PostgreSQL_Client("university", "rucdb", "testdb");
    // retain university database
    // but change username and password
  }

  public PostgreSQL_Client client;

  // method definitions

  // dynamicQuery()
  // Get user-defined query, send query to db

  public void dynamicQuery()
  {
    // defining the query 
    Console.Write("Please type any SQL query: ");
    string? sql = Console.ReadLine();

    // printing query string to console
    Console.Write("Query to be executed: ");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(sql + "\n");
    Console.ForegroundColor = ConsoleColor.Black;

    // executing query
    client.query(sql);
  } // end method dynamicQuery() 

  // composedQuery()
  // Get dynamic part from user,
  // then compose dynamic and static part,
  // and send query to db

  virtual public void composedQuery()
  {
    // defining the query
    string staticSQLbefore = "select * from course where course_id like '%";
    Console.Write("Please type id of a course, or part of id: ");
    string? user_defined = Console.ReadLine();
    string staticSQLafter = "' and dept_name != 'Biology'";
    string sql = staticSQLbefore + user_defined + staticSQLafter;

    // printing query string to console
    Console.Write("Query to be executed: " + staticSQLbefore);
    Console.ForegroundColor = ConsoleColor.Red;
    Console.Write(user_defined);
    Console.ForegroundColor = ConsoleColor.Black;
    Console.WriteLine(staticSQLafter + "\n");

    // executing query
    client.query(sql);
  }

  // safeComposedQuery()
  // Get user input and safely call the stored function safe_course()
  // using separate parameter passing (query/3)
  // This prevents SQL injection attacks

  virtual public void safeComposedQuery()
  {
    // define the query using the stored function with parameter placeholder
    string sql = "select * from safe_course(@course_id)";
    
    Console.Write("Please type id of a course: ");
    string? user_defined = Console.ReadLine();

    // printing query string to console
    Console.Write("Query to be executed: ");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(sql + " where @course_id = " + user_defined + "\n");
    Console.ForegroundColor = ConsoleColor.Black;

    // executing query with separate parameter passing (safe from SQL injection)
    client.query(sql, "@course_id", user_defined);
  }
}
