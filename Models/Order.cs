using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcCF5.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public int SuppId { get; set; }
        public string SuppName { get; set; }
        public double Amount { get; set; }
        public decimal Longtitude { get; set; }
        public decimal Latitude { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string ShipInfo { get; set; }
        public string Note { get; set; }
        public DateTime? Date { get; set; }
        public bool Status { get; set; }

    }
}