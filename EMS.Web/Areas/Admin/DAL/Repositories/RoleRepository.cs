using EMS.Web.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Areas.Admin.DAL.Repositories
{
    public class RoleRepository
    {
        private readonly emsEntities _ctx;
        public RoleRepository(emsEntities ctx)
        {
            _ctx = ctx;
        }

        public List<adminrole> GetAllRoles()
        {
            return _ctx.adminroles.OrderBy(r => r.name).ToList();
        }

        public List<adminrole> GetAllRoles(int startIndex, int count, string sorting)
        {
            IEnumerable<adminrole> query = _ctx.adminroles;

            if (string.IsNullOrEmpty(sorting) || sorting.Equals("name ASC"))
            {
                query = query.OrderBy(p => p.name);
            }
            else if (sorting.Equals("name DESC"))
            {
                query = query.OrderByDescending(p => p.name);
            }
            else
            {
                query = query.OrderBy(p => p.name);
            }

            return count > 0 ? query.Skip(startIndex).Take(count).ToList() : query.ToList();
        }

        public int GetRoleCount()
        {
            return _ctx.adminroles.ToList().Count;
        }

        public adminrole AddRole(adminrole adminrole)
        {
            if (adminrole != null)
            {
                //clientGroup.parent = 0;
                _ctx.adminroles.Add(adminrole);
                _ctx.SaveChanges();
            }
            return adminrole;
        }

        public void DeleteRole(int id)
        {
            var delete = _ctx.adminroles.Where(r => r.id == id).FirstOrDefault();
            if (delete != null)
            {
                _ctx.adminroles.Remove(delete);
                _ctx.SaveChanges();
            }

        }

        public void UpdateRole(adminrole adminrole)
        {
            var adminroles = _ctx.adminroles.FirstOrDefault(cg => cg.id == adminrole.id);
            if (adminroles == null)
            {
                return;
            }

            adminroles.name = adminrole.name;

            _ctx.SaveChanges();

        }

        public void SelectRole(int? id)
        {
            if (id == null)
            {
                return;
            }
            var role = _ctx.adminroles.Where(r => r.id == id).FirstOrDefault();
            if (role == null)
            {
                return;
            }
        }
    }
}