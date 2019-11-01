using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcCF5.Models
{
    public class Setting
    {
        public int Id { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string AboutUs { get; set; }
        public string Contact { get; set; }
        public string Referances { get; set; }
        public string Status { get; set; }
        public string Phone { get; set; }
    }
}