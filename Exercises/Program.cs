using Npgsql;
using ExerciseClasses;

/*var connectionString = "Host=newtlike.com;Port=5432;Database=rucdb;Username=rucdb;Password=testdb";

var connection = new NpgsqlConnection(connectionString);
connection.Open();*/

var db = new NorthWindContext();
var attributes = db.Attributes.ToList();
foreach (var attribute in attributes)
{
    Console.WriteLine(attribute.attribute);
}