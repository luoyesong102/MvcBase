using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Esmart.Framework.Exceptions;

namespace Esmart.Framework.DB
{
    public static class SqlUtility
    {
        /// <summary>
        /// 定位SQL语句关键字所在位置（主关键字，不定位子查询中的关键字）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="keyword">关键字</param>
        /// <returns>关键字的索引位置</returns>
        public static int LocationSqlKeyWord(string sql, string keyword)
        {
            var keywordSplit = Regex.Split(sql, "\\s+" + keyword + "\\s+", RegexOptions.IgnoreCase);
            var keywords = Regex.Matches(sql, "\\s+" + keyword + "\\s+", RegexOptions.IgnoreCase);
            int partIndex = -1;
            string residueSql = "";
            for (int i = keywordSplit.Length - 1; i > 0; i--)
            {
                residueSql = CombineResidueString(keywordSplit, i, keywords);
                var leftBracketCount = residueSql.Count(ch => ch.Equals('('));
                var rightBracketCount = residueSql.Count(ch => ch.Equals(')'));
                if (leftBracketCount == rightBracketCount)
                {
                    partIndex = i;
                    break;
                }
            }
            if (partIndex == -1)
            {
                return -1;
            }
            return sql.Length - residueSql.Length - keywords[partIndex - 1].Groups[0].Value.TrimStart().Length;
        }

        /// <summary>
        /// 拼接关键字之后的SQL语句
        /// </summary>
        /// <param name="keywordSplit">根据关键字分割后的字符串集</param>
        /// <param name="partIndex">当前关键字在关键字集合中的索引</param>
        /// <param name="keywords">关键字集合</param>
        public static string CombineResidueString(string[] keywordSplit, int partIndex, MatchCollection keywords)
        {
            string sql = "";
            string @keyword;
            for (int i = keywordSplit.Length - 1; i >= partIndex; i--)
            {
                @keyword = i == keywords.Count ? "" : keywords[i].Groups[0].Value;
                sql = keywordSplit[i] + @keyword + sql;
            }
            return sql;
        }

        /// <summary>
        /// 根据给定的OrderBy和SQL语句获取最原始的OrderBy（非别名）
        /// </summary>
        /// <param name="orderBy">给定OrderBy（可能是别名）</param>
        /// <param name="sql">原始SQL语句</param>
        public static string GetOriginalOrderBy(string orderBy, string sql)
        {
            string result = "";
            List<string> splits = GetSplitSelect(sql);
            var orderBySplits = orderBy.Split(',');
            foreach (var orderBySplit in orderBySplits)
            {
                var temp = orderBySplit.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var orderField = temp.Length == 2 ? temp[0] : orderBySplit;
                var orderType = temp.Length == 2 ? temp[1] : "";
                orderField = GetOriginalField(orderField, sql, splits);
                result += orderField + " " + orderType + ",";
            }
            return result.Substring(0, result.Length - 1);
        }

        /// <summary>
        /// 获取字段的原始表示
        /// </summary>
        public static string GetOriginalField(string field, string sql, List<string> splits = null)
        {
            string result = field;
            if (splits == null)
            {
                splits = GetSplitSelect(sql);
            }
            if (!field.Contains("."))
            {
                var tempField = new List<string>();
                foreach (var split in splits)
                {
                    var sps = Regex.Match(split, "([\\s\\S]+)as([\\s\\S]+)", RegexOptions.IgnoreCase);
                    string original;
                    if (sps.Groups.Count == 3)
                    {
                        original = sps.Groups[2].Value;
                    }
                    else
                    {
                        var fieldSplit = split.Split('.');
                        original = fieldSplit.Length == 2 ? fieldSplit[1] : split;
                    }
                    var commentIdx = original.IndexOf("--", StringComparison.Ordinal);
                    commentIdx = commentIdx == -1 ? original.Length : commentIdx;
                    if (original.Substring(0, commentIdx).Trim().Equals(field.Trim()))
                    {
                        tempField.Add(sps.Groups.Count == 3 ? sps.Groups[1].Value.Trim() : split);
                        break;
                    }
                }
                if (tempField.Count == 1)
                {
                    result = tempField[0];
                }
                if (tempField.Count > 1)
                {
                    throw new BusinessException("SELECT子语句过于复杂，匹配失败！");
                }
            }
            return result;
        }

