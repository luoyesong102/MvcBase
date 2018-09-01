
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Esmart.Framework.ModelValidate
{
    public delegate bool Validator<TEntity>(TEntity entity, out string message) where TEntity : class;
    public delegate bool ValidMethod();

    /// <summary>
    /// 验证工具类
    /// </summary>
    public static partial class ModelValidate
    {
        #region 验证逻辑控制
        /// <summary>
        /// 继续验证 如果之前的结果为false 则不继续执行
        /// </summary>
        /// <param name="res"></param>
        /// <param name="MethodResult">执行方法后的结果</param>
        /// <returns></returns>
        public static bool GoOn(this bool res, ValidMethod action)
        {
            if (res)
            {
                var a = action();
                return a;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 值类型验证

        #region 大于
        /// <summary>
        /// 小于某个值
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="InputValue"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        public static bool GT(this int Value, int InputValue, string ErrorMessage, StringBuilder OutMessage)
        {
            var res = Value.GT(InputValue);
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        /// <summary>
        /// 大于某个值
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="InputValue"></param>
        /// <returns></returns>
        public static bool GT(this int Value, int InputValue)
        {

            return Value > InputValue ? true : false;
        }
        /// <summary>
        /// 大于某个值
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="InputValue"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        public static bool GT(this double Value, int InputValue, string ErrorMessage, StringBuilder OutMessage)
        {
            var res = Value.GT(InputValue);
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        /// <summary>
        /// 大于某个值
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="InputValue"></param>
        /// <returns></returns>
        public static bool GT(this double Value, int InputValue)
        {

            return Value > InputValue ? true : false;
        }
        #endregion

        #region 小于
        /// <summary>
        /// 小于某个值
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="InputValue"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        public static bool LT(this int Value, int InputValue, string ErrorMessage, StringBuilder OutMessage)
        {
            var res = Value.LT(InputValue);
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        /// <summary>
        /// 小于某个值
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="InputValue"></param>
        /// <returns></returns>
        public static bool LT(this int Value, int InputValue)
        {

            return Value < InputValue ? true : false;
        }
        /// <summary>
        /// 小于某个值
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="InputValue"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        public static bool LT(this double Value, int InputValue, string ErrorMessage, StringBuilder OutMessage)
        {
            var res = Value.LT(InputValue);
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        /// <summary>
        /// 小于某个值
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="InputValue"></param>
        /// <returns></returns>
        public static bool LT(this double Value, int InputValue)
        {

            return Value < InputValue ? true : false;
        }
        #endregion

        #region 小于等于
        /// <summary>
        /// 小于等于某个值
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="MinValue"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        public static bool LTEquals(this int Value, int InputValue, string ErrorMessage, StringBuilder OutMessage)
        {
            var res = Value.LTEquals(InputValue);
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        public static bool LTEquals(this int Value, int InputValue)
        {

            return Value > InputValue ? false : true;
        }

        /// <summary>
        /// 小于等于
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="MinValue"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        public static bool LTEquals(this double Value, double InputValue, string ErrorMessage, StringBuilder OutMessage)
        {

            var res = Value.LTEquals(InputValue);
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        /// <summary>
        /// 小于等于
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="InputValue">最小值</param>
        /// <returns></returns>
        public static bool LTEquals(this double Value, double InputValue)
        {

            return Value > InputValue ? false : true;
        }


        #endregion

        #region  大于等于
        /// <summary>
        /// 大于等于某个值
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="MinValue"></param>
        /// <param name="Message"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        public static bool GTEquals(this int Value, int InputValue, string Message, StringBuilder OutMessage)
        {

            var res = Value.GTEquals(InputValue);
            if (res == false)
            {
                OutMessage.Append(Message);
            }
            return res;
        }
        public static bool GTEquals(this int Value, int InputValue)
        {

            return Value < InputValue ? false : true;
        }

        /// <summary>
        /// 大于等于某个值
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="MinValue"></param>
        /// <param name="Message"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        public static bool GTEquals(this double Value, double InputValue, string Message, StringBuilder OutMessage)
        {

            var res = Value.GTEquals(InputValue);
            if (res == false)
            {
                OutMessage.Append(Message);
            }
            return res;
        }
        /// <summary>
        /// 大于等于某个值
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="MinValue"></param>
        /// <param name="Message"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        internal static bool GTEquals(this double Value, double InputValue)
        {

            return Value < InputValue ? false : true;
        }
        #endregion

        #region 非空验证

        #region 非0
        /// <summary>
        /// 不为0
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        internal static bool HasValue(this int Value, string ErrorMessage, StringBuilder OutMessage)
        {

            var res = Value.HasValue();
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        internal static bool HasValue(this int Value)
        {
            return Value == 0 ? false : true;
        }
        #endregion

        #region 非空
        public static bool HasValue(this double Value, string ErrorMessage, StringBuilder OutMessage)
        {
            var res = Value.HasValue();
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        internal static bool HasValue(this double Value)
        {
            return Value == 0 ? false : true;
        }

        #endregion

        #endregion

        #endregion




        #region 字符串验证

        #region 非空
        public static bool HasValue(this DateTime? Value, string ErrorMessage, StringBuilder OutMessage)
        {

            var res = Value.HasValue();
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;

        }
        public static bool HasValue(this DateTime? Value)
        {
            var res = Value.HasValue;
            return res;

        }


        public static bool HasValue(this string Value, string ErrorMessage, StringBuilder OutMessage)
        {

            var res = Value.HasValue();
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;

        }

        private static void AppendWithEnter(string ErrorMessage, StringBuilder OutMessage)
        {
            OutMessage.Append(ErrorMessage).Append("<br/>");
        }
        public static bool HasValue(this string Value)
        {
            if (string.IsNullOrEmpty(Value))
            {
                return false;
            }
            else
            {
                if (string.IsNullOrEmpty(Value.Trim()))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 字符串最大长度
        /// <summary>
        /// 字符串最大长度
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="MaxLength"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        public static bool MaxLength(this string Value, int Length, string ErrorMessage, StringBuilder OutMessage)
        {

            var res = Value.MaxLength(Length);
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        /// <summary>
        /// 字符串最大长度
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static bool MaxLength(this string Value, int Length)
        {

            return Value.Length > Length ? false : true;
        }


        #endregion

        #region 字符串最小长度
        /// <summary>
        /// 字符串最小长度
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="MaxLength"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        public static bool MinLength(this string Value, int Length, string ErrorMessage, StringBuilder OutMessage)
        {

            var res = Value.MaxLength(Length);
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        /// <summary>
        /// 字符串最小长度
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="MaxLength"></param>
        /// <returns></returns>
        public static bool MinLength(this string Value, int Length)
        {

            return Value.Length < Length ? false : true;
        }
        #endregion

        #endregion

        #region 正则验证

        #region 正整数验证
        /// <summary>
        /// 正整数
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public static bool IsNum(this string Value, string ErrorMessage, StringBuilder OutMessage)
        {

            var res = Value.IsNum();
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        /// <summary>
        /// 正整数
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool IsNum(this string Value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(Value, @"^[0-9]*[1-9][0-9]*$");
        }

        #endregion

        #region 金额验证
        public static bool RegMoney(this string Value, string ErrorMessage, StringBuilder OutMessage)
        {
            var res = Value.RegMoney();
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        public static bool RegMoney(this string Value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(Value, @"^([1-9][\d]{0,7}|0)(\.[\d]{1,2})?$");
        }
        #endregion

        #region 手机验证

        public static bool RegPhone(this string Value, string ErrorMessage, StringBuilder OutMessage)
        {
            var res = Value.RegPhone();
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        /**
        *	手机号:目前全国有27种手机号段。&nbsp;
        *	移动有16个号段：134、135、136、137、138、139、147、150、151、152、157、158、159、182、187、188。其中147、157、188是3G号段，其他都是2G号段。
        *	联通有7种号段：130、131、132、155、156、185、186。其中186是3G（WCDMA）号段，其余为2G号段。
        *	电信有4个号段：133、153、180、189。其中189是3G号段（CDMA2000），133号段主要用作无线网卡号。
        *	150、151、152、153、155、156、157、158、159&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;九个&nbsp;
        *	130、131、132、133、134、135、136、137、138、139&nbsp;&nbsp;&nbsp;&nbsp;十个&nbsp;
        *	180、182、185、186、187、188、189&nbsp;&nbsp;&nbsp;七个&nbsp;
        *	13、15、18三个号段共30个号段，154、181、183、184暂时没有，加上147共27个。&nbsp;&nbsp;
        */
        public static bool RegPhone(this string Value)
        {
            if (Value.StartsWith("1"))
            {
                var res = Value.Length == 11 ? true : false;
                return res;
            }
            else
            {
                return true;
            }
           // return System.Text.RegularExpressions.Regex.IsMatch(Value, @"(^13\d{9}$)|(^15[0,1,2,3,5,6,7,8,9]\d{8}$)|(^18[0,2,5,6,7,8,9]\d{8}$)|(^147\d{8}$)");
        }

        #endregion

        #region 邮箱验证
        /// <summary>
        /// 邮箱验证
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        public static bool RegEmail(this string Value, string ErrorMessage, StringBuilder OutMessage)
        {

            var res = Value.RegEmail();
            if (res == false)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        /// <summary>
        /// 邮箱验证
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Message"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        public static bool RegEmail(this string Value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(Value, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
        #endregion

        #region 特殊符号验证

        /// <summary>
        /// 特殊符号验证
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        public static bool RegSymbol(this string Value, string ErrorMessage, StringBuilder OutMessage)
        {

            var res = Value.RegSymbol();
            if (res == true)
            {
                AppendWithEnter(ErrorMessage, OutMessage);
            }
            return res;
        }
        /// <summary>
        /// 特殊符号验证
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Message"></param>
        /// <param name="OutMessage"></param>
        /// <returns></returns>
        public static bool RegSymbol(this string Value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(Value, @"[`~%!@#^=''?~！@#￥……&——‘”“'？*()（），,。.、]");
        }
        #endregion
        #endregion
    }

    public static class EntityModelValidate
    {
        public static BaseViewModel Validate<TEntity>(Validator<TEntity> modelValidate, TEntity model)
            where TEntity : class
        {
            string message;
            var success = modelValidate(model, out message);
            return new BaseViewModel
            {
                Success = success,
                Message = message
            };
        }

        public static BaseViewModel Validate<TEntity>(Validator<TEntity> modelValidate, TEntity[] model)
            where TEntity : class
        {
            string message = "", temp;
            for (int i = 0; i < model.Length; i++)
            {
                if (!modelValidate(model[i], out temp))
                {
                    message += string.Format("第{0}条数据： {1}", i + 1, temp);
                }
            }
            return new BaseViewModel
            {
                Success = message.Length == 0,
                Message = message
            };
        }


    }

}
