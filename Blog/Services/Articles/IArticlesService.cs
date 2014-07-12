using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.ViewModels;
using Blog.Infrastructure;

namespace Blog.Services
{
    public interface IArticlesService
    {
        bool Add(ArticleViewModel viewModel);
        bool Add(ArticleViewModel viewModel, int version, int? parent);
        ArticleViewModel Get(int id, bool shortVersion);
        ArticleViewModel GetByAlias(String alias, bool shortVersion);
        List<ArticleViewModel> GetAll(bool shortVersion, ArticleSiteAccessSettings settings, ref PaginationSettings pagination);
        List<ArticleViewModel> GetVersionsByID(int articleID);
        bool Remove(int id);
        bool Edit(ArticleViewModel viewModel);
        bool SetArticleStatus(int id, bool status);
        List<ArticleViewModel> GetByTagName(String tag, bool shortVersion, ref PaginationSettings pagination);
        List<ArticleViewModel> GetByCategoryAlias(String name, bool shortVersion, ref PaginationSettings pagination);
        List<ArticleViewModel> GetByDate(String date, bool shortVersion, ref PaginationSettings pagination);
    }
}
