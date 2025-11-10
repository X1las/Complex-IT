// Program.cs
// Entry point of the application SQL Injection Frontend

// Program.cs defines a simple 'looping' interface
// that asks the end user for input.
// In response to user input,
// the program calls an instance of class QueryConstructor
#nullable enable
using System;

// Welcome message
Console.WriteLine("Welcome to SQL Injection Frontend\n");

// Defining a query constructor
QueryConstructor qConstructor = new QueryConstructor();

// the user interface
string? s = "x";

do {
  Console.Write("Please select character + enter\n"
          + "'d' (dynamic query)\n"
          + "'c' (composed query)\n"
          + "'sc' (safe composed query)\n"
          + "'x' (exit)\n"
          + ">");
  s = Console.ReadLine();
  Console.WriteLine();
  switch (s) {
     case "d":
       qConstructor.dynamicQuery();
       break;
     case "c":
       qConstructor.composedQuery();
       break;
     case "sc":
       qConstructor.safeComposedQuery();
       break;
     case "x": 
       Console.WriteLine("exiting ..");
       break;
     default:
       Console.WriteLine("you typed " + "'" + s + "'" + " -- please use a suggested value");
       break;
   } // end switch
} while (s != "x");

