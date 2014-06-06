using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.ViewModels;

namespace Blog.Services
{
    public interface ITagsService
    {
        void Parse(String tags, int articleID);
        String GetStringByArticleID(int articleID);
        List<String> GetListByArticleID(int articleID);
        bool RemoveByArticleID(int articleID);
        List<int> GetArticlesIDByTagName(String tag);
        List<TagsModuleViewModel> GetMostPopularTags(int count);
    }
}
