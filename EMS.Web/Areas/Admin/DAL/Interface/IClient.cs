using EMS.Web.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Web.Areas.Admin.DAL.Interface
{
    public interface IClient
    {
        List<account> GetAllClients();
        List<account> GetAllClients(int startIndex, int count, string sorting);
        account AddClientGroup(account Client);
        void UpdateClientGroup(account Client);
        void DeleteStudent(int ClientId);
        int GetStudentCount();
    }
}
