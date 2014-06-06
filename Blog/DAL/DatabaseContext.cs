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
        public DbSet<SiteModel> Sites { get; set; }

        public DatabaseContext() : base("MainDBConnection")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DatabaseContext>());
            Database.CreateIfNotExists();
            Database.Initialize(true);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            SetTableNames(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetTableNames(DbModelBuilder modelBuilder)
        {
            var dbType = typeof(DatabaseContext);
            var builderType = typeof(DbModelBuilder);
            var entities = dbType.GetProperties()
                .Where(p => p.PropertyType.Name == "DbSet`1")
                .ToList();
            
            for(int i=0; i<entities.Count(); i++)
            {
                var configMethod = builderType.GetMethod("Entity");

                var modelType = entities[i].PropertyType.GenericTypeArguments.First();
                configMethod = configMethod.MakeGenericMethod(modelType);
                var entityConfig = configMethod.Invoke(modelBuilder, null);
                var entityConfigType = entityConfig.GetType();           

                var toTableMethod = entityConfigType.GetMethod("ToTable", new[] {typeof(string)});
                toTableMethod.Invoke(entityConfig, new[] {entities[i].Name});
            }
        }
    }
}