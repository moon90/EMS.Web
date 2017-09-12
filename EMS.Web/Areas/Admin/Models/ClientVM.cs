using EMS.Web.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Areas.Admin.Models
{
    public class ClientVM
    {
        public int id { get; set; }
        public Nullable<int> groupid { get; set; }
        public string name { get; set; }
        public string lname { get; set; }
        public string company { get; set; }
        public string website { get; set; }
        public string email { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public string password { get; set; }
        public string image { get; set; }
        public System.DateTime datecreated { get; set; }
        public int email_limit { get; set; }
        public string sms_limit { get; set; }
        public Nullable<System.DateTime> lastlogin { get; set; }
        public int email_perm { get; set; }
        public int sms_perm { get; set; }
        public Nullable<int> online { get; set; }
        public string status { get; set; }
        public string pwresetkey { get; set; }
        public int pwresetexpiry { get; set; }
        public string emailnotify { get; set; }
        public string email_gateway { get; set; }
        public string sms_gateway { get; set; }
        public int parent { get; set; }

        public virtual accgroup groupname { get; set; }
        public virtual email_providers emailProvider { get; set; }
        public virtual sms_gateway smsGateway  { get; set; }
    }
}