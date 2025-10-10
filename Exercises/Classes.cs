using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ExerciseClasses;

public class Attributes
{
    [Key]
     public string attribute { get; set; }
}

public class NorthWindContext : DbContext
{
    public DbSet<Attributes> Attributes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "Host=newtlike.com;Port=5432;Database=rucdb;Username=rucdb;Password=testdb";
        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attributes>().ToTable("attributes");
    }
}