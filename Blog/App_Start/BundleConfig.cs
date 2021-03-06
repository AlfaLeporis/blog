﻿using System.Web;
using System.Web.Optimization;

namespace Blog
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/custom-scripts").Include("~/Scripts/confirm.js",
                                                                             "~/Scripts/showRemovedRefreshPage.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/admin-css").Include("~/Content/admin-site.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js"));
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                        "~/Content/bootstrap.css", 
                        "~/Content/bootstrap-theme.css"));

            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
                "~/Scripts/CKEditor/ckeditor.js",
                "~/Scripts/CKEditor/config.js"));

            bundles.Add(new ScriptBundle("~/bundles/syntax").Include(
                "~/Scripts/SyntaxHighlighter/shCore.js",
                "~/Scripts/SyntaxHighlighter/shBrushCSharp.js"));

            bundles.Add(new ScriptBundle("~/bundles/alias-generator").Include(
                "~/Scripts/alias-generator.js"));

            bundles.Add(new StyleBundle("~/Content/syntax").Include(
                "~/Content/SyntaxHighlighter/shCore.css",
                "~/Content/SyntaxHighlighter/shThemeDefault.css"));
        }
    }
}