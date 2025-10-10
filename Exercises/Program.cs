using Npgsql;

var connectionString = "Host=newtlike.com;Port=5432;Database=rucdb;Username=rucdb;Password=testdb;Trust Server Certificate=true";

try
{
    var connection = new NpgsqlConnection(connectionString);
    connection.Open();

    Console.WriteLine("Connected to the database successfully.");

    connection.Close();

}
catch (Exception ex)
{
    Console.WriteLine($"Error connecting to the database: {ex.Message}");
}


Thread.Sleep(1000);

Console.WriteLine("Connection closed.");