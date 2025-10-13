using EfExample;
using Microsoft.EntityFrameworkCore;

var db = new NorthwindContext();

foreach(var cat in db.Categories)
{
   Console.WriteLine(cat.Name);
}

var query = db.Categories.Where(x => x.Id == 1).Select(x => x.Name);

foreach (var item in query)
{
   Console.WriteLine(item);
}

var category = new Category { Id = 9, Name="testing", Description = "blah blah" };

db.Categories.Add(category);
db.SaveChanges();

var categoryUpdate = db.Categories.FirstOrDefault(x => x.Id == 9);
category.Name = "updated";

db.SaveChanges();

var categoryRemove = db.Categories.FirstOrDefault(x => x.Id == 9);
db.Categories.Remove(db.Categories.Find(9));
db.SaveChanges();

foreach(var p in db.Products.Include(x => x.Category))
{
   Console.WriteLine($"{p.Name}: {p.Category.Name}");
}

var categoryFirst = db.Categories.Include(x => x.Products).First();

Console.WriteLine(category.Name);
Console.WriteLine(category.Products.Count);



