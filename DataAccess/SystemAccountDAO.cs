using BussinessObject;

namespace DataAccess
{
    public class SystemAccountDAO
    {
        public List<SystemAccount> GetSystemAccounts()
        {
            var listSystemAccounts = new List<SystemAccount>();
            try
            {
                using var db = new FunewsManagementContext();
                listSystemAccounts = db.SystemAccounts.ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listSystemAccounts;
        }

        public void SaveSystemAccount(SystemAccount p)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.SystemAccounts.Add(p);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void UpdateSystemAccount(SystemAccount p)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Entry<SystemAccount>(p).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void DeleteSystemAccount(SystemAccount p)
        {
            try
            {
                using var context = new FunewsManagementContext();
                var p1 = context.SystemAccounts.SingleOrDefault(c => c.AccountId == p.AccountId);
                context.SystemAccounts.Remove(p1);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public SystemAccount GetSystemAccount(string email)
        {
            using var db = new FunewsManagementContext();
            return db.SystemAccounts.FirstOrDefault(p => p.AccountEmail.Equals(email));
        }

        public SystemAccount GetSystemAccountById(short id)
        {
            using var db = new FunewsManagementContext();
            var account = db.SystemAccounts.FirstOrDefault(p => p.AccountId == id) ?? db.SystemAccounts.Find(id);
            return account;
        }

        public List<SystemAccount> SearchAccount(string searchHeadline)
        {
            var list = new List<SystemAccount>();
            try
            {
                using var db = new FunewsManagementContext();
                if (string.IsNullOrWhiteSpace(searchHeadline))
                {
                    list = db.SystemAccounts.ToList();
                }
                else
                {
                    list = db.SystemAccounts.Where(n => n.AccountName.Contains(searchHeadline)).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return list;
        }
    }
}
