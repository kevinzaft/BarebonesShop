using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class HomeController : Controller
    {
        private static List<Product> products;
        private static List<Product> cart = new List<Product>();

        public ActionResult Index()
        {
            var t = getWasteInfoFromJSON();
            var json = JsonConvert.SerializeObject(t);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetItem(int id)
        {
            var t = getWasteInfoFromJSON().Where(x => x.id == id).First();
            var json = JsonConvert.SerializeObject(t);
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult BuyItem(int id)
        {
            var t = getWasteInfoFromJSON().Where(x => x.id == id).First();
            if (t.count <= 0)
            {
                return Json("this item is soldout", JsonRequestBehavior.AllowGet);
            }
            else
            {
                t.count--;
                var json = JsonConvert.SerializeObject(t);
                return Json("item: " + t.title + " was bought", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Cart()
        {
            var totalCost = 0.0;
            var combined = new List<Product>();
            foreach (var item in cart)
            {
                totalCost += item.price;

                if (combined.Where(x => x.id == item.id).ToList().Count()>0)
                {
                    combined.Where(x => x.id == item.id).First().count++;
                }
                else
                {
                    var newProd = new Product()
                    {
                        id = item.id,
                        price = item.price,
                        title = item.title,
                        count = 1
                    };
                    combined.Add(newProd);
                }
            }
            
            return View(new { combined, totalCost = Math.Round(totalCost, 2)});
        }

        [HttpGet]
        public ActionResult AddToCart(int id)
        {
            var t = getWasteInfoFromJSON().Where(x => x.id == id).First();
            cart.Add(t);
            return Json("item: " + t.title + " was added to cart", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CompleteCart()
        {
            foreach (var item in cart)
            {
                if (cart.Where(x => x.id == item.id).Count() > item.count)
                {
                    return Json("item: " + item.title + " has exceeded the stock limit, please reset your cart and try again", JsonRequestBehavior.AllowGet);
                }
            }
            foreach (var item in cart)
            {
                products.Where(x => x.id == item.id).First().count--;
            }
            cart = new List<Product>();
            return Json("cart was completed successfully", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ResetStore()
        {
            using (StreamReader r = new StreamReader(HttpContext.Server.MapPath("~/App_Data/DefaultData.json")))
            {
                string json = r.ReadToEnd();
                products = JsonConvert.DeserializeObject<List<Product>>(json);
            }

            return Json(products, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ResetCart()
        {
            cart = new List<Product>();
            return Json("cart was reset", JsonRequestBehavior.AllowGet);
        }

        private List<Product> getWasteInfoFromJSON()
        {
            //if temp file not exist then make 1
            
            using (StreamReader r = new StreamReader(HttpContext.Server.MapPath("~/App_Data/DefaultData.json")))
            {
                string json = r.ReadToEnd();
                JsonConvert.DeserializeObject<List<Product>>(json);
                if (products == null)
                {
                    products = JsonConvert.DeserializeObject<List<Product>>(json);
                }
                return products;
            }
        }


        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            switch (Request.Url.Scheme)
            {
                case "https":
                    Response.AddHeader("Strict-Transport-Security", "max-age=300");
                    break;
                case "http":
                    var path = "https://" + Request.Url.Host + Request.Url.PathAndQuery;
                    Response.Status = "Please Use https";
                    Response.AddHeader("Location", path);
                    break;
            }
        }
    }
}
