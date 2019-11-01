using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcCF5.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public decimal longtitude { get; set; }
        public decimal latitude { get; set; }
        public double distance { get; set; }
        public string Image { get; set; }
        public string Detail { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
        public bool Status { get; set; }
        



    }
}