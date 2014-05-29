using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.ViewModels;

namespace Blog.Services
{
    public interface ICommentsService
    {
        bool Add(CommentViewModel viewModel);
        bool Edit(CommentViewModel viewModel);
        void Remove(int id);
        CommentViewModel Get(int id);
        List<CommentViewModel> GetAll();
        List<CommentViewModel> GetByArticleID(int id);
    }
}
