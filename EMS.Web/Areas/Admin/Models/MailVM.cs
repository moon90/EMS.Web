using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Areas.Admin.Models
{
    public class MailVM
    {
        public string From { get; set; }
        public string  To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}