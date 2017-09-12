using EMS.Web.Areas.Admin.DAL.Interface;
using EMS.Web.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMS.Web.Areas.Admin.DAL.Repositories
{
    public class ClientGroupRepository : IClientGroup
    {
        private readonly emsEntities _ctx;
        public ClientGroupRepository(emsEntities ctx)
        {
            _ctx = ctx;
        }

        public accgroup AddClientGroup(accgroup clientGroup)
        {
            if (clientGroup != null)
            {
                clientGroup.parent = 0;
                _ctx.accgroups.Add(clientGroup);
                _ctx.SaveChanges();
            }
            return clientGroup;

        }

        public void DeleteClientGroup(int ClientGroupId)
        {
            var delete = _ctx.accgroups.Where(cg => cg.id == ClientGroupId).FirstOrDefault();
            if (delete != null)
            {
                _ctx.accgroups.Remove(delete);
                _ctx.SaveChanges();
            }
            
        }

        public List<accgroup> GetAllClientGroup()
        {
            return _ctx.accgroups.OrderBy(c => c.groupname).ToList();
        }

        public List<accgroup> GetAllClientGroup(int startIndex, int count, string sorting)
        {
            IEnumerable<accgroup> query = _ctx.accgroups;

            if (string.IsNullOrEmpty(sorting) || sorting.Equals("groupname ASC"))
            {
                query = query.OrderBy(p => p.groupname);
            }
            else if (sorting.Equals("groupname DESC"))
            {
                query = query.OrderByDescending(p => p.groupname);
            }
            else
            {
                query = query.OrderBy(p => p.groupname);
            }

            return count > 0 ? query.Skip(startIndex).Take(count).ToList() : query.ToList();
        }

        public int GetClientGroupCount()
        {
            return _ctx.accgroups.ToList().Count;
        }

        public void UpdateClientGroup(accgroup clientGroup)
        {
            var accgroups = _ctx.accgroups.FirstOrDefault(cg => cg.id == clientGroup.id);
            if (accgroups == null)
            {
                return;
            }

            accgroups.groupname = clientGroup.groupname;
            accgroups.parent = 0;

            _ctx.SaveChanges();
            
        }
    }
}