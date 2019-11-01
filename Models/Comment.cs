using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcCF5.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? SupplierId { get; set; }
        public int? ProductId { get; set; }
        public string UserName { get; set; }
        public string ProductName { get; set; }
        public string ProductComment { get; set; }
        public int Rate { get; set; }
        public bool Status { get; set; }
    }

}