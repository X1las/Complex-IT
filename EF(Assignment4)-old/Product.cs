using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfExample;
public class Product
{
    public required int productId { get; set; }
    public required string productName { get; set; }
    public required int SupplierId { get; set; }
    public required int CategoryId { get; set; }
    public required string quantityperunit { get; set; }
    public required int UnitPrice { get; set; }
    public required int UnitsInStock { get; set; }
    public required Category Category { get; set; }
}
