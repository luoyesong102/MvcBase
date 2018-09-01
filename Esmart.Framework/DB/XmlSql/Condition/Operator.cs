using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Condition
{
    public struct Operator
    {
        public readonly static string Greater = " > ";
        public readonly static string GreaterAndEqual = " >= ";
        public readonly static string Less = " < ";
        public readonly static string LessAndEqual = " <= ";
        public readonly static string Equal = " = ";
        public readonly static string NotEqual = " <> ";
        public readonly static string IsNull = " Is Null ";
        public readonly static string IsNotNull = " Is Not Null ";
        public readonly static string In = " In ";
        public readonly static string Like = " Like ";
        public readonly static string Between = " Between ";
        public readonly static string NotIn = " Not In ";

    }
}
