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
        ArticleViewModel Get(int id, bool shortVersion);
        ArticleViewModel GetByAlias(String alias);
        List<ArticleViewModel> GetAll(bool shortVersion);
        bool Remove(int id);
        bool Edit(ArticleViewModel viewModel);
        bool SetArticleStatus(int id, bool status);
        List<ViewModels.ArticleViewModel> GetByTagName(String tag, bool shortVersion);
        List<ArticleViewModel> GetByCategoryName(String name, bool shortVersion);
        List<ArticleViewModel> GetByDate(String date);
    }
}
