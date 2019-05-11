using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramShop
{
    public class Product
    {
        public int id { get; set; }
        public string name { get; set; }
        public int category_id { get; set; }
        public int code { get; set; }
        public double price { get; set; }
        public int availability { get; set; }
        public string brand { get; set; }
        public string description { get; set; }
        public int is_new { get; set; }
        public int is_recommended { get; set; }
        public int status { get; set; }

        public Product(int id = 0,
         string name = "",
         int category_id = 0,
         int code = 0,
         double price = 0,
         int availability = 0,
         string brand = "",
         string description = "",
         int is_new = 0,
         int is_recommended = 0,
         int status = 0)
        {
            this.id = id;
            this.name = name;
            this.category_id = category_id;
            this.code = code;
            this.price = price;
            this.availability = availability;
            this.brand = brand;
            this.description = description;
            this.is_new = is_new;
            this.is_recommended = is_recommended;
            this.status = status;
        }
    }
}
