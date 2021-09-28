using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PizzaWebsite.Helper;
using PizzaWebsite.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PizzaWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext db;
        public List<PizzaList> p;
        public List<Cart> myCart;
        public HomeController(ILogger<HomeController> logger, DatabaseContext _db)
        {
            _logger = logger;
            db = _db;

        }
       
      
        public IActionResult Selection()
        {

            p = db.Pizza.ToList();
            return View(p);
        }
      
       [HttpPost]
        public JsonResult Selection(int pizzaId)
        {
            var objItem = db.Pizza.Find(pizzaId);
            var cart = SessionHelper.GetObjectFromJson<List<Cart>>(HttpContext.Session, "cart");
         

            if (cart == null)
            {
                cart = new List<Cart>
                {
                    new Cart()
                    {
                        Pizza = objItem,
                        Quantity = 1
                    }
                };
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);  
            }
            else
            {
                var index = Exists(cart, pizzaId);
                if(index==-1)
                {
                    cart.Add(new Cart()
                    {
                        Pizza = objItem,
                        Quantity = 1
                    });
                }
                else
                {
                    var newQuantity = cart[index].Quantity + 1;
                    cart[index].Quantity = newQuantity;

                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);

            }


            return Json(cart);
        }
        private int Exists(List<Cart> cart, int id)
        {
            for(int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Pizza.PizzaID.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }

        [HttpGet]
        public IActionResult Order()
        {
           myCart = SessionHelper.GetObjectFromJson<List<Cart>>(HttpContext.Session, "cart");
            return View(myCart);
        }

        public IActionResult SaveOrder()
        {            
            myCart = SessionHelper.GetObjectFromJson<List<Cart>>(HttpContext.Session, "cart");

            // var order=db.POrder
            foreach(var item in myCart)
            {

                Cart order = new Cart();

                //order.Pizza.PizzaID = item.Pizza.PizzaID;
                order.PizzaId = item.Pizza.PizzaID;
                order.Quantity = item.Quantity;
                order.UniPrice = item.Pizza.Price * item.Quantity; 
                db.POrder.Add(order);
                db.SaveChanges();

            }

            return RedirectToAction("Confirmation");
        }
        [HttpPost]
        public IActionResult Remove(int pizzaID)
        {
            var cart = SessionHelper.GetObjectFromJson<List<Cart>>(HttpContext.Session, "cart");
            var index = Exists(cart, pizzaID);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);

            return RedirectToPage("Order");
        }
        public IActionResult Confirmation()
        {
            IEnumerable<COrder> o = (from order in db.POrder
                                                 join
                                                     item in db.Pizza
                                                     on order.PizzaId equals item.PizzaID
                                                 select new COrder()
                                                 {
                                                     Image = item.Image,
                                                     Name = item.Name,
                                                     Quantity = order.Quantity,
                                                     UniPrice = order.UniPrice,
                                                 }

              ).ToList();

            return View(o);
        }

    }
}
