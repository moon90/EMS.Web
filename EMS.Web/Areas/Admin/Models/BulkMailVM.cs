using EMS.Web.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMS.Web.Areas.Admin.Models
{
    public class BulkMailVM
    {
        public int ClientId { get; set; }

        public string Subject { get; set; }

        [AllowHtml]
        public string Message { get; set; }
        public int ClientGroupId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string clientGroups { get; set; }

        public int EmailProviderId { get; set; }
        public string EmailProviderName { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
        public int Status { get; set; }

        public int EmailLogID { get; set; }
        public int UserID { get; set; }
        public string EmailLogEmail { get; set; }
        public string EmailLogSubject { get; set; }
        public string EmailLogMessage { get; set; }
        public DateTime?  data { get; set; }
        public int SendBy { get; set; }

        public int EmailTemplateID { get; set; }
        public string Tplname { get; set; }
        public int LanguageId { get; set; }
        public string EmailTemplateSubject { get; set; }
        public string EmailTemplateMessage { get; set; }
        public bool? EmailTemplateSend { get; set; }
        public core? Core { get; set; }
        public int Hidden { get; set; }

    }

    public enum core {
        Yes,
        No
    }
}