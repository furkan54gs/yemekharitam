using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcCF5.Models
{
    public class ShopCart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SupplierId { get; set; }
        public int Quantity { get; set; }

        public virtual Product Product { get; set; }
    }
}