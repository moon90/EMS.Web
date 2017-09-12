using EMS.Web.Areas.Admin.DAL.Interface;
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
    public class ClientGroupController : Controller
    {
        private emsEntities ctx = new emsEntities();
        private readonly ClientGroupRepository _repo;

        public ClientGroupController()
        {
            _repo = new ClientGroupRepository(ctx);
        }

        // GET: Admin/ClientGroup
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ClientGroupList(int startIndex = 0, int pageSize = 0, string sorting = null)
        {
            try
            {
                Thread.Sleep(200);
                var clientGroupCount = _repo.GetClientGroupCount();
                var clientGroups = _repo.GetAllClientGroup(startIndex, pageSize, sorting);
                return Json(new { Result = "OK", Records = clientGroups, TotalRecordCount = clientGroupCount });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreateClientGroup(accgroup clientGroup)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                var addedClientGroup = _repo.AddClientGroup(clientGroup);
                return Json(new { Result = "OK", Record = addedClientGroup });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateClientGroup(accgroup clientGroup)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                _repo.UpdateClientGroup(clientGroup);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteClientGroup(int id)
        {
            try
            {
                Thread.Sleep(50);
                _repo.DeleteClientGroup(id);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}