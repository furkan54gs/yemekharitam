using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcCF5.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int Amount { get; set; }
        public float? Price { get; set; }
        public float SellPrice { get; set; }
        public float? Weight { get; set; }
        public string Image { get; set; }
        public string Detail { get; set; }
        public string Recipe { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<ShopCart> ShopCarts { get; set; }
    }
}