using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Blog.Services;
using Blog.DAL;
using Blog.Models;

namespace Blog.Services
{
    public class TagsService : ITagsService
    {
        public void Parse(String tags, int articleID)
        {
            using(var db = new DatabaseContext())
            {
                db.Tags.RemoveRange(db.Tags.Where(p => p.ArticleID == articleID));
                var tagsList = tags.Split(',').ToList();

                for (int i = 0; i < tagsList.Count(); i++)
                {
                    var tagModel = new TagModel()
                    {
                        ArticleID = articleID,
                        Name = tagsList[i].Trim()
                    };

                    db.Tags.Add(tagModel);
                }

                db.SaveChanges();
            }
        }

        public string GetStringByArticleID(int articleID)
        {
            using(var db = new DatabaseContext())
            {
                var tags = String.Empty;
                var splittedTags = db.Tags
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
        }

        public List<String> GetListByArticleID(int articleID)
        {
            using (var db = new DatabaseContext())
            {
                var tags = String.Empty;
                var splittedTags = db.Tags
                    .Where(p => p.ArticleID == articleID)
                    .Select(p => p.Name)
                    .ToList();

                return splittedTags;
            }
        }

        public void RemoveByArticleID(int articleID)
        {
            using(var db = new DatabaseContext())
            {
                var tags = db.Tags.Where(p => p.ArticleID == articleID);
                db.Tags.RemoveRange(tags);

                db.SaveChanges();
            }
        }

        public List<int> GetArticlesIDByTagName(String tag)
        {
            using(var db = new DatabaseContext())
            {
                var articles = db.Tags.Where(p => p.Name == tag).Select(p => p.ArticleID).ToList();
                return articles;
            }
        }


        public List<ViewModels.TagsModuleViewModel> GetMostPopularTags(int count)
        {
            using(var db = new DatabaseContext())
            {
                var tagsList = new List<ViewModels.TagsModuleViewModel>();
                var list = db.Tags.ToList();

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
}