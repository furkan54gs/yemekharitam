using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcCF5.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Messages { get; set; }
        public string Comment { get; set; }
        public bool Status { get; set; }
    }
}