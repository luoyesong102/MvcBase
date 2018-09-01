using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Esmart.Permission.Web.MVC
{
    public class CustomizeDateTimePicker
    {
        public const string IconSpanCss = "input-group-addon";
        public const string RemoveIcon = "glyphicon glyphicon-remove";
        public const string DataTimeTextBoxCss = "form-control background-white pointer ";

        public static IDictionary<DateTimeCategory, string> AddIconsDictionary = new Dictionary<DateTimeCategory, string>()
        {
            { DateTimeCategory.Date, "glyphicon glyphicon-calendar"},
            {DateTimeCategory.DateTime, "glyphicon glyphicon-th"},
            {DateTimeCategory.Time, "glyphicon glyphicon-time"}
        };

        public const string DateTimePickerLinkFormat = "yyyy-mm-dd hh:ii";
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm";

        public static IDictionary<DateTimeCategory, string> DateTimePickerFormatsDictionary = new Dictionary<DateTimeCategory, string>()
        {
            { DateTimeCategory.Date, "yyyy-mm-dd"},
            {DateTimeCategory.DateTime, "yyyy-mm-dd hh:ii"},
            {DateTimeCategory.Time, "hh:ii"}
        };

        public static IDictionary<DateTimeCategory, string> DateTimeFormatsDictionary = new Dictionary<DateTimeCategory, string>( )
        {
            { DateTimeCategory.Date, "yyyy-MM-dd"},
            {DateTimeCategory.DateTime, "yyyy-MM-dd HH:mm"},
            {DateTimeCategory.Time, "HH:mm"}
        };


        public const string DateTimePickerCssFormat = "pointer input-group date col-md-5 {0}";
        public static IDictionary<DateTimeCategory, string> CssClassDictionary = new Dictionary<DateTimeCategory, string>( )
        {
            { DateTimeCategory.Date, "form_date"},
            {DateTimeCategory.DateTime, "form_datetime"},
            {DateTimeCategory.Time, "form_time"}
        };

        public const string PickerViewSettingTemplate = "startView: {0}, minView: {1}, maxView: {2}";

        public static IDictionary<DateTimeCategory, string> PickerViewSettingsDictionary = new Dictionary
            <DateTimeCategory, string>()
        {
            {
                DateTimeCategory.Date,
                string.Format(CultureInfo.CurrentCulture, PickerViewSettingTemplate, (int) PickerViewCategory.Day,
                    (int) PickerViewCategory.Day, (int) PickerViewCategory.Year)
            },
            {
                DateTimeCategory.DateTime,
                string.Format(CultureInfo.CurrentCulture, PickerViewSettingTemplate, (int) PickerViewCategory.Day,
                    (int) PickerViewCategory.Minute, (int) PickerViewCategory.Year)
            },
            {
                DateTimeCategory.Time,
                string.Format(CultureInfo.CurrentCulture, PickerViewSettingTemplate, (int) PickerViewCategory.Hour,
                    (int) PickerViewCategory.Minute, (int) PickerViewCategory.Hour)
            }
        };

        public DateTime? MinDateTime { get; set; }

        public DateTime? MaxDateTime { get; set; }

        public int MinuteStep { get; set; }

        public bool SelectOnly { get; set; }

        public CustomizeDateTimePicker() : this(null, null, true)
        {
            //do nothing
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="minDate">最小时间</param>
        /// <param name="maxDate">最大时间</param>
        /// <param name="selectOnly">是否只选</param>
        public CustomizeDateTimePicker( DateTime? minDate, DateTime? maxDate, bool selectOnly )
            : this( minDate, maxDate, 5, selectOnly )
        {
            //do nothing
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="minDate">最小时间</param>
        /// <param name="maxDate">最大时间</param>
        /// <param name="minuteStep">分钟间隔，日期控件需要选择分钟时使用，默认值为5</param>
        /// <param name="selectOnly">是否只选</param>
        public CustomizeDateTimePicker(DateTime? minDate, DateTime? maxDate, int minuteStep, bool selectOnly)
        {
            MinDateTime = minDate;
            MaxDateTime = maxDate;
            MinuteStep = minuteStep;
            SelectOnly = selectOnly;
        }

        public IDictionary<string, object> GetDateTextBoxAttributes( IDictionary<string, object> htmlAttributes )
        {
            var textHtmlAttributes = new Dictionary<string, object>();
            if (SelectOnly)
            {
                textHtmlAttributes.Add("readonly", "readonly");
            }
            var cssClass = DataTimeTextBoxCss;
            if (htmlAttributes != null && htmlAttributes.ContainsKey("class"))
            {
                cssClass = DataTimeTextBoxCss + htmlAttributes["class"];
            }

            textHtmlAttributes.Add("class", cssClass);

            return textHtmlAttributes;
        }

        public string GetDateTimePickerSetting( DateTimeCategory category, string datePickerIndentity )
        {
            var viewSetting = PickerViewSettingsDictionary[category];
            var commonSetting = string.Format(CultureInfo.CurrentCulture,
                "language: 'zh-CN', forceParse: 1, autoclose: 1, minuteStep: {0}, todayHighlight: {1}, todayBtn: {1}, format: '{2}' ", MinuteStep,
                category == DateTimeCategory.Time ? "0" : "1", DateTimePickerFormatsDictionary [category]);
            var dateLimitSetting = DateLimitSetting();

            return string.Format(CultureInfo.CurrentCulture, "{0},{1}{2}", viewSetting, commonSetting, dateLimitSetting);
        }

        private string DateLimitSetting( )
        {
            var builder = new StringBuilder( );
            if( MinDateTime != null )
            {
                builder.AppendFormat( ", startDate: '{0}'", MinDateTime.Value.ToString( DateTimeFormat, CultureInfo.InvariantCulture ) );
            }
            if( MaxDateTime != null )
            {
                builder.AppendFormat( ", endDate: '{0}'", MaxDateTime.Value.ToString( DateTimeFormat, CultureInfo.InvariantCulture ) );
            }
            return builder.ToString( );
        }


        public static string GetDateTimePickerCssClass( DateTimeCategory category )
        {
            return String.Format(CultureInfo.CurrentCulture, DateTimePickerCssFormat, CssClassDictionary[category] );
        }

        public static object ConvertValueFormat(object value, DateTimeCategory category)
        {
            if (value != null)
            {
                var strVal = value as string;
                if (!string.IsNullOrWhiteSpace(strVal))
                {
                    value = DateTime.Parse(strVal);
                }

                var dateVal = value as DateTime?;
                if (dateVal != null)
                {
                    value = dateVal.Value.ToString(DateTimeFormatsDictionary[category]);
                }
            }
            return value;
        }

        public enum PickerViewCategory
        {
            Minute = 0,
            Hour = 1,
            Day = 2,
            Month = 3,
            Year = 4
        }
    }
}
