using EMS.Web.Areas.Admin.Models;
using EMS.Web.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMS.Web.Areas.Admin.DAL.Repositories
{
    public class BulkMailRepository
    {

        emsEntities ctx = new emsEntities();


        public IList<account> accounts(FormCollection form)
        {
            BulkMailVM bulkMailVM = new BulkMailVM();

            char[] clientGroups = form["clientGroups"].ToArray();
            string subject = form["Subject"];
            string message = form["Message"];
            string emailProvider = form["Email_Provider"];

            var result = ctx.accounts.ToList();
            if (bulkMailVM != null)
            {
                
            }

           return result;
        }
    }
}