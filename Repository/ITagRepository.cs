using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ITagRepository
    {
        void SaveTag(Tag p);
        void DeleteTag(Tag p);
        void UpdateTag(Tag p);
        List<Tag> GetTags();
        Tag GetTagById(int id);
    }
}
