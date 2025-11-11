using BussinessObject;
using Repository;
using System;
using System.Linq.Expressions;

namespace Service
{
    public class SystemAccountService : ISystemAccountService
    {
        private readonly ISystemAccountRepository iSystemAccountRepository;

        public SystemAccountService(ISystemAccountRepository systemAccountRepository)
        {
            iSystemAccountRepository = systemAccountRepository;
        }

        public void DeleteSystemAccount(short id)
        {
            var p = iSystemAccountRepository.GetSystemAccountById(id);
            if (p != null)
            {
                iSystemAccountRepository.DeleteSystemAccount(p);
            }
        }

        public SystemAccount GetSystemAccount(string email)
        {
            return iSystemAccountRepository.GetSystemAccount(email);
        }
        public SystemAccount GetSystemAccountById(short id)
        {
            return iSystemAccountRepository.GetSystemAccountById(id);
        }

        public List<SystemAccount> SearchAccount(string searchHeadline)
        {
            return iSystemAccountRepository.SearchAccount(searchHeadline);
        }

        public List<SystemAccount> GetSystemAccounts()
        {
            return iSystemAccountRepository.GetSystemAccounts();
        }

        public void SaveSystemAccount(SystemAccount p)
        {
            iSystemAccountRepository.SaveSystemAccount(p);
        }

        public void UpdateSystemAccount(SystemAccount p)
        {
            iSystemAccountRepository.UpdateSystemAccount(p);
        }
    }
}
