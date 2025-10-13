using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

// Instantiates Order from DB

namespace EfExample;

public class Order
{
    public required int orderid { get; set; }
    public required string customerid { get; set; }
    public int employeeid { get; set; }
    public DateOnly orderdate { get; set; }
    public DateOnly requiredate { get; set; }
    public DateOnly shippeddate { get; set; }
    public int freight { get; set; }
    public required string shipname { get; set; }
    public required string shipaddress { get; set; }
    public string shipcity { get; set; }
    public string shippostalcode { get; set; }
    public string shipcountry { get; set; }

    public ICollection<OrderDetails> OrderDetails { get; set; }

}