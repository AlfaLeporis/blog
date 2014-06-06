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
        public CategoriesService()
        {

        }

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

                var model = db.Categories.FirstOrDefault(p => p.ID == viewModel.ID);

                if (model == null)
                    return false;

                model.InjectFrom(viewModel);

                db.SaveChanges();
            }

            return true;
        }

        public bool Remove(int id)
        {
            using (var db = new DatabaseContext())
            {
                if (!db.Categories.Any(p => p.ID == id))
                    return false;

                var element = db.Categories.FirstOrDefault(p => p.ID == id);

                if (element == null)
                    return false;

                db.Categories.Remove(element);

                db.SaveChanges();
            }

            return true;
        }

        public CategoryViewModel Get(int id)
        {
            using (var db = new DatabaseContext())
            {
                var element = db.Categories.FirstOrDefault(p => p.ID == id);

                if (element == null)
                    return null;

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


        public List<CategoriesModuleViewModel> GetAllWithDetails()
        {
            using(var db = new DatabaseContext())
            {
                var viewModel = new List<CategoriesModuleViewModel>();
                var categories = db.Categories.ToList();

                for(int i=0; i< categories.Count; i++)
                {
                    var vm = new CategoriesModuleViewModel();
                    vm.Name = categories[i].Title;

                    var categoryID = categories[i].ID;
                    
                    var count = db.Articles.Count(p => p.CategoryID == categoryID);
                    vm.Count = count;

                    viewModel.Add(vm);
                }

                return viewModel;
            }
        }


        public int GetIDByName(string name)
        {
            using(var db = new DatabaseContext())
            {
                if (!db.Categories.Any(p => p.Title == name))
                    return -1;

                return db.Categories.First(p => p.Title == name).ID;
            }
        }
    }
}