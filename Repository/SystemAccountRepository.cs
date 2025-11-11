using BussinessObject;
using DataAccess;

namespace Repository
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        private readonly SystemAccountDAO _dao;

        public SystemAccountRepository(SystemAccountDAO dao)
        {
            _dao = dao;
        }

        public void DeleteSystemAccount(SystemAccount p) => _dao.DeleteSystemAccount(p);
        public SystemAccount GetSystemAccount(string email) => _dao.GetSystemAccount(email);
        public SystemAccount GetSystemAccountById(short id) => _dao.GetSystemAccountById(id);
        public List<SystemAccount> SearchAccount(string searchHeadline) => _dao.SearchAccount(searchHeadline);
        public List<SystemAccount> GetSystemAccounts() => _dao.GetSystemAccounts();
        public void SaveSystemAccount(SystemAccount p) => _dao.SaveSystemAccount(p);
        public void UpdateSystemAccount(SystemAccount p) => _dao.UpdateSystemAccount(p);
    }
}