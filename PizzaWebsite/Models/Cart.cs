using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaWebsite.Models
{
    public class Cart
    {        
        public PizzaList Pizza { get; set; }

        [Key]
        public int OrderId { get; set; }
        public int PizzaId { get; set; }

        public int Quantity { get; set; }
        public decimal UniPrice { get; set; }




    }

}
