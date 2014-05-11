using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.ViewModels;
using Blog.DAL;
using Blog.Models;
using Omu.ValueInjecter;
using Blog.App_Start;

namespace Blog.Services
{
    public class CategoriesService : ICategoriesService
    {
        public bool Add(CategoryViewModel viewModel)
        {
            using(var db = new DatabaseContext())
            {
                if (db.Categories.Any(p => p.Title == viewModel.Title))
                    return false;

                var model = new CategoryModel();
                model.InjectFrom(viewModel);

                db.Categories.Add(model);
                db.SaveChanges();
            }

            return true;
        }

        public bool Edit(CategoryViewModel viewModel)
        {
            using(var db = new DatabaseContext())
            {
                if (db.Categories.Any(p => p.Title == viewModel.Title))
                    return false;

                var model = db.Categories.First(p => p.ID == viewModel.ID);
                model.InjectFrom(viewModel);

                db.SaveChanges();
            }

            return true;
        }

        public void Remove(int id)
        {
            using (var db = new DatabaseContext())
            {
                if (!db.Categories.Any(p => p.ID == id))
                    return;

                var element = db.Categories.First(p => p.ID == id);
                db.Categories.Remove(element);

                db.SaveChanges();
            }
        }

        public CategoryViewModel Get(int id)
        {
            using (var db = new DatabaseContext())
            {
                var element = db.Categories.FirstOrDefault(p => p.ID == id);

                var viewModel = new CategoryViewModel();
                viewModel.InjectFrom<CustomInjection>(element);

                return viewModel;
            }
        }

        public List<CategoryViewModel> GetAll()
        {
            using (var db = new DatabaseContext())
            {
                var categories = db.Categories.ToList();
                var viewModels = new List<CategoryViewModel>();

                for(int i=0; i<categories.Count; i++)
                {
                    var vm = new CategoryViewModel();
                    vm.InjectFrom<CustomInjection>(categories[i]);
           
                    viewModels.Add(vm);
                }

                return viewModels;
            }
        }
    }
}