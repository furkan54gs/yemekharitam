using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcCF5.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Img { get; set; }
        public int? UserImg { get; set; }
        public int? ProductImg { get; set; }

    }
}