//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EMS.Web.DTO
{
    using System;
    using System.Collections.Generic;
    
    public partial class sms_history
    {
        public int id { get; set; }
        public int userid { get; set; }
        public string sender { get; set; }
        public string receiver { get; set; }
        public int amount { get; set; }
        public string sms { get; set; }
        public string ip { get; set; }
        public string report { get; set; }
        public System.DateTime reqlogtime { get; set; }
        public Nullable<int> send_by { get; set; }
    }
}
