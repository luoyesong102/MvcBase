using System;
using System.Linq;
using System.Linq.Expressions;

namespace Esmart.Permission.Application.Data
{
    public static class QueryExtensions
    {
        public static IQueryable<T> SortBy<T>(this IQueryable<T> source, string sortExpression)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            string sortDirection;
            string propertyName;

            sortExpression = sortExpression.Trim();
            int spaceIndex = sortExpression.Trim().IndexOf(" ");
            if (spaceIndex < 0)
            {
                propertyName = sortExpression;
                sortDirection = "ASC";
            }
            else
            {
                propertyName = sortExpression.Substring(0, spaceIndex);
                sortDirection = sortExpression.Substring(spaceIndex + 1).Trim();
            }

            if (String.IsNullOrEmpty(propertyName))
            {
                return source;
            }

            ParameterExpression parameter = Expression.Parameter(source.ElementType, String.Empty);
            MemberExpression property = Expression.Property(parameter, propertyName);
            LambdaExpression lambda = Expression.Lambda(property, parameter);

            string methodName = sortDirection.Equals("ASC", StringComparison.OrdinalIgnoreCase) ? "OrderBy" : "OrderByDescending";

            Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                                new[] { source.ElementType, property.Type },
                                                source.Expression, Expression.Quote(lambda));

            return source.Provider.CreateQuery<T>(methodCallExpression);
        }

    }
}
