using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Esmart.Permission.Web.MVC
{
    public static class TpoDateTimeInputExtensions
    {
        public static MvcHtmlString DateTimePickerFor<TModel, TProperty>( this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, DateTimeCategory category )
        {
            return DateTimePickerFor(htmlHelper, expression, category, null, null);
        }

        public static MvcHtmlString DateTimePickerFor<TModel, TProperty>( this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, DateTimeCategory category, object htmlAttributes )
        {
            return DateTimePickerFor(htmlHelper, expression, category, null, htmlAttributes);
        }

        public static MvcHtmlString DateTimePickerFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, DateTimeCategory category, CustomizeDateTimePicker customize)
        {
            return DateTimePickerFor(htmlHelper, expression, category, customize, null);
        }

        public static MvcHtmlString DateTimePickerFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, DateTimeCategory category, CustomizeDateTimePicker customize,
            object htmlAttributes)
        {
            if( expression == null )
            {
                throw new ArgumentNullException( "expression" );
            }

            var metadata = ModelMetadata.FromLambdaExpression( expression, htmlHelper.ViewData );

            return DateTimePickerHelper(htmlHelper, metadata, ExpressionHelper.GetExpressionText(expression), category,
                customize, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString DateTimePicker(this HtmlHelper htmlHelper, string name, DateTimeCategory category)
        {
            return DateTimePicker(htmlHelper, name, category, null, null);
        }

        public static MvcHtmlString DateTimePicker(this HtmlHelper htmlHelper, string name, DateTimeCategory category,
            object htmlAttributes)
        {
            return DateTimePicker(htmlHelper, name, category, null, htmlAttributes);
        }

        public static MvcHtmlString DateTimePicker(this HtmlHelper htmlHelper, string name, DateTimeCategory category,
            CustomizeDateTimePicker customize )
        {
            return DateTimePicker(htmlHelper, name, category, customize, null);
        }

        public static MvcHtmlString DateTimePicker(this HtmlHelper htmlHelper, string name, DateTimeCategory category,
            CustomizeDateTimePicker customize,
            object htmlAttributes)
        {
            return DateTimePickerHelper(htmlHelper, null, name, category,
                customize, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        private static MvcHtmlString DateTimePickerHelper(HtmlHelper htmlHelper, ModelMetadata metadata,
            string name, DateTimeCategory category, CustomizeDateTimePicker customize,
            IDictionary<string, object> htmlAttributes)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName( name );
            if( string.IsNullOrEmpty( fullName ) )
            {
                throw new ArgumentNullException("name");
            }

            var value = metadata == null
                ? TpoHtmlHelper.GetModelStateValue(htmlHelper.ViewContext, fullName, typeof (object))
                : metadata.Model;

            var datePickerIndentity = "div_" + Guid.NewGuid( ).ToString( ).Replace( "-", "" ).Substring( 0, 7 );
            var divTag = GenerateDateTimePickerDiv(htmlHelper, datePickerIndentity, name, value, category,
                customize ?? new CustomizeDateTimePicker(), htmlAttributes);

            return new MvcHtmlString( divTag );
        }


        private static string GenerateDateTimePickerDiv( HtmlHelper htmlHelper, string datePickerIndentity, string name, object value, DateTimeCategory category, CustomizeDateTimePicker customize, IDictionary<string, object> htmlAttributes  )
        {
            var divTag = new TagBuilder( "div" );
            divTag.MergeAttribute( "id", datePickerIndentity );
            divTag.AddCssClass( CustomizeDateTimePicker.GetDateTimePickerCssClass( category ) );
            divTag.MergeAttribute( "data-date-format", CustomizeDateTimePicker.DateTimePickerFormatsDictionary[category] );
            divTag.MergeAttribute( "data-link-format", CustomizeDateTimePicker.DateTimePickerFormatsDictionary[category] );
            divTag.MergeAttribute( "data-link-field", name );
            //divTag.MergeAttribute("data-date", value);

            var dateTextbox = htmlHelper.TextBox( name, CustomizeDateTimePicker.ConvertValueFormat( value, category ), customize.GetDateTextBoxAttributes( htmlAttributes ) );

            var firstSpan = new TagBuilder( "span" );
            firstSpan.AddCssClass( CustomizeDateTimePicker.IconSpanCss );
            var removeSpan = new TagBuilder( "span" );
            removeSpan.AddCssClass( CustomizeDateTimePicker.RemoveIcon );

            var secondSpan = new TagBuilder( "span" );
            secondSpan.AddCssClass( CustomizeDateTimePicker.IconSpanCss );
            var addSpan = new TagBuilder( "span" );
            addSpan.AddCssClass( CustomizeDateTimePicker.AddIconsDictionary[category] );

            firstSpan.InnerHtml = removeSpan.ToString(TagRenderMode.Normal);
            secondSpan.InnerHtml = addSpan.ToString(TagRenderMode.Normal);

            var script = GenerateDateTimePickerScript( datePickerIndentity, category, customize );

            divTag.InnerHtml = dateTextbox.ToHtmlString() + firstSpan.ToString(TagRenderMode.Normal) +
                               secondSpan.ToString(TagRenderMode.Normal) + script;

            return divTag.ToString(TagRenderMode.Normal);
        }

        private static string GenerateDateTimePickerScript(string datePickerIndentity, DateTimeCategory category,
            CustomizeDateTimePicker customize)
        {
            var script = new TagBuilder( "script" );
            script.MergeAttribute( "type", "text/javascript" );

            var setting = customize.GetDateTimePickerSetting(category, datePickerIndentity);
            script.InnerHtml = string.Format( "var datetimePickerInit = function (){{$('#{0}').datetimepicker({{ {1} }});$('#{0}').on('remove', function(){{$('#{0}_picker').remove();}});}};if($('#{0}').datetimepicker){{datetimePickerInit();}}else{{$(function(){{datetimePickerInit();}});}}", datePickerIndentity, setting );
            
            return script.ToString(TagRenderMode.Normal);
        }
    }
}
