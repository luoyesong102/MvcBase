using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esmart.Permission.Application.Constants
{
    class SwithType
    {
        public static string GetEventtype(string code)
        {
            string codename="";
            switch (code)
            {
                case "001001":
                   codename= "襄城";
                    break;
                case "001002":
                    codename = "襄城";
                    break;
                case "001003":
                    codename = "襄城";
                    break;
                case "001004":
                    codename = "襄城";
                    break;
                case "001005":
                    codename = "襄城";
                    break;
                case "001006":
                    codename = "襄城";
                    break;
                case "001007":
                    codename = "襄城";
                    break;
                case "001008":
                    codename = "襄城";
                    break;
                case "001009":
                    codename = "襄城";
                    break;
                case "001010":
                    codename = "襄城";
                    break;
                case "001012":
                    codename = "襄城";
                    break;
                case "001013":
                    codename = "襄城";
                    break;
                default:
                    codename = "其他";
                    break;
            }
            return codename;
        }
        public static string GetOrdertype(string code)
        {
            string codename = "";
            switch (code)
            {
                case "100020":
                    codename = "城市管理";
                    break;
                case "100021":
                    codename = "经贸管理";
                    break;
                case "100022":
                    codename = "农业农村";
                    break;
                case "100023":
                    codename = "公共服务";
                    break;
                case "100024":
                    codename = "公安司法";
                    break;
                case "100025":
                    codename = "科教文体";
                    break;
                case "100026":
                    codename = "自然资源";
                    break;
                case "100027":
                    codename = "社会保障";
                    break;
              
                //100028
                default:
                    codename = "其他";
                    break;
            }
            return codename;
        }
    }
}
