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
        List<ArticleViewModel> GetAll();
        void Remove(int id);
        bool Edit(ArticleViewModel viewModel);
        void SetArticleStatus(int id, bool status);
    }
}
