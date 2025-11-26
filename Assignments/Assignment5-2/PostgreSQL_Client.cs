// PostgreSQL_Client.cs
/****************************************************
* class PostgreSQL_Client                           *
*                                                   * 
* An SQL client to a PostgreSQL database            * 
* that accepts query strings                        *
* and prints the result set to the console.         * 
*                                                   * 
* Constructor PostgreSQL_Client(uname, pword, db)   *
* specifies which database the client connects to   *
* as well as the user and password.                 *
*                                                   * 
* Method resultset() prints the result of a query   *
* directly onto the console.                        *
*****************************************************/
#nullable enable
using Npgsql;
using System.Collections.Generic;
using System;

public class PostgreSQL_Client {
  public PostgreSQL_Client (string db, string uname, string pword) {
    string s = "Host=localhost;Port=5432;Username=" + uname
               + ";Password=" + pword
               + ";Database=" + db;
    con = new NpgsqlConnection(s);
    con.Open();
  }
 
  NpgsqlConnection con;

  public void query(string? sql) {
    try {
      NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
      if (cmd == null) Console.WriteLine("Error: database could not execute SQL query: " + sql);
      else {
        NpgsqlDataReader rdr = cmd.ExecuteReader(); 
        int statements = countStatements(rdr);
        // the number of SQL statements in the query, usually one
        for (int s = 0; s < statements; s++) { 
          Table table = new Table(rdr);
          table.print();
          rdr.NextResult(); // advance to result of next SQL statement
        }
        rdr.Close();
      }
    } catch (Exception e) {
      Console.WriteLine("Exception caught by SQL-Injection-Frontend");
      string s = e.ToString().Substring(1,23); // printing only first part of exc message
      Console.WriteLine(s + " ....");
      Console.WriteLine();
    }
  }

  public void query(string? sql, string? name, string? val) {
    Console.WriteLine("Query/3: " + sql + " with " + name + " = " + val); 
    try {
      NpgsqlCommand cmd = new NpgsqlCommand(sql, con);
      cmd.Parameters.Add(new NpgsqlParameter(name, val));
      if (cmd == null) Console.WriteLine("Error: database could not execute SQL query: " + sql);
      else {
        NpgsqlDataReader rdr = cmd.ExecuteReader(); 
        int statements = countStatements(rdr);
        // the number of SQL statements in the query, usually one
        for (int s = 0; s < statements; s++) { 
          Table table = new Table(rdr);
          table.print();
          rdr.NextResult(); // advance to result of next SQL statement
        }
        rdr.Close();
      }
    } catch (Exception e) {
      Console.WriteLine("Exception caught by SQL-Injection-Frontend");
      string s = e.ToString().Substring(1,23); // printing only first part of exc message
      Console.WriteLine(s + " ....");
      Console.WriteLine();
    } 
  }


  private int countStatements(NpgsqlDataReader rdr) {
      // suppress the warning message saying NpgsqlDataReader.Statements is obsolete
      #pragma warning disable 0618  
    int s = rdr.Statements.Count;
      #pragma warning restore 0618
    return s;
  }
} // end class PostgreSQ

/*****************************************************
* class Table                                        *
*                                                    * 
* An instance of class Table                         *
* is the result of a single sql query                *
*                                                    * 
* Normally it is a real table with headers and rows. *
*                                                    * 
* In some cases it has an empty lists of headers     *
* in which case it is interpreted and shown as:      *
* "(Query result is not a table)"                    *
*                                                    * 
******************************************************/

class Table {
  public Table(NpgsqlDataReader rdr) {
    max = new int[rdr.FieldCount];        
    headers = readHeaders(rdr, max);   
    rowList = readRows(rdr, max);
  }

  private string[] headers;
    // column headers 
  private List<string[]> rowList;
    // finally, readiing the actual rows of the query's resultest
  private int[] max;
    // int[] max stores max #chars in a column, for formatting

  // methods for reading data from the NpgsqlDataReader

  private string[] readHeaders(NpgsqlDataReader rdr, int[] max) {
    int columns = max.Length;
    string[] ss = new string[columns];
    for (int c = 0; c < columns; c++) {       
      ss[c] = rdr.GetName(c);  // rdr.GetName(c) returns the name of the c'th column
      int l = ss[c].Length;
      if (l > max[c]) max[c] = l;
    }
    return ss;
  }

  private List<string[]> readRows(NpgsqlDataReader rdr, int[] max) {
    List<string[]> rList = new List<string[]>();
    int columns = max.Length;
    while (rdr.Read()) {
      string[] ss = new string[columns];
      for (int c = 0; c < columns; c++) {                     
        string typestring = rdr.GetFieldType(c).ToString();
          // before reading a field, the type of the field must be determined
        switch (typestring) {
          case "System.String": 
            ss[c] = rdr.GetString(c);
            break;
          case "System.Decimal":
            decimal d = rdr.GetDecimal(c);
            ss[c] = d.ToString();
            break;
          default:
            Console.WriteLine("Unknown field type: " + typestring
                               + " .. field value is defaulted to the empty string");
            ss[c] = "";
            break;
           //  if needed, add cases for System.Int16, System.Int32), System.Int64
        }
        int l = ss[c].Length;
        if (l > max[c]) max[c] = l;
      } // end for
      rList.Add(ss);
    }
    return rList;
  }

  // methods for printing

  public void print() {
    if (headers.Length == 0) Console.WriteLine("(Query result is not a table)");
    else {
      printHeaders();
      int rows = printRecords();
      if (rows == 1) Console.WriteLine("(1 row)");
      else Console.WriteLine("(" + rows + " rows)");
    } 
    Console.WriteLine();
  }

  private void printHeaders() {
    int columns = max.Length;
    for (int c = 0; c < columns; c++) {
      Console.Write(" ");          // a border to the left
      Console.Write(headers[c]);   // the field value
                                   // and then spacing to the right:
      for (int s=0;s<max[c]+1-headers[c].Length;s++) Console.Write(" "); 
      if (c+1 < columns) {         // separate from next column (if any)
        Console.Write("|"); 
      }
    }
    Console.WriteLine("");
    // printing a line of dashes to separate field names from records
    // -----------------------------------------------------
    for (int c = 0; c < columns; c++) {
      for (int d=0;d<max[c]+2;d++) Console.Write('-');
        if (c+1 < columns) Console.Write("+");
    }
    Console.WriteLine("");
  } // end printHeaders
 
  private int printRecords() {
    int rows = rowList.Count;
    int columns = max.Length;
    for (int r = 0; r < rows; r++) {
      string[] row = rowList[r];
      for (int c = 0; c < columns; c++) {       
        Console.Write(" ");
        Console.Write(row[c]);
        for (int s=0;s<max[c]+1-row[c].Length;s++) Console.Write(" "); 
        if (c+1 < columns) Console.Write("|");
      }
      Console.WriteLine("");
    }
    return rows;
  } // end printRecords
}
