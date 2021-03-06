﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.ViewModels;
using Blog.Infrastructure;

namespace Blog.Services
{
    public interface ICategoriesService
    {
        bool Add(CategoryViewModel viewModel);
        bool Edit(CategoryViewModel viewModel);
        bool Remove(int id);
        CategoryViewModel Get(int id);
        List<CategoryViewModel> GetAll();
        List<CategoriesModuleViewModel> GetAllWithDetails();
        int GetIDByName(String name);
        int GetIDByAlias(String alias);
        CategoryViewModel GetByAlias(string alias);
    }
}
