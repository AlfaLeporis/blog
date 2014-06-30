using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO.Compression;
using Blog.Services;
using Blog.DAL;
using System.Data.Entity;
using Blog.ViewModels;
using System.IO;

namespace Blog.Areas.Administrator.Controllers
{
    public class BackupsController : Controller
    {
        private DatabaseContext _db = null;
        private IBackupsService _backupsService = null;

        public BackupsController(DatabaseContext db,
                                IBackupsService backupsService)
        {
            _db = db;
            _backupsService = backupsService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var backups = _backupsService.GetAll();

            var viewModel = new BackupSiteViewModel();
            viewModel.Backups = backups.OrderByDescending(p => p.CreateDate).ToList();
            viewModel.Count = backups.Count;
            viewModel.LastBackup = backups.Max(p => p.CreateDate);

            return View(viewModel);
        }

        public ActionResult CreateBackup()
        {
            var result = _backupsService.CreateNew();
            if (!result)
                throw new Exception("Błąd w trakcie próby utworzenia kopii zapasowej.");

            return RedirectToAction("Index");
        }

        public ActionResult RestoreBackup(String id)
        {
            var path = Server.MapPath("/Backups/");
            var files = Directory.GetFiles(path);
            var backupPath = String.Empty;

            for (int i = 0; i < files.Count(); i++)
            {
                var time = Directory.GetCreationTime(files[i]);
                if (time.Ticks.ToString() == id)
                    backupPath = files[i];
            }

            var result = _backupsService.Restore(backupPath);
            if (!result)
                throw new Exception("Błąd w trakcie próby przywrócenia kopii zapasowej");

            return RedirectToAction("Index");
        }
    }
}