using BussinessObject;
using Repository;

namespace Service
{
    public class TagService : ITagService
    {
        private readonly ITagRepository iTagRepository;

        public TagService(ITagRepository tagRepository)
        {
            iTagRepository = tagRepository;
        }

        public void DeleteTag(int id)
        {
            var p = iTagRepository.GetTagById(id);
            if (p != null) iTagRepository.DeleteTag(p);
        }

        public Tag GetTagById(int id)
        {
            return iTagRepository.GetTagById(id);
        }

        public List<Tag> GetTags()
        {
            return iTagRepository.GetTags();
        }

        public void SaveTag(Tag p)
        {
            iTagRepository.SaveTag(p);
        }

        public void UpdateTag(Tag p)
        {
            iTagRepository.UpdateTag(p);
        }
    }
}
