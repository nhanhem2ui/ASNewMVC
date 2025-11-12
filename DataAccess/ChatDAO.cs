using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class ChatDAO
    {
        public List<Chat> GetChats()
        {
            var listChats = new List<Chat>();
            try
            {
                using var db = new FunewsManagementContext();
                listChats = db.Chats
                    .Include(c => c.Sender)
                    .Include(c => c.Receiver)
                    .OrderBy(c => c.Timestamp)
                    .ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listChats;
        }

        public void SaveChat(Chat p)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Chats.Add(p);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void UpdateChat(Chat p)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Entry<Chat>(p).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void DeleteChat(Chat p)
        {
            try
            {
                using var context = new FunewsManagementContext();
                var p1 = context.Chats.SingleOrDefault(c => c.ChatId == p.ChatId);
                if (p1 != null)
                {
                    context.Chats.Remove(p1);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Chat GetChatById(int id)
        {
            using var db = new FunewsManagementContext();
            return db.Chats
                .Include(c => c.Sender)
                .Include(c => c.Receiver)
                .FirstOrDefault(c => c.ChatId.Equals(id.ToString()));
        }
    }
}