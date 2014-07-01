using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Blog.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DbSet<UsersInRoles> webpages_UsersInRoles { get; set; }
        public DbSet<Roles> webpages_Roles { get; set; }
        public DbSet<Membership> webpages_Membership { get; set; }

        public DatabaseContext() : base("MainDBConnection")
        {
            Database.CreateIfNotExists();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            SetTableNames(modelBuilder);

            modelBuilder.Entity<Membership>().HasKey(p => p.UserId);
            modelBuilder.Entity<Membership>().Property(p => p.UserId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<Roles>().HasKey(p => p.RoleId);
            modelBuilder.Entity<Roles>().Property(p => p.RoleId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<UsersInRoles>().HasKey(p => p.UserId).HasKey(p => p.RoleId);
            modelBuilder.Entity<UsersInRoles>().Property(p => p.UserId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            modelBuilder.Entity<UsersInRoles>().Property(p => p.RoleId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //modelBuilder.Entity<ArticleModel>().HasKey(p => p.ID);
            //modelBuilder.Entity<ArticleModel>().Property(p => p.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //modelBuilder.Entity<CommentModel>().HasKey(p => p.ID);
            //modelBuilder.Entity<CommentModel>().Property(p => p.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //modelBuilder.Entity<UserModel>().HasKey(p => p.ID);
            //modelBuilder.Entity<UserModel>().Property(p => p.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //modelBuilder.Entity<CategoryModel>().HasKey(p => p.ID);
            //modelBuilder.Entity<CategoryModel>().Property(p => p.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //modelBuilder.Entity<TagModel>().HasKey(p => p.ID);
            //modelBuilder.Entity<TagModel>().Property(p => p.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //modelBuilder.Entity<SiteModel>().HasKey(p => p.ID);
            //modelBuilder.Entity<SiteModel>().Property(p => p.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
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

        //public override int SaveChanges()
        //{
        //    var entries = ChangeTracker.Entries().Where(p => p.State == EntityState.Added).Select(p => p.Entity).ToList();

        //    for (int i = 0; i < entries.Count; i++ )
        //    {
        //        var property = entries[i].GetType().GetProperties().Where(p => p.CustomAttributes.Any(q => q.AttributeType == typeof(KeyAttribute))).First();
        //        property.SetValue(entries[i], (int)DateTime.Now.TimeOfDay.TotalSeconds);
        //    }

        //    return base.SaveChanges();
        //}

    }
}