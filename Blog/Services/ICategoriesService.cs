using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.ViewModels;

namespace Blog.Services
{
    public interface ICategoriesService
    {
        bool Add(CategoryViewModel viewModel);
        bool Edit(CategoryViewModel viewModel);
        void Remove(int id);
        CategoryViewModel Get(int id);
        List<CategoryViewModel> GetAll();
    }
}
