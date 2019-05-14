using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramShop
{
    class ProductOrder
    {
        public int Id { get; set; }
        public string User_name { get; set; }
        public string User_phone { get; set; }
        public string User_comment { get; set; }
        public int User_id { get; set; }
        public DateTime Date { get; set; } // ???
        public string Products { get; set; }
        public int Status { get; set; }

        public ProductOrder()
        {

        }

        public ProductOrder(int id, string user_name, string user_phone, string user_comment, int user_id, DateTime date, string products, int status)
        {
            Id = id;
            User_name = user_name;
            User_phone = user_phone;
            User_comment = user_comment;
            User_id = user_id;
            Date = date;
            Products = products;
            Status = status;
        }
    }
}
