using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Esmart.Permission.Web.MVC
{
    public static class TpoSelectExtensions
    {
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>( this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression )
        {
            return EnumDropDownListFor( htmlHelper, expression, optionLabel: null );
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>( this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression, object htmlAttributes )
        {
            return EnumDropDownListFor( htmlHelper, expression, optionLabel: null, htmlAttributes: htmlAttributes );
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>( this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression, IDictionary<string, object> htmlAttributes )
        {
            return EnumDropDownListFor( htmlHelper, expression, optionLabel: null, htmlAttributes: htmlAttributes );
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>( this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression, string optionLabel )
        {
            return EnumDropDownListFor( htmlHelper, expression, optionLabel, (IDictionary<string, object>)null );
        }


        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>( this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression, string optionLabel, object htmlAttributes )
        {
            return EnumDropDownListFor( htmlHelper, expression, optionLabel,
                HtmlHelper.AnonymousObjectToHtmlAttributes( htmlAttributes ) );
        }

        // Unable to constrain TEnum.  Cannot include IComparable, IConvertible, IFormattable because Nullable<T> does
        // not implement those interfaces (and Int32 does).  Enum alone is not compatible with expression restrictions
        // because that requires a cast from all enum types.  And the struct generic constraint disallows passing a
        // Nullable<T> expression.
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>( this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TEnum>> expression, string optionLabel, IDictionary<string, object> htmlAttributes )
        {
            if( expression == null )
            {
                throw new ArgumentException( "expression" );
            }

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression( expression, htmlHelper.ViewData );
            if( metadata == null )
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, "Unable to determine ModelMetadata for expression '{0}'.",
                        expression.ToString()), "expression");
            }

            if( metadata.ModelType == null )
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, "Unable to determine type of expression '{0}'.",
                        expression.ToString()), "expression");
            }

            if( !TpoEnumHelper.IsValidForEnumHelper( metadata.ModelType ) )
            {
                string formatString;
                if( TpoEnumHelper.HasFlags( metadata.ModelType ) )
                {
                    formatString = "Return type '{0}' is not supported. Type must not have a '{1}' attribute.";
                }
                else
                {
                    formatString = "Return type '{0}' is not supported.";
                }

                throw new ArgumentException( string.Format(CultureInfo.CurrentCulture,  formatString, metadata.ModelType.FullName, "Flags"), "expression" );
            }

            // Run through same processing as SelectInternal() to determine selected value and ensure it is included
            // in the select list.
            string expressionName = ExpressionHelper.GetExpressionText( expression );
            string expressionFullName =
                htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName( expressionName );
            Enum currentValue = null;
            if( !String.IsNullOrEmpty( expressionFullName ) )
            {
                currentValue = TpoHtmlHelper.GetModelStateValue(htmlHelper.ViewContext, expressionFullName, metadata.ModelType ) as Enum;
            }

            if( currentValue == null && !String.IsNullOrEmpty( expressionName ) )
            {
                // Ignore any select list (enumerable with this name) in the view data
                currentValue = htmlHelper.ViewData.Eval( expressionName ) as Enum;
            }

            if( currentValue == null )
            {
                currentValue = metadata.Model as Enum;
            }

            IList<SelectListItem> selectList = TpoEnumHelper.GetSelectList( metadata.ModelType, currentValue );
            if( !String.IsNullOrEmpty( optionLabel ) && selectList.Count != 0 && String.IsNullOrEmpty( selectList[0].Text ) )
            {
                // Were given an optionLabel and the select list has a blank initial slot.  Combine.
                selectList[0].Text = optionLabel;

                // Use the option label just once; don't pass it down the lower-level helpers.
                optionLabel = null;
            }

            return htmlHelper.DropDownListFor(expression, selectList, optionLabel, htmlAttributes);
        }
    }
}
