using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcCF5.Models
{
    public class OrderDetail
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int SuppId { get; set; }
        public int Quantity { get; set; }
        public float SellPrice { get; set; }
        public double Amount { get; set; }
        public bool Status { get; set; }

    }
}