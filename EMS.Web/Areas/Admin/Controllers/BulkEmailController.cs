using EMS.Web.Areas.Admin.Models;
using EMS.Web.Areas.Admin.Utilites;
using EMS.Web.DTO;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EMS.Web.Areas.Admin.Controllers
{
    public class BulkEmailController : Controller
    {

        emsEntities ctx = null;

        public BulkEmailController()
        {
            ctx = new emsEntities();
        }

        // GET: Admin/BulkEmail
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ViewResult Index(MailVM emailModel)
        {
            if (ModelState.IsValid)
            {

                try
                {                
                
                MailMessage mail = new MailMessage();
                mail.To.Add(emailModel.To);
                mail.From = new MailAddress(emailModel.From);
                mail.Subject = emailModel.Subject;
                string Body = emailModel.Body;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("axegangmoon@gmail.com", "262651Nabiun");
                smtp.EnableSsl = true;
                smtp.Send(mail);

                return View("Index", emailModel);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            else
            {
                return View();
            }
            
        }
        public ActionResult EmailProviders()
        {
            var emailProviders = ctx.email_providers.OrderBy(e => e.name).ToList();
            return View(emailProviders);
        }
        public ActionResult EmailProvidersManage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            email_providers emailProviders = ctx.email_providers.Where(e => e.id == id).OrderBy(e => e.name).FirstOrDefault();

            return View(emailProviders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmailProvidersManage(email_providers emailProviders )
        {
            if (ModelState.IsValid)
            {
                if (emailProviders.id > 0)
                {
                    var v = ctx.email_providers.Where(e => e.id.Equals(emailProviders.id)).FirstOrDefault();

                    if (v != null)
                    {
                        v.name = emailProviders.name;
                        v.host_name = emailProviders.host_name;
                        v.username = emailProviders.username;
                        v.password = emailProviders.password;
                        v.port = emailProviders.port;
                        v.status = emailProviders.status;
                    }
                    else
                    {
                        return HttpNotFound();
                    }
                }

                //ctx.SaveChanges();


                try
                {
                   // ctx.Entry(email).State = System.Data.Entity.EntityState.Modified;
                    ctx.SaveChanges();
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
                
                return RedirectToAction("EmailProviders"); 
            }
            return View(emailProviders);
        }

        public int DuplicateCount(email_providers emailProviders)
        {
            List<email_providers> _checkUnique = (from d in ctx.email_providers
                                                  where d.name == emailProviders.name
                                                  select d).ToList();
            return _checkUnique.Count;
        }


        [HttpGet]
        public ActionResult SendBulkEmail()
        {
            IEnumerable<accgroup> item = ctx.accgroups.ToList();
            ViewBag.ClientGroupID = new MultiSelectList(item, "id", "groupname");
            ViewBag.EmailProviders = new SelectList(ctx.email_providers, "name", "name");

            return View();
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult SendBulkEmail(FormCollection form)
        {
            BulkMailVM bulkMail = new BulkMailVM();
            


            string clientGroupsForm = form["clientGroups"];
            string subjectForm = form["Subject"];
            string messageForm = form["Message"];
            string emailProviderForm = form["EmailProviderName"];

            bulkMail.clientGroups = clientGroupsForm;
            bulkMail.Subject = subjectForm;
            bulkMail.Message = messageForm;
            bulkMail.EmailProviderName = emailProviderForm;


            if (ModelState.IsValid)
            {                
                var numbers = clientGroupsForm.Split(',').Select(Int32.Parse).ToList();

                var clientgroup = ctx.accgroups.FirstOrDefault();
                foreach (var number in numbers)
                {
                   var clientList = ctx.accounts.Where(a => a.groupid == number).ToList();
                }
                
                var clientGroupList = ctx.accgroups.ToList();

                var appConfig = ctx.appconfigs.FirstOrDefault();

                var appConfigList = ctx.appconfigs.ToList();
                //var accountList = ctx.accounts.Where(ac => ac.groupid).ToList();
                var emailProviderList = ctx.email_providers.ToList();
                var emailLogs = ctx.email_logs.ToList();

                var company = ctx.appconfigs.Where(a => a.setting == "CompanyName").Select(a => a.value).First();
                var email = ctx.appconfigs.Where(a => a.setting == "Email").Select(a => a.value).First();
                var sysUrl = ctx.appconfigs.Where(a => a.setting == "sysUrl").Select(a => a.value).First();




                foreach (var number in numbers)
                {
                    var clientList = ctx.accounts.Where(a => a.groupid == number).FirstOrDefault();

                    var clid = clientList.id;
                    var clname = clientList.name;
                    var clemail = clientList.email;
                    var v = ctx.email_logs.FirstOrDefault();
                    v.userid = clid;
                    v.email = clemail;
                    v.subject = bulkMail.Subject;
                    v.message = bulkMail.Message;
                    v.send_by = 0;
                    ctx.email_logs.Add(v);
                    ctx.SaveChanges();
                }

                if (ctx.accounts.ToList().Count > 0)
                {

                    bulkMail.Company = company;
                    bulkMail.Email = email;

                    var emailTemplates = ctx.email_templates.Where(et => et.tplname == "Bulk Email Send").FirstOrDefault();

                    bulkMail.Message = emailTemplates.message;
                    bulkMail.EmailTemplateSend = emailTemplates.send;


                    bulkMail.Message = bulkMail.EmailTemplateMessage;


                    if (bulkMail.EmailTemplateSend == true)
                    {
                        if (bulkMail.EmailProviderName == "Gmail")
                        {
                            var pmq = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).FirstOrDefault();
                            var status_check = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).Where(ep => ep.status == 1).Count();

                            if (status_check == 1)
                            {
                                bulkMail.HostName = pmq.host_name;
                                bulkMail.UserName = pmq.username;
                                bulkMail.Password = pmq.password;
                                bulkMail.Port = pmq.port;

                                MailMessage mail = new MailMessage();
                                mail.From = new MailAddress(bulkMail.Email, bulkMail.Company);
                                mail.ReplyTo = new MailAddress(bulkMail.Email, bulkMail.Company);


                                foreach (var i in numbers)
                                {
                                    var clientList = ctx.accounts.Where(a => a.groupid == i).FirstOrDefault();
                                    var clientMail = clientList.email;
                                    var clientName = clientList.name;
                                    mail.To.Add(new MailAddress(clientMail, clientName));
                                }
                                bulkMail.Message = messageForm;
                                mail.Subject = bulkMail.Subject;
                                mail.Body = bulkMail.Message;
                                mail.BodyEncoding = Encoding.UTF8;
                                mail.IsBodyHtml = true;

                                
                                SmtpClient smtp = new SmtpClient();
                                smtp.Host = bulkMail.HostName;
                                if (Convert.ToInt32(bulkMail.Port) == 465)
                                {
                                    smtp.EnableSsl = false;
                                }
                                else
                                {
                                    smtp.EnableSsl = true;
                                }

                                smtp.Port = Convert.ToInt32(bulkMail.Port);
                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = new System.Net.NetworkCredential(bulkMail.UserName, bulkMail.Password);
                                smtp.Send(mail);
                            }
                        }
                        else if (bulkMail.EmailProviderName == "MailGun")
                        {
                            var pmq = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).FirstOrDefault();
                            var status_check = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).Where(ep => ep.status == 1).Count();
                            if (status_check == 1)
                            {
                                bulkMail.HostName = pmq.host_name;
                                bulkMail.UserName = pmq.username;
                                bulkMail.Password = pmq.password;
                                bulkMail.Port = pmq.port;

                                MailMessage mail = new MailMessage();
                                mail.From = new MailAddress(bulkMail.Email, bulkMail.Company);
                                mail.ReplyTo = new MailAddress(bulkMail.Email, bulkMail.Company);

                                foreach (var i in numbers)
                                {
                                    var clientList = ctx.accounts.Where(a => a.groupid == i).FirstOrDefault();
                                    var clientMail = clientList.email;
                                    var clientName = clientList.name;
                                    mail.To.Add(new MailAddress(clientMail, clientName));
                                }

                                SmtpClient smtp = new SmtpClient();
                                smtp.Host = bulkMail.HostName;
                                if (Convert.ToInt32(bulkMail.Port) == 465)
                                {
                                    smtp.EnableSsl = false;
                                }
                                else
                                {
                                    smtp.EnableSsl = true;
                                }

                                smtp.Port = Convert.ToInt32(bulkMail.Port);
                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = new System.Net.NetworkCredential(bulkMail.UserName, bulkMail.Password);
                                smtp.Send(mail);
                            }
                        }
                        else if (bulkMail.EmailProviderName == "SendGrid")
                        {
                            var pmq = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).FirstOrDefault();
                            var status_check = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).Where(ep => ep.status == 1).Count();
                            if (status_check == 1)
                            {
                                bulkMail.HostName = pmq.host_name;
                                bulkMail.UserName = pmq.username;
                                bulkMail.Password = pmq.password;
                                bulkMail.Port = pmq.port;

                                MailMessage mail = new MailMessage();
                                mail.From = new MailAddress(bulkMail.Email, bulkMail.Company);
                                mail.ReplyTo = new MailAddress(bulkMail.Email, bulkMail.Company);

                                foreach (var i in numbers)
                                {
                                    var clientList = ctx.accounts.Where(a => a.groupid == i).FirstOrDefault();
                                    var clientMail = clientList.email;
                                    var clientName = clientList.name;
                                    mail.To.Add(new MailAddress(clientMail, clientName));
                                }

                                SmtpClient smtp = new SmtpClient();
                                smtp.Host = bulkMail.HostName;
                                if (Convert.ToInt32(bulkMail.Port) == 465)
                                {
                                    smtp.EnableSsl = false;
                                }
                                else
                                {
                                    smtp.EnableSsl = true;
                                }

                                smtp.Port = Convert.ToInt32(bulkMail.Port);
                                smtp.UseDefaultCredentials = false;
                                smtp.Credentials = new System.Net.NetworkCredential(bulkMail.UserName, bulkMail.Password);
                                smtp.Send(mail);
                            }
                        }
                    }
                }
            }

            IEnumerable<accgroup> item = ctx.accgroups.ToList();
            ViewBag.ClientGroupID = new SelectList(item, "id", "groupname", bulkMail.clientGroups);
            ViewBag.EmailProviders = new SelectList(ctx.email_providers.ToList(), "name", "name", bulkMail.EmailProviderName);

            return View(bulkMail);
        }

        [HttpGet]
        public ActionResult SendMailFromFile()
        {
            ViewBag.EmailProviders = new SelectList(ctx.email_providers, "name", "name");

            return View();
        }


        [ActionName("SendMailFromFile")]
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]        
        public ActionResult SendMailFromFile(FormCollection form)
        {
            BulkMailVM bulkMail = new BulkMailVM();

            string subjectForm = form["Subject"];
            string messageForm = form["Message"];
            string emailProviderForm = form["EmailProviderName"];


            if (Request.Files["FileUpload"].ContentLength > 0)
            {
                string extension = System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName).ToLower();
                string query = null;
                string connString = "";

                string[] validFileTypes = { ".xls", ".xlsx", ".csv", ".txt" };

                string path = string.Format("{0}/{1}", Server.MapPath("~/Content/Doc"), Request.Files["FileUpload"].FileName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/Doc"));
                }

                

                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path))
                    { System.IO.File.Delete(path); }
                    Request.Files["FileUpload"].SaveAs(path);


                    var company = ctx.appconfigs.Where(a => a.setting == "CompanyName").Select(a => a.value).First();
                    var email = ctx.appconfigs.Where(a => a.setting == "Email").Select(a => a.value).First();
                    var sysUrl = ctx.appconfigs.Where(a => a.setting == "sysUrl").Select(a => a.value).First();

                    string csvData = System.IO.File.ReadAllText(path);

                    //string[] lines = theText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)

                    string[] lines = csvData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                    bulkMail.UserID = 0;


                    //var emaillogList = ctx.email_logs.ToList();
                    var emaillog = new email_logs();

                    if (extension == ".csv")
                    {                       
                        foreach (var row in lines)
                        {
                            if (!string.IsNullOrEmpty(row))
                            {
                                bulkMail.Email = row.Split(',')[0];
                                emaillog.userid = bulkMail.UserID;
                                emaillog.email = bulkMail.Email;
                                emaillog.subject = subjectForm;
                                emaillog.message = messageForm;
                                emaillog.send_by = 0;
                                emaillog.date = DateTime.Now;
                                ctx.email_logs.Add(emaillog);
                                ctx.SaveChanges();
                            }
                        }

                        if (csvData.Count() > 0)
                        {
                            bulkMail.Company = company;
                            bulkMail.Email = email;

                            var emailTemplates = ctx.email_templates.Where(et => et.tplname == "Bulk Email Send").FirstOrDefault();

                            bulkMail.Message = emailTemplates.message;
                            bulkMail.EmailTemplateSend = emailTemplates.send;
                            bulkMail.EmailProviderName = emailProviderForm;

                            bulkMail.Subject = subjectForm;

                            bulkMail.Message = bulkMail.EmailTemplateMessage;

                            if (bulkMail.EmailTemplateSend == true)
                            {
                                if (bulkMail.EmailProviderName == "Gmail")
                                {
                                    var pmq = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).FirstOrDefault();
                                    var status_check = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).Where(ep => ep.status == 1).Count();

                                    if (status_check == 1)
                                    {
                                        bulkMail.HostName = pmq.host_name;
                                        bulkMail.UserName = pmq.username;
                                        bulkMail.Password = pmq.password;
                                        bulkMail.Port = pmq.port;

                                        MailMessage mail = new MailMessage();
                                        mail.From = new MailAddress(bulkMail.Email, bulkMail.Company);
                                        mail.ReplyTo = new MailAddress(bulkMail.Email, bulkMail.Company);


                                        foreach (var row in lines)
                                        {
                                            if (!string.IsNullOrEmpty(row))
                                            {
                                                bulkMail.Email = row.Split(',')[0];
                                                bulkMail.Company = company+" "+"Client";
                                                mail.To.Add(new MailAddress(bulkMail.Email, bulkMail.Company));
                                            }
                                        }
                                        bulkMail.Message = messageForm;
                                        mail.Subject = bulkMail.Subject;
                                        mail.Body = bulkMail.Message;
                                        mail.BodyEncoding = Encoding.UTF8;
                                        mail.IsBodyHtml = true;


                                        SmtpClient smtp = new SmtpClient();
                                        smtp.Host = bulkMail.HostName;
                                        if (Convert.ToInt32(bulkMail.Port) == 465)
                                        {
                                            smtp.EnableSsl = false;
                                        }
                                        else
                                        {
                                            smtp.EnableSsl = true;
                                        }

                                        smtp.Port = Convert.ToInt32(bulkMail.Port);
                                        smtp.UseDefaultCredentials = false;
                                        smtp.Credentials = new System.Net.NetworkCredential(bulkMail.UserName, bulkMail.Password);
                                        smtp.Send(mail);
                                    }
                                }
                                else if (bulkMail.EmailProviderName == "MailGun")
                                {
                                    var pmq = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).FirstOrDefault();
                                    var status_check = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).Where(ep => ep.status == 1).Count();
                                    if (status_check == 1)
                                    {
                                        bulkMail.HostName = pmq.host_name;
                                        bulkMail.UserName = pmq.username;
                                        bulkMail.Password = pmq.password;
                                        bulkMail.Port = pmq.port;

                                        MailMessage mail = new MailMessage();
                                        mail.From = new MailAddress(bulkMail.Email, bulkMail.Company);
                                        mail.ReplyTo = new MailAddress(bulkMail.Email, bulkMail.Company);

                                        foreach (var row in lines)
                                        {
                                            if (!string.IsNullOrEmpty(row))
                                            {
                                                bulkMail.Email = row.Split(',')[0];
                                                bulkMail.Company = company + " " + "Client";
                                                mail.To.Add(new MailAddress(bulkMail.Email, bulkMail.Company));
                                            }
                                        }

                                        bulkMail.Message = messageForm;
                                        mail.Subject = bulkMail.Subject;
                                        mail.Body = bulkMail.Message;
                                        mail.BodyEncoding = Encoding.UTF8;
                                        mail.IsBodyHtml = true;

                                        SmtpClient smtp = new SmtpClient();
                                        smtp.Host = bulkMail.HostName;
                                        if (Convert.ToInt32(bulkMail.Port) == 465)
                                        {
                                            smtp.EnableSsl = false;
                                        }
                                        else
                                        {
                                            smtp.EnableSsl = true;
                                        }

                                        smtp.Port = Convert.ToInt32(bulkMail.Port);
                                        smtp.UseDefaultCredentials = false;
                                        smtp.Credentials = new System.Net.NetworkCredential(bulkMail.UserName, bulkMail.Password);
                                        smtp.Send(mail);
                                    }
                                }
                                else if (bulkMail.EmailProviderName == "SendGrid")
                                {
                                    var pmq = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).FirstOrDefault();
                                    var status_check = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).Where(ep => ep.status == 1).Count();
                                    if (status_check == 1)
                                    {
                                        bulkMail.HostName = pmq.host_name;
                                        bulkMail.UserName = pmq.username;
                                        bulkMail.Password = pmq.password;
                                        bulkMail.Port = pmq.port;

                                        MailMessage mail = new MailMessage();
                                        mail.From = new MailAddress(bulkMail.Email, bulkMail.Company);
                                        mail.ReplyTo = new MailAddress(bulkMail.Email, bulkMail.Company);

                                        foreach (var row in lines)
                                        {
                                            if (!string.IsNullOrEmpty(row))
                                            {
                                                bulkMail.Email = row.Split(',')[0];
                                                bulkMail.Company = company + " " + "Client";
                                                try
                                                {
                                                    mail.To.Add(new MailAddress(bulkMail.Email, bulkMail.Company));
                                                }
                                                catch (Exception ex)
                                                {
                                                    ViewBag.error = ex.Data + " " + ex.Message; 
                                                }
                                                
                                            }
                                        }

                                        bulkMail.Message = messageForm;
                                        mail.Subject = bulkMail.Subject;
                                        mail.Body = bulkMail.Message;
                                        mail.BodyEncoding = Encoding.UTF8;
                                        mail.IsBodyHtml = true;

                                        SmtpClient smtp = new SmtpClient();
                                        smtp.Host = bulkMail.HostName;
                                        if (Convert.ToInt32(bulkMail.Port) == 465)
                                        {
                                            smtp.EnableSsl = false;
                                        }
                                        else
                                        {
                                            smtp.EnableSsl = true;
                                        }

                                        smtp.Port = Convert.ToInt32(bulkMail.Port);
                                        smtp.UseDefaultCredentials = false;
                                        smtp.Credentials = new System.Net.NetworkCredential(bulkMail.UserName, bulkMail.Password);
                                        smtp.Send(mail);
                                    }
                                }
                            }
                        }



                        //DataTable dt = Utility.ConvertCSVtoDataTable(path);
                        //ViewBag.Data = dt;
                    }
                    else if (extension == ".txt")
                    {

                        foreach (var row in lines)
                        {
                            if (!string.IsNullOrEmpty(row))
                            {
                                bulkMail.Email = row.Split(',')[0];
                                emaillog.userid = 0;
                                emaillog.email = bulkMail.Email;
                                emaillog.subject = subjectForm;
                                emaillog.message = messageForm;
                                emaillog.send_by = 0;
                                emaillog.date = DateTime.Now;
                                ctx.email_logs.Add(emaillog);
                                ctx.SaveChanges();
                            }
                        }

                        if (csvData.Count() > 0)
                        {
                            bulkMail.Company = company;
                            bulkMail.Email = email;

                            var emailTemplates = ctx.email_templates.Where(et => et.tplname == "Bulk Email Send").FirstOrDefault();

                            bulkMail.Message = emailTemplates.message;
                            bulkMail.EmailTemplateSend = emailTemplates.send;
                            bulkMail.EmailProviderName = emailProviderForm;

                            bulkMail.Subject = subjectForm;

                            bulkMail.Message = bulkMail.EmailTemplateMessage;

                            if (bulkMail.EmailTemplateSend == true)
                            {
                                if (bulkMail.EmailProviderName == "Gmail")
                                {
                                    var pmq = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).FirstOrDefault();
                                    var status_check = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).Where(ep => ep.status == 1).Count();

                                    if (status_check == 1)
                                    {
                                        bulkMail.HostName = pmq.host_name;
                                        bulkMail.UserName = pmq.username;
                                        bulkMail.Password = pmq.password;
                                        bulkMail.Port = pmq.port;

                                        MailMessage mail = new MailMessage();
                                        mail.From = new MailAddress(bulkMail.Email, bulkMail.Company);
                                        mail.ReplyTo = new MailAddress(bulkMail.Email, bulkMail.Company);


                                        foreach (var row in lines)
                                        {
                                            if (!string.IsNullOrEmpty(row))
                                            {
                                                bulkMail.Email = row.Split(',')[0];
                                                bulkMail.Company = company + " " + "Client";
                                                mail.To.Add(new MailAddress(bulkMail.Email, bulkMail.Company));
                                            }
                                        }
                                        bulkMail.Message = messageForm;
                                        mail.Subject = bulkMail.Subject;
                                        mail.Body = bulkMail.Message;
                                        mail.BodyEncoding = Encoding.UTF8;
                                        mail.IsBodyHtml = true;


                                        SmtpClient smtp = new SmtpClient();
                                        smtp.Host = bulkMail.HostName;
                                        if (Convert.ToInt32(bulkMail.Port) == 465)
                                        {
                                            smtp.EnableSsl = false;
                                        }
                                        else
                                        {
                                            smtp.EnableSsl = true;
                                        }

                                        smtp.Port = Convert.ToInt32(bulkMail.Port);
                                        smtp.UseDefaultCredentials = false;
                                        smtp.Credentials = new System.Net.NetworkCredential(bulkMail.UserName, bulkMail.Password);
                                        smtp.Send(mail);
                                    }
                                }
                                else if (bulkMail.EmailProviderName == "MailGun")
                                {
                                    var pmq = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).FirstOrDefault();
                                    var status_check = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).Where(ep => ep.status == 1).Count();
                                    if (status_check == 1)
                                    {
                                        bulkMail.HostName = pmq.host_name;
                                        bulkMail.UserName = pmq.username;
                                        bulkMail.Password = pmq.password;
                                        bulkMail.Port = pmq.port;

                                        MailMessage mail = new MailMessage();
                                        mail.From = new MailAddress(bulkMail.Email, bulkMail.Company);
                                        mail.ReplyTo = new MailAddress(bulkMail.Email, bulkMail.Company);

                                        foreach (var row in lines)
                                        {
                                            if (!string.IsNullOrEmpty(row))
                                            {
                                                bulkMail.Email = row.Split(',')[0];
                                                bulkMail.Company = company + " " + "Client";
                                                mail.To.Add(new MailAddress(bulkMail.Email, bulkMail.Company));
                                            }
                                        }

                                        bulkMail.Message = messageForm;
                                        mail.Subject = bulkMail.Subject;
                                        mail.Body = bulkMail.Message;
                                        mail.BodyEncoding = Encoding.UTF8;
                                        mail.IsBodyHtml = true;

                                        SmtpClient smtp = new SmtpClient();
                                        smtp.Host = bulkMail.HostName;
                                        if (Convert.ToInt32(bulkMail.Port) == 465)
                                        {
                                            smtp.EnableSsl = false;
                                        }
                                        else
                                        {
                                            smtp.EnableSsl = true;
                                        }

                                        smtp.Port = Convert.ToInt32(bulkMail.Port);
                                        smtp.UseDefaultCredentials = false;
                                        smtp.Credentials = new System.Net.NetworkCredential(bulkMail.UserName, bulkMail.Password);
                                        smtp.Send(mail);
                                    }
                                }
                                else if (bulkMail.EmailProviderName == "SendGrid")
                                {
                                    var pmq = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).FirstOrDefault();
                                    var status_check = ctx.email_providers.Where(ep => ep.name == bulkMail.EmailProviderName).Where(ep => ep.status == 1).Count();
                                    if (status_check == 1)
                                    {
                                        bulkMail.HostName = pmq.host_name;
                                        bulkMail.UserName = pmq.username;
                                        bulkMail.Password = pmq.password;
                                        bulkMail.Port = pmq.port;

                                        MailMessage mail = new MailMessage();
                                        mail.From = new MailAddress(bulkMail.Email, bulkMail.Company);
                                        mail.ReplyTo = new MailAddress(bulkMail.Email, bulkMail.Company);

                                        foreach (var row in lines)
                                        {
                                            if (!string.IsNullOrEmpty(row))
                                            {
                                                bulkMail.Email = row.Split(',')[0];
                                                bulkMail.Company = company + " " + "Client";
                                                mail.To.Add(new MailAddress(bulkMail.Email, bulkMail.Company));
                                            }
                                        }

                                        bulkMail.Message = messageForm;
                                        mail.Subject = bulkMail.Subject;
                                        mail.Body = bulkMail.Message;
                                        mail.BodyEncoding = Encoding.UTF8;
                                        mail.IsBodyHtml = true;

                                        SmtpClient smtp = new SmtpClient();
                                        smtp.Host = bulkMail.HostName;
                                        if (Convert.ToInt32(bulkMail.Port) == 465)
                                        {
                                            smtp.EnableSsl = false;
                                        }
                                        else
                                        {
                                            smtp.EnableSsl = true;
                                        }

                                        smtp.Port = Convert.ToInt32(bulkMail.Port);
                                        smtp.UseDefaultCredentials = false;
                                        smtp.Credentials = new System.Net.NetworkCredential(bulkMail.UserName, bulkMail.Password);
                                        smtp.Send(mail);
                                    }
                                }
                            }
                        }

                        //DataTable dt = Utility.ConvertCSVtoDataTable(path);
                        //ViewBag.Data = dt;
                    }
                    else if (extension.Trim() == ".xls")
                    {
                        connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        DataTable dt = Utility.ConvertXSLXtoDataTable(path, connString);
                        ViewBag.Data = dt;
                    }
                    else if (extension.Trim() == ".xlsx")
                    {
                        connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        DataTable dt = Utility.ConvertXSLXtoDataTable(path, connString);
                        ViewBag.Data = dt;
                    }
                }
                else
                {
                    ViewBag.Error = "Please Upload Files in .xls, .xlsx or .csv format";
                }
            }

            ViewBag.EmailProviders = new SelectList(ctx.email_providers.ToList(), "name", "name", bulkMail.EmailProviderName);

            return View(bulkMail);
        }

        //public ActionResult FileUpload()
        //{
        //    return View(new List<BulkMailVM>());
        //}

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase uploaded_file)
        {
            BulkMailVM bulkMail = new BulkMailVM();
            string filePath = string.Empty;

            if (uploaded_file != null)
            {
                string path = Server.MapPath("~/Content/Doc/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(uploaded_file.FileName);
                string extension = Path.GetExtension(uploaded_file.FileName);
                uploaded_file.SaveAs(filePath);

                //Read the contents of CSV file.
                string csvData = System.IO.File.ReadAllText(filePath);

                //Execute a loop over the rows.
                foreach (var row in csvData.Split('\n'))
                {
                    if (!string.IsNullOrEmpty(row))
                    {
                        bulkMail.Email = row.Split(',')[0];
                    }
                }
            }
            return View(bulkMail);
        }

        public FileResult DownloadCSV()
        {
            string path = "~/Content/Doc/TestFile.csv";
            return File(path, "application/CSV", "TestFile.csv");
        }

        public ActionResult ImportExcel()
        {
            return View();
        }

        [ActionName("ImportExcel")]
        [HttpPost]
        public ActionResult Importexcel()
        {
            if (Request.Files["FileUpload"].ContentLength > 0)
            {
                string extension = System.IO.Path.GetExtension(Request.Files["FileUpload"].FileName).ToLower();
                string query = null;
                string connString = "";

                string[] validFileTypes = { ".xls", ".xlsx", ".csv", ".txt" };

                string path = string.Format("{0}/{1}", Server.MapPath("~/Content/Doc"), Request.Files["FileUpload"].FileName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(Server.MapPath("~/Content/Doc"));
                }

                if (validFileTypes.Contains(extension))
                {
                    if (System.IO.File.Exists(path))
                    { System.IO.File.Delete(path);}
                    Request.Files["FileUpload"].SaveAs(path);
                    if (extension == ".csv")
                    {
                        DataTable dt = Utility.ConvertCSVtoDataTable(path);
                        ViewBag.Data = dt;
                    }
                    else if (extension == ".txt")
                    {
                        DataTable dt = Utility.ConvertCSVtoDataTable(path);
                        ViewBag.Data = dt;
                    }
                    else if(extension.Trim() == ".xls")
                    {
                        connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        DataTable dt = Utility.ConvertXSLXtoDataTable(path, connString);
                        ViewBag.Data = dt;
                    }
                    else if(extension.Trim() == ".xlsx")
                    {
                        connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        DataTable dt = Utility.ConvertXSLXtoDataTable(path, connString);
                        ViewBag.Data = dt;
                    }
                }
                else
                {
                    ViewBag.Error = "Please Upload Files in .xls, .xlsx or .csv format";
                }
            }
            return View();
        }

    }
}