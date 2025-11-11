using BussinessObject;
using DataAccess;

namespace Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly TagDAO _dao;

        public TagRepository(TagDAO dao)
        {
            _dao = dao;
        }

        public void DeleteTag(Tag p) => _dao.DeleteTag(p);
        public Tag GetTagById(int id) => _dao.GetTagById(id);
        public List<Tag> GetTags() => _dao.GetTags();
        public void SaveTag(Tag p) => _dao.SaveTag(p);
        public void UpdateTag(Tag p) => _dao.UpdateTag(p);
    }
}