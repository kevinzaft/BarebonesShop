using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class Product
    {
        public int id { get; set; }
        public string title { get; set; }
        public float price { get; set; }
        public int count { get; set; }
    }
}