using System.Web.Optimization;

namespace Esmart.Permission.Web
{
    /// <summary>
    /// bootbox.js弹出框jquery-2.1.4.js操作dom对象bootstrap.min.jsDIV布局jquery.ztree.all-3.5.js树main.js对象插件及重写对象magicsuggest-min.js下拉框
    /// jquery.validationEngine.js验证表单jquery.validationEngine-zh_CN.js验证规则bootstrap-datetimepicker.js bootstrap-datetimepicker.zh-CN.js时间控件
    /// tpo.vendor.js版本tpo.main.js自定义插件对象 moment.js日期处理js
    /// </summary>
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/script").Include(
                        "~/Scripts/jquery-2.1.4.js",
                        "~/Scripts/bootstrap.min.js",
                        "~/scripts/bootbox.js",
                        "~/Scripts/zTree/jquery.ztree.all-3.5.js",
                        "~/Scripts/jquery-ui-1.11.4.min.js",
                        "~/Scripts/main.js",
                        "~/Scripts/bootbox.js",
                        "~/scripts/magicsuggest-min.js",
                        "~/Scripts/validation.engine/jquery.validationEngine.js",
                        "~/Scripts/validation.engine/jquery.validationEngine-zh_CN.js",
                        "~/Scripts/boostrap-datetimepicker/bootstrap-datetimepicker.js",
                        "~/Scripts/boostrap-datetimepicker/locales/bootstrap-datetimepicker.zh-CN.js",
                        "~/scripts/app/tpo.vendor.js",
                        "~/scripts/app/tpo.main.js"
                     ));
           

            bundles.Add(new StyleBundle("~/Content/main").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/css/icons.css",
                      "~/Content/jquery_ui/jquery-ui-1.10.2.custom.min.css",
                      "~/Content/css/main.css",
                      "~/Content/css/custom.css",
                      "~/Content/css/plugins.css",
                      "~/Content/style.css",
                      "~/Content/layout.css",
                      "~/Content/validation.engine/validationEngine.jquery.css",
                      "~/Content/boostrap-datetimepicker/bootstrap-datetimepicker.css"));
#if DEBUG
            BundleTable.EnableOptimizations = true;
#else
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
