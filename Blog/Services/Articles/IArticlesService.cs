using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.ViewModels;

namespace Blog.Services
{
    public interface IArticlesService
    {
        bool Add(ArticleViewModel viewModel);
        ArticleViewModel Get(int id);
        ArticleViewModel GetByAlias(String alias);
        ArticleViewModel GetShortVersion(int id);
        List<ArticleViewModel> GetAll();
        List<ArticleViewModel> GetAllShortVersion();
        void Remove(int id);
        bool Edit(ArticleViewModel viewModel);
        void SetArticleStatus(int id, bool status);
        List<ViewModels.ArticleViewModel> GetByTagName(String tag);
        List<ViewModels.ArticleViewModel> GetShortByTagName(String tag);
        List<ArticleViewModel> GetByCategoryName(String name);
        List<ArticleViewModel> GetShortByCategoryName(String name);
    }
}
