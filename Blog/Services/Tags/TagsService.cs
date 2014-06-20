using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Services;
using Blog.DAL;
using Blog.Models;
using System.Data.Entity;

namespace Blog.Services
{
    public class TagsService : ITagsService
    {
        private DbContext _db = null;

        public TagsService(DbContext db)
        {
            _db = db;
        }

        public void Parse(String tags, int articleID)
        {
            _db.Set<TagModel>().RemoveRange(_db.Set<TagModel>().Where(p => p.ArticleID == articleID));
            var tagsList = tags.Split(',').ToList();

            for (int i = 0; i < tagsList.Count(); i++)
            {
                var tagModel = new TagModel()
                {
                    ArticleID = articleID,
                    Name = tagsList[i].Trim()
                };

                _db.Set<TagModel>().Add(tagModel);
            }

            _db.SaveChanges();
        }

        public string GetStringByArticleID(int articleID)
        {
            var tags = String.Empty;
            var splittedTags = _db.Set<TagModel>()
                .Where(p => p.ArticleID == articleID)
                .ToList();

            for(int i=0; i<splittedTags.Count(); i++)
            {
                tags += splittedTags[i].Name + ", ";
            }

            if (splittedTags.Count > 0)
                tags = tags.Remove(tags.Length - 2, 2);

            return tags;
        }

        public List<String> GetListByArticleID(int articleID)
        {
            var tags = String.Empty;
            var splittedTags = _db.Set<TagModel>()
                .Where(p => p.ArticleID == articleID)
                .Select(p => p.Name)
                .ToList();

            return splittedTags;
        }

        public bool RemoveByArticleID(int articleID)
        {
            var tags = _db.Set<TagModel>().Where(p => p.ArticleID == articleID);
            _db.Set<TagModel>().RemoveRange(tags);

            _db.SaveChanges();
            return true;
        }

        public List<int> GetArticlesIDByTagName(String tag)
        {
            var articles = _db.Set<TagModel>().Where(p => p.Name == tag).Select(p => p.ArticleID).ToList();
            return articles;
        }


        public List<ViewModels.TagsModuleViewModel> GetMostPopularTags(int count)
        {
            var tagsList = new List<ViewModels.TagsModuleViewModel>();
            var list = _db.Set<TagModel>().ToList();

            for(int i=0; i<list.Count; i++)
            {
                if (!tagsList.Any(p => p.TagName == list[i].Name))
                {
                    var viewModel = new ViewModels.TagsModuleViewModel()
                    {
                        TagName = list[i].Name,
                        Size = 0,
                        Count = list.Count(p => p.Name == list[i].Name)
                    };

                    tagsList.Add(viewModel);
                }
            }

            return tagsList;
        }
    }
}