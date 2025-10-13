using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfExample;
public class Category
{
    public int categoryId { get; set; }
    public string categoryName { get; set; }
    public string categoryDescription { get; set; }
    public ICollection<Product> Products { get; set; }
}