        /// <summary>
        /// 获取select单语句集合
        /// </summary>
        /// <param name="sql">原始SQL</param>
        public static List<string> GetSplitSelect(string sql)
        {
            List<string> splits = new List<string>();
            var fromIndex = LocationSqlKeyWord(sql, "from");
            var subStartToFrom = sql.Substring(0, fromIndex);
            var subSelects = subStartToFrom.Trim().Substring(6).Trim();
            var selectSplits = subSelects.Split(',');
            int leftCount = 0, rightCount = 0;
            string combine = "";
            for (int i = 0; i < selectSplits.Length; i++)
            {
                var leftBracketCount = selectSplits[i].Count(ch => ch.Equals('('));
                var rightBracketCount = selectSplits[i].Count(ch => ch.Equals(')'));
                if (leftCount != 0)
                {
                    leftCount += leftBracketCount;
                    rightCount += rightBracketCount;
                    combine += selectSplits[i] + ",";
                    if (leftCount == rightCount)
                    {
                        leftCount = rightCount = 0;
                        splits.Add(combine.Substring(0, combine.Length - 1));
                    }
                    continue;
                }
                if (leftBracketCount == rightBracketCount)
                {
                    splits.Add(selectSplits[i]);
                }
                else if (leftCount == 0)
                {
                    leftCount = leftBracketCount;
                    rightCount = rightBracketCount;
                    combine = selectSplits[i] + ",";
                }
            }
            return splits;
        }

        /// <summary>
        /// 将原始SQL转换为ROW NUMBER方式的分页SQL
        /// </summary>
        /// <param name="sql">原始SQL</param>
        /// <param name="primaryKey">主键</param>
        /// <param name="first">起始条数</param>
        /// <param name="last">截止条数</param>
        public static string RowNumberPagingSql(string sql, string primaryKey, int first, int last)
        {
            var fromIndex = LocationSqlKeyWord(sql, "from");
            var whereIndex = LocationSqlKeyWord(sql, "where");
            var orderIndex = LocationSqlKeyWord(sql, "order");
            if (orderIndex == -1)
            {
                throw new BusinessException("分页必须指定order by");
            }
            var subStartToWhere = sql.Substring(0, whereIndex == -1 ? orderIndex : whereIndex);
            var subFromToOrder = sql.Substring(fromIndex, orderIndex - fromIndex);
            //var subWhereToEnd = whereIndex == -1 ? "" : " AND " + sql.Substring(whereIndex + 5);
            var subOrderToEnd = sql.Substring(orderIndex);
            //var distinctFieldSplit = distinctField.Split('.');
            //var dustinctFieldShort = distinctFieldSplit.Length == 2 ? distinctFieldSplit[1] : distinctField;
            var orderby = subOrderToEnd.Substring(5).Trim().Substring(2);
            var orderbyStr = " ORDER BY " + SqlUtility.GetOriginalOrderBy(orderby, sql);
            var originalOrimaryKey = GetOriginalField(primaryKey, sql);

            sql =
                string.Format(
                    "{0} INNER JOIN (SELECT {1} AS HOURGLASS_IDENTITY, ROW_NUMBER() OVER({2}) AS TPOOO_ROW_ID {3}) AS TPOOO_ROW_TABLE ON {1}=TPOOO_ROW_TABLE.HOURGLASS_IDENTITY WHERE TPOOO_ROW_TABLE.TPOOO_ROW_ID BETWEEN {4} AND {5} {2} OPTION(RECOMPILE)",
                    subStartToWhere, originalOrimaryKey, orderbyStr, subFromToOrder, first, last);
            return sql;
        }

        /// <summary>
        /// 拷贝SqlParameter的一个备份
        /// </summary>
        public static SqlParameter[] SqlParameterCopy(SqlParameter[] parameters)
        {
            SqlParameter[] @params = new SqlParameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var item = parameters[i];
                @params[i] = new SqlParameter(item.ParameterName, item.Value) {SqlDbType = item.SqlDbType};
            }
            return @params;
        }
    }
}
