// using Assignment4;
// using Microsoft.EntityFrameworkCore;

// var db = new NorthwindContext();

// foreach(var cat in db.Categories)
// {
//    Console.WriteLine(cat.Name);
// }

// var query = db.Categories.Where(x => x.Id == 1).Select(x => x.Name);

// foreach (var item in query)
// {
//    Console.WriteLine(item);
// }

// var category = new Category { Id = 8, Name = "UpdatedName", Description = "UpdatedDescription" };

// db.Categories.Add(category);
// db.SaveChanges();

// var categoryUpdate = db.Categories.FirstOrDefault(x => x.Id == 8);
// if (categoryUpdate != null)
// {
//     // Update existing record
//     categoryUpdate.Name = "UpdatedName";
//     categoryUpdate.Description = "UpdatedDescription";
//     db.SaveChanges();  // This updates, not inserts

//     // Remove
//     db.Categories.Remove(categoryUpdate);
//     db.SaveChanges();
// }

// var categoryRemove = db.Categories.Find(8);
// if (categoryRemove != null)
// {
//     db.Categories.Remove(categoryRemove);
//     db.SaveChanges();
// }

// foreach(var p in db.Products.Include(x => x.Category))
// {
//    Console.WriteLine($"{p.Name}: {p.Category?.Name ?? "No Category"}");
// }

// var categoryFirst = db.Categories.Include(x => x.Products).First();

// Console.WriteLine(category.Name);
// Console.WriteLine(category.Products.Count);



