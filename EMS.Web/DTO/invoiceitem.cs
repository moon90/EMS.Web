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
    
    public partial class invoiceitem
    {
        public int id { get; set; }
        public string cname { get; set; }
        public string item { get; set; }
        public decimal price { get; set; }
        public Nullable<int> qty { get; set; }
        public decimal tamount { get; set; }
        public int invoiceid { get; set; }
    }
}
