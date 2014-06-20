using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.ViewModels;
using Blog.Models;

namespace Blog.Services
{
    public interface ICommentsService
    {
        bool Add(CommentViewModel viewModel);
        bool Edit(CommentViewModel viewModel);
        bool Remove(int id);
        CommentViewModel Get(int id);
        List<CommentViewModel> GetAll();
        List<CommentViewModel> GetByTargetID(int id, CommentTarget target);
        List<CommentViewModel> GetRecentComments(int count);
        List<CommentViewModel> GetByUserID(int id);
    }
}
