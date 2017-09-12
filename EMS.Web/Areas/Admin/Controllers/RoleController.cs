using EMS.Web.Areas.Admin.DAL.Repositories;
using EMS.Web.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace EMS.Web.Areas.Admin.Controllers
{
    public class RoleController : Controller
    {
        private emsEntities ctx;
        //private readonly RoleRepository _repo;

        public RoleController()
        {
            ctx = new emsEntities();
        }

        // GET: Admin/Role
        public ActionResult Index()
        {
            return View(ctx.adminroles.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }


        // POST: DrugGenericName/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name")] adminrole adminrole)
        {
            if (ModelState.IsValid)
            {
                //check if values is duplicate
                int count = DuplicateCount(adminrole);
                if (count > 0)
                {
                    ViewBag.DuplicateError = "Already Exists!!";
                    return View(adminrole);
                }
                else
                {
                    ctx.adminroles.Add(adminrole);
                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(adminrole);
        }

        //[HttpPost]
        //public JsonResult RoleList(int startIndex = 0, int pageSize = 0, string sorting = null)
        //{
        //    try
        //    {
        //        Thread.Sleep(200);
        //        var roleCount = _repo.GetRoleCount();
        //        var roleGroups = _repo.GetAllRoles(startIndex, pageSize, sorting);
        //        return Json(new { Result = "OK", Records = roleGroups, TotalRecordCount = roleCount });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public JsonResult CreateRole(adminrole adminRole)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
        //        }
        //        var addedRole = _repo.AddRole(adminRole);
        //        return Json(new { Result = "OK", Record = addedRole });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public JsonResult UpdateRole(adminrole adminRole)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
        //        }
        //        _repo.UpdateRole(adminRole);
        //        return Json(new { Result = "OK" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        //[HttpPost]
        //public JsonResult DeleteRole(int id)
        //{
        //    try
        //    {
        //        Thread.Sleep(50);
        //        _repo.DeleteRole(id);
        //        return Json(new { Result = "OK" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        //[HttpGet]
        //public JsonResult Details(int? id)
        //{
        //    try
        //    {
        //        _repo.SelectRole(id);
        //        return Json(new { Result = "OK" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        public int DuplicateCount(adminrole adminrole)
        {
            List<adminrole> _checkUnique = (from d in ctx.adminroles
                                                  where d.name == adminrole.name
                                                  select d).ToList();
            return _checkUnique.Count;
        }

    }
}