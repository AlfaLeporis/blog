using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.ViewModels;
using Blog.DAL;
using Blog.Models;
using Omu.ValueInjecter;
using Blog.App_Start;
using System.Data.Entity;
using Blog.Infrastructure;

namespace Blog.Services
{
    public class CategoriesService : ICategoriesService
    {
        private DbContext _db = null;

        public CategoriesService(DbContext db)
        {
            _db = db;
        }

        public bool Add(CategoryViewModel viewModel)
        {
            if (_db.Set<CategoryModel>().Any(p => p.Title == viewModel.Title))
                return false;

            var model = new CategoryModel();
            model.InjectFrom<CustomInjection>(viewModel);
            model.IsRemoved = false;

            _db.Set<CategoryModel>().Add(model);
            _db.SaveChanges();

            return true;
        }

        public bool Edit(CategoryViewModel viewModel)
        {
            var model = _db.Set<CategoryModel>().FirstOrDefault(p => p.ID == viewModel.ID);

            if (model == null)
                return false;

            model.InjectFrom<CustomInjection>(viewModel);

            _db.SaveChanges();

            return true;
        }

        private CategoryViewModel ConvertModel(CategoryModel model)
        {
            var viewModel = new CategoryViewModel();
            viewModel.InjectFrom<CustomInjection>(model);

            return viewModel;
        }

        public bool Remove(int id)
        {
            var element = _db.Set<CategoryModel>().FirstOrDefault(p => p.ID == id);

            if (element == null)
                return false;

            element.IsRemoved = true;
            _db.SaveChanges();

            return true;
        }

        public CategoryViewModel Get(int id)
        {
            var element = _db.Set<CategoryModel>().FirstOrDefault(p => p.ID == id);

            if (element == null)
                return null;

            var viewModel = ConvertModel(element);

            return viewModel;
        }

        public List<CategoryViewModel> GetAll()
        {
            var categories = _db.Set<CategoryModel>()
                .Where(p => !p.IsRemoved)
                .OrderBy(p => p.ID)
                .ToList();
            var viewModels = new List<CategoryViewModel>();

            for(int i=0; i<categories.Count; i++)
            {
                var vm = ConvertModel(categories[i]);
                viewModels.Add(vm);
            }

            return viewModels;
        }


        public List<CategoriesModuleViewModel> GetAllWithDetails()
        {
            var viewModel = new List<CategoriesModuleViewModel>();
            var categories = _db.Set<CategoryModel>().Where(p => !p.IsRemoved).ToList();

            for(int i=0; i< categories.Count; i++)
            {
                var vm = new CategoriesModuleViewModel();
                vm.Name = categories[i].Title;

                var categoryID = categories[i].ID;
                    
                var count = _db.Set<ArticleModel>().Count(p => p.CategoryID == categoryID);
                vm.Count = count;

                viewModel.Add(vm);
            }

            return viewModel;
        }


        public int GetIDByName(string name)
        {
            if (!_db.Set<CategoryModel>().Any(p => p.Title == name))
                return -1;

            return _db.Set<CategoryModel>().First(p => p.Title == name).ID;
        }
    }
}