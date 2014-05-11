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
        public void Parse(string tags, int articleID)
        {
            using(var db = new DatabaseContext())
            {
                var splittedTags = tags.Split(',');

                db.Tags.RemoveRange(db.Tags.Where(p => p.ArticleID == articleID));

                for(int i=0; i<splittedTags.Count(); i++)
                {
                    var tagModel = new TagModel()
                    {
                        ArticleID = articleID,
                        Name = splittedTags[i].Trim()
                    };

                    db.Tags.Add(tagModel);
                }

                db.SaveChanges();
            }
        }

        public string GetByArticleID(int articleID)
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

        public void RemoveByArticleID(int articleID)
        {
            using(var db = new DatabaseContext())
            {
                var tags = db.Tags.Where(p => p.ArticleID == articleID);
                db.Tags.RemoveRange(tags);

                db.SaveChanges();
            }
        }
    }
}