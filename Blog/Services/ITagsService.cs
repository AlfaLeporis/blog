using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Services
{
    public interface ITagsService
    {
        void Parse(String tags, int articleID);
        String GetByArticleID(int articleID);
        void RemoveByArticleID(int articleID);
    }
}
