using EMS.Web.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Web.Areas.Admin.DAL.Interface
{
    public interface IClientGroup
    {
        List<accgroup> GetAllClientGroup();
        List<accgroup> GetAllClientGroup(int startIndex, int count, string sorting);
        accgroup AddClientGroup(accgroup clientGroup);
        void UpdateClientGroup(accgroup clientGroup);
        void DeleteClientGroup(int clientGroupId);
        int GetClientGroupCount();
    }
}
