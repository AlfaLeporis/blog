using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.Practices.Unity;
using Blog.Infrastructure;

namespace Blog.Areas.Administrator.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CategoriesController : Controller
    {
        private ICategoriesService _categoriesService = null;

        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        [HttpGet]
        public ActionResult Categories()
        {
            var allCategories = _categoriesService.GetAll();
            return View(allCategories);
        }

        [HttpGet]
        public ActionResult EditCategory(int? id)
        {
            if (id.HasValue)
            {
                var viewModel = _categoriesService.Get(id.Value);
                return View(viewModel);
            }

            return View();
        }

        [HttpPost]
        public ActionResult EditCategory(CategoryViewModel viewModel)
        {
            bool result;

            if (!viewModel.ID.HasValue)
                result = _categoriesService.Add(viewModel);
            else
                result = _categoriesService.Edit(viewModel);

            if (!result)
            {
                TempData.Add("ErrorMsg", "Wpisana nazwa kategorii już istnieje! Spróbuj ponownie.");
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Categories");
            }
        }

        [HttpGet]
        public ActionResult RemoveCategory(int id)
        {
            bool result = _categoriesService.Remove(id);

            if (!result)
                throw new Exception("Usunięcie katogorii (" + id + ") nie powiodło się.");

            return RedirectToAction("Categories");
        }

        public ActionResult PartialCategoriesList(int? selectedCategory)
        {
            var categories = _categoriesService.GetAll();
            ViewBag.SelectedCategoryID = selectedCategory;
            return PartialView("_CategoriesList", categories);
        }
	}
}