using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaWebsite.Models
{
    public class COrder
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UniPrice { get; set; }
        public string Image { get; set; }
    }
}
