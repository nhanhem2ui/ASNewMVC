using BussinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ITagService
    {
        void SaveTag(Tag p);
        void DeleteTag(int id);
        void UpdateTag(Tag p);
        List<Tag> GetTags();
        Tag GetTagById(int id);
    }
}
