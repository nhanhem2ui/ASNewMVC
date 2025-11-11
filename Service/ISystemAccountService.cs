using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ISystemAccountService
    {
        void SaveSystemAccount(SystemAccount p);
        void DeleteSystemAccount(short id);
        void UpdateSystemAccount(SystemAccount p);
        List<SystemAccount> GetSystemAccounts();
        SystemAccount GetSystemAccount(string email);
        SystemAccount GetSystemAccountById(short id);
        List<SystemAccount> SearchAccount(string searchHeadline);
    }
}
