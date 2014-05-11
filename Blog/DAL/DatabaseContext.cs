using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Blog.Models;

namespace Blog.DAL
{
    public class DatabaseContext : DbContext
    {
        public DbSet<ArticleModel> Articles { get; set; }
        public DbSet<CommentModel> Comments { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<TagModel> Tags { get; set; }

        public DatabaseContext() : base("MainDBConnection")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DatabaseContext>());
            Database.CreateIfNotExists();
            Database.Initialize(true);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticleModel>().ToTable("Articles");
            modelBuilder.Entity<ArticleModel>().HasKey(p => p.ID);

            modelBuilder.Entity<CommentModel>().ToTable("Comments");
            modelBuilder.Entity<CommentModel>().HasKey(p => p.ID);

            modelBuilder.Entity<UserModel>().ToTable("Users");
            modelBuilder.Entity<UserModel>().HasKey(p => p.ID);

            modelBuilder.Entity<CategoryModel>().ToTable("Categories");
            modelBuilder.Entity<CategoryModel>().HasKey(p => p.ID);

            modelBuilder.Entity<TagModel>().ToTable("Tags");
            modelBuilder.Entity<TagModel>().HasKey(p => p.ID);

            base.OnModelCreating(modelBuilder);
        }
    }
}