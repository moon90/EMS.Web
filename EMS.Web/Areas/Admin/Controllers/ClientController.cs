using EMS.Web.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Data.Entity.Validation;
using System.Globalization;

namespace EMS.Web.Areas.Admin.Controllers
{
    public class ClientController : Controller
    {
        private emsEntities ctx;

        public ClientController()
        {
            ctx = new emsEntities();
        }

        // GET: Admin/Client
        public ActionResult Index()
        {

            List<string> CountryList = new List<string>();
            CultureInfo[] CInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo CInfo in CInfoList)
            {
                RegionInfo R = new RegionInfo(CInfo.LCID);
                if (!(CountryList.Contains(R.EnglishName)))
                {
                    CountryList.Add(R.EnglishName);
                }
            }

            CountryList.Sort();
            ViewBag.CounrtyList = CountryList;

            return View(ctx.accounts.OrderByDescending(c => c.id).ToList());
        }

        public ActionResult Create()
        {

            List<email_providers> allEmail = new List<email_providers>();
            List<sms_gateway> allSMS = new List<sms_gateway>();

            ViewBag.country = from p in CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures).OrderBy(c => c.Name)
                              select new SelectListItem
                              {
                                  Text = p.EnglishName,
                                  Value = p.DisplayName
                              };

            allEmail = ctx.email_providers.OrderBy(e => e.name).ToList();
            allSMS = ctx.sms_gateway.OrderBy(s => s.name).ToList();
            ViewBag.ClientGroupID = new SelectList(ctx.accgroups.ToList(), "id", "groupname");
            ViewBag.EmailProviders = new SelectList(allEmail, "name", "name");
            ViewBag.SMSGetway = new SelectList(allSMS, "name", "name");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(account account)
        {
            try
            {
                if (ModelState.IsValid)
                {
                                    
                    //check if values is duplicate
                    int count = DuplicateCount(account);
                    if (count > 0)
                    {
                        ViewBag.country = from p in CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures).OrderBy(c => c.Name)
                                          select new SelectListItem
                                          {
                                              Text = p.EnglishName,
                                              Value = p.DisplayName
                                          };

                        ViewBag.DuplicateError = "Email Address Already Exists!!";
                        ViewBag.ClientGroupID = new SelectList(ctx.accgroups.ToList(), "id", "groupname", account.groupid);
                        ViewBag.EmailProviders = new SelectList(ctx.email_providers, "name", "name", account.email_gateway);
                        ViewBag.SMSGetway = new SelectList(ctx.sms_gateway, "name", "name", account.sms_gateway);
                        
                        return View(account);
                    }
                    else
                    {
                        account.datecreated = DateTime.Now;
                        account.parent = 0;
                        account.email_limit = 0;
                        account.sms_limit = "0";
                        account.status = "Active";
                        account.pwresetkey = "serfwsjsie4we";
                        account.pwresetexpiry = 20;
                        account.emailnotify = "Yes";
                        //account.email_gateway = "Gmail";
                        //account.sms_gateway = "Gooel";

                        

                        //if (account.emailnotify == "Yes" && account.email != "")
                        //{

                        //}

                        ctx.accounts.Add(account);
                        ctx.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }

                ViewBag.emailpermission = account.email_perm;
                ViewBag.smspermission = account.sms_perm;

                List<email_providers> allEmail = new List<email_providers>();
                List<sms_gateway> allSMS = new List<sms_gateway>();

                ViewBag.country = from p in CultureInfo.GetCultures(CultureTypes.AllCultures & ~CultureTypes.NeutralCultures).OrderBy(c => c.Name)
                                  select new SelectListItem
                                  {
                                      Text = p.EnglishName,
                                      Value = p.DisplayName
                                  };

                allEmail = ctx.email_providers.OrderBy(e => e.name).ToList();
                allSMS = ctx.sms_gateway.OrderBy(s => s.name).ToList();

                ViewBag.ClientGroupID = new SelectList(ctx.accgroups.ToList(), "id", "groupname", account.groupid);
                ViewBag.EmailProviders = new SelectList(allEmail, "name", "name", account.email_gateway);
                ViewBag.SMSGetway = new SelectList(allSMS, "name", "name", account.sms_gateway);
                //ViewBag.CounrtyList = CountryList;

                return View(account);

            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                            ve.PropertyName,
                            eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                            ve.ErrorMessage);
                    }
                }
                throw;
            }

            
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            account account = ctx.accounts.Where(a => a.id == id).Select(a => a).FirstOrDefault();
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        public int DuplicateCount(account account)
        {
            List<account> _checkUnique = (from d in ctx.accounts
                                          where d.email == account.email
                                            select d).ToList();
            return _checkUnique.Count;
        }

    }
}