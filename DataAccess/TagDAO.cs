using BussinessObject;

namespace DataAccess
{
    public class TagDAO
    {
        public List<Tag> GetTags()
        {
            var listTags = new List<Tag>();
            try
            {
                using var db = new FunewsManagementContext();
                listTags = db.Tags.ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listTags;
        }

        public void SaveTag(Tag p)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Tags.Add(p);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void UpdateTag(Tag p)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Entry<Tag>(p).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void DeleteTag(Tag p)
        {
            try
            {
                using var context = new FunewsManagementContext();
                var p1 = context.Tags.SingleOrDefault(c => c.TagId == p.TagId);
                context.Tags.Remove(p1);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Tag GetTagById(int id)
        {
            using var db = new FunewsManagementContext();
            return db.Tags.FirstOrDefault(c => c.TagId.Equals(id));
        }
    }
}
