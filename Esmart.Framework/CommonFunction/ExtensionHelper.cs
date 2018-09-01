using Eureka.IAI.Solution.Core.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Esmart.Framework.Utilities
{
    public static class ExtensionHelper
    {
        public static string Format(this DateTime dateTime)
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff");
        }

        public static string FormatFullDateTime(this DateTime dateTime)
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public static string DateTimeToShortDate(this DateTime? dateTime)
        {
            if (dateTime.HasValue)
                return dateTime.Value.ToString("yyyyMMdd");
            return "";
        }

        public static string DateTimeToLongDate(this DateTime? dateTime)
        {
            if (dateTime.HasValue)
                return dateTime.Value.ToString("yyyyMMddHHmmss");
            return "";
        }

        public static DateTime? StringToShortDateHasValue(this string dateTimeString)
        {
            try
            {
                return DateTime.ParseExact(dateTimeString, "yyyyMMdd", new CultureInfo("en-US"));
            }
            catch (Exception exception)
            {
                exception.HandleException();
                return null;
            }
        }

        public static DateTime? StringToLongDateHasValue(this string dateTimeString)
        {
            try
            {
                return DateTime.ParseExact(dateTimeString, "yyyyMMddHHmmss", new CultureInfo("en-US"));
            }
            catch (Exception exception)
            {
                exception.HandleException();
                return null;
            }
        }

        public static bool ValidFullDateTime(this string dateTime)
        {
            try
            {   if(string.IsNullOrEmpty(dateTime))
                {
                    return true;//空值不需要验证
                }
                DateTime dt;
                if(DateTime.TryParseExact(dateTime,
                    "yyyyMMddHHmmss",
                    new System.Globalization.CultureInfo("en-US"),
                    DateTimeStyles.None,
                    out dt))
                {
                    return true;
                }
                else
                {
                  return  DateTime.TryParseExact(dateTime,
                    "yyyyMMdd",
                    new System.Globalization.CultureInfo("en-US"),
                    DateTimeStyles.None,
                    out dt);
                }
                
            }
            catch (Exception exception)
            {
                exception.HandleException();
                return false;
            }
        }

        public static bool ValidDecimals(this string decimalstr)
        {
            try
            {
                if (string.IsNullOrEmpty(decimalstr))
                {
                    return true;//空值不需要验证
                }
               
                else
                {
                    decimal.Parse(decimalstr);
                    return true;
                }
            }
            catch (Exception exception)
            {
                exception.HandleException();
                return false;
            }
        }

        public static string FormatFullDate(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }

        public static bool ValidFullDateTime(this string dateTime, out DateTime dt)
        {
            dt = DateTime.MinValue;
            try
            {
                dt = Convert.ToDateTime(dateTime);
                return true;
            }
            catch (Exception exception)
            {
                exception.HandleException();
                return false;
            }
        }

        public static DateTime? TransStrToDateTime(this string dateTime)
        {
            try
            {
                DateTime  dt;
                if (string.IsNullOrEmpty(dateTime))
                {
                    Nullable<DateTime> times = null;
                    return times;
                }
                if (DateTime.TryParseExact(dateTime,
                    "yyyyMMddHHmmss",
                    new System.Globalization.CultureInfo("en-US"),
                    DateTimeStyles.None,
                    out dt))
                {
                    return dt;
                }
                else
                {
                     DateTime.TryParseExact(dateTime,
                      "yyyyMMdd",
                      new System.Globalization.CultureInfo("en-US"),
                      DateTimeStyles.None,
                      out dt);
                    return dt;
                }
            }
            catch (Exception exception)
            {
                exception.HandleException();
                throw new Exception(dateTime + "格式不正确");

            }
        }
        public static DateTime TransStrToDateTimeByDate(this string dateTime)
        {
            try
            {

                return DateTime.ParseExact(dateTime,
                       "yyyyMMdd",
                       new System.Globalization.CultureInfo("en-US")
                       );
            }
            catch (Exception exception)
            {
                exception.HandleException();
                throw new Exception(dateTime + "格式不正确");

            }
        }
        public static bool ValidFullDate(this string dateTime)
        {
            try
            {
                DateTime dt;
                return DateTime.TryParseExact(dateTime,
                    "yyyyMMdd",
                    new System.Globalization.CultureInfo("en-US"),
                    DateTimeStyles.None,
                    out dt);
            }
            catch (Exception exception)
            {
                exception.HandleException();
                return false;
            }
        }
        public static bool ValidFullDate(this string dateTime, out DateTime dt)
        {
            dt = DateTime.MinValue;
            try
            {
                dt = Convert.ToDateTime(dateTime);
                return true;
            }
            catch (Exception exception)
            {
                exception.HandleException();
                return false;
            }
        }

        public static Boolean? TransStrToBool(this string boolstr)
        {
            bool btest;
            if (string.IsNullOrEmpty(boolstr))
            {
                Nullable<bool> times = null;
                return times;
            }
            Boolean.TryParse(boolstr, out btest);
            return btest;
        }
        public static byte? TransStrToInt(this string boolstr)
        {
            byte btest;
            if (string.IsNullOrEmpty(boolstr))
            {
                Nullable<byte> times = null;
                return times;
            }
            byte.TryParse(boolstr, out btest);
            return btest;
        }
        public static decimal? TransStrToDecimal(this string boolstr)
        {
            try
            {
            decimal btest;
            if (string.IsNullOrEmpty(boolstr))
            {
                Nullable<decimal> times = null;
                return times;
            }
            decimal.TryParse(boolstr, out btest);
            return btest;
            }
            catch (Exception exception)
            {
                exception.HandleException();
                throw new Exception(boolstr + "格式不正确");

            }
        }
        public static XElement GetDescendantsFirst(this XElement inputElement, string elementName, string nameSpace = "urn:hl7-org:v3")
        {
            XNamespace defaultNamespace = XNamespace.Get(nameSpace);
            return inputElement?.Descendants(defaultNamespace + elementName).FirstOrDefault();
        }

        public static List<XElement> GetDescendants(this XElement inputElement, string elementName, string nameSpace = "urn:hl7-org:v3")
        {
            XNamespace defaultNamespace = XNamespace.Get(nameSpace);
            return inputElement?.Descendants(defaultNamespace + elementName).ToList();
        }

        public static XElement GetElement(this XElement inputElement, string elementName, string nameSpace = "urn:hl7-org:v3")
        {
            XNamespace defaultNamespace = XNamespace.Get(nameSpace);
            return inputElement?.Element(defaultNamespace + elementName);
        }

        public static string GetElementAttribute(this XElement inputElement, string attributeName)
        {
            if (inputElement == null)
                return "";
            return inputElement.Attribute(attributeName).Value;
        }

        public static string GetElementValue(this XElement inputElement)
        {
            if (inputElement == null)
                return "";
            return inputElement.Value;
        }


        public static string GetBodyXml(this string xml, string elementName, string nameSpace = "urn:hl7-org:v3")
        {
            var xmlBody = XElement.Parse(xml);
            XNamespace defaultNamespace = XNamespace.Get(nameSpace);
            var xElement = xmlBody.GetDescendantsFirst(elementName);

            var bodyXml = "";
            if (xElement != null)
            {
                bodyXml = xElement.ToString();
            }
            return bodyXml;
        }
        public static XElement GetBodyXElement(this string xml, string elementName, string nameSpace = "urn:hl7-org:v3")
        {
            var xmlBody = XElement.Parse(xml);

            XNamespace defaultNamespace = XNamespace.Get(nameSpace);

            var xElement = xmlBody.Descendants(defaultNamespace + elementName).FirstOrDefault();
            return xElement;
        }
        public static IEnumerable<XElement> GetBodyXElementList(this string xml, string elementName, string nameSpace = "urn:hl7-org:v3")
        {
            var xmlBody = XElement.Parse(xml);

            XNamespace defaultNamespace = XNamespace.Get(nameSpace);
            return xmlBody.Descendants(defaultNamespace + elementName);
        }
        public static XElement GetElementObj(this XElement inputElement, string elementName)
        {
            XNamespace defaultNamespace = XNamespace.Get("urn:hl7-org:v3");
            return inputElement.Element(defaultNamespace + elementName);
           
        }
        public static IEnumerable<XElement> GetElementObjlist(this XElement inputElement, string elementName)
        {
            XNamespace defaultNamespace = XNamespace.Get("urn:hl7-org:v3");
            
            return inputElement.Elements(defaultNamespace + elementName);
        }
      
        public static string GetElementAttributeObj(this XElement inputElement, string attributeName)
        {
            if (inputElement == null)
                return "";
            return inputElement.Attribute(attributeName).Value;
        }



    }
}
