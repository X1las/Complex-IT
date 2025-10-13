using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfExample;
public class OrderDetails
{
    public required string orderId { get; set; }
    public required int productId { get; set; }
    public decimal unitPrice { get; set; }
    public int quantity { get; set; }
    public float discount { get; set; }
}