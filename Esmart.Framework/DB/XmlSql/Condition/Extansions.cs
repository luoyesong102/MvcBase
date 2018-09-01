using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Esmart.Framework.Condition
{
    public static class Extansions
    {
        public static bool In(this Type type, params Type[] types)
        {
            if (types == null || types.Length <= 0) return false;

            return types.Any(t => t == type);
        }

        /// <summary>
        ///  linq 表达式拼接 Extansion by nero
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1">表达式1</param>
        /// <param name="expr2">表达式1</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        ///  linq 表达式拼接 Extansion by nero
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1">表达式1</param>
        /// <param name="expr2">表达式1</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}
