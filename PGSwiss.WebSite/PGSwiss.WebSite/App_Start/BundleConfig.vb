Imports System.Web
Imports System.Web.Optimization

Public Module BundleConfig
    ' For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
    Public Sub RegisterBundles(bundles As BundleCollection)
        bundles.Add(New ScriptBundle("~/bundles/jquery").Include(
                    "~/Scripts/jquery-{version}.js"))

        bundles.Add(New ScriptBundle("~/bundles/knockout").Include(
            "~/Scripts/knockout-{version}.js",
            "~/Scripts/knockout.validation.js"))

        bundles.Add(New ScriptBundle("~/bundles/app").Include(
            "~/Scripts/app/ajaxPrefilters.js",
            "~/Scripts/app/app.bindings.js",
            "~/Scripts/app/app.datamodel.js",
            "~/Scripts/app/app.viewmodel.js",
            "~/Scripts/app/home.viewmodel.js",
            "~/Scripts/app/login.viewmodel.js",
            "~/Scripts/app/register.viewmodel.js",
            "~/Scripts/app/registerExternal.viewmodel.js",
            "~/Scripts/app/manage.viewmodel.js",
            "~/Scripts/app/userInfo.viewmodel.js",
            "~/Scripts/app/_run.js"))

        ' Use the development version of Modernizr to develop with and learn from. Then, when you're
        ' ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
        bundles.Add(New ScriptBundle("~/bundles/modernizr").Include(
            "~/Scripts/modernizr-*"))

        bundles.Add(New ScriptBundle("~/bundles/bootstrap").Include(
            "~/Scripts/bootstrap.js",
            "~/Scripts/respond.js"))

        bundles.Add(New StyleBundle("~/Content/css").Include(
            "~/Content/bootstrap.css",
            "~/Content/Site.css"))
    End Sub
End Module
