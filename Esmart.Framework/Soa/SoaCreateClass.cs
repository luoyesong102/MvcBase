using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Soa
{
    internal static class SoaCreateClass
    {


        static List<string> GetDll(Type type)
        {
            List<string> dll = new List<string>() { "System.dll", "System.Core.dll", "mscorlib.dll", typeof(Newtonsoft.Json.Formatting).Assembly.Location, type.Assembly.Location, typeof(Esmart.Framework.Soa.SoaManager).Assembly.Location };
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                var pars = method.GetParameters();
                foreach (var par in pars)
                {
                    var dllLocation = par.ParameterType.Assembly.Location;
                    if (!dll.Contains(dllLocation))
                    {
                        dll.Add(dllLocation);
                    }
                }

                var returntype = method.ReturnType;

                if (method.ReturnType.IsGenericParameter)
                {
                    var types = method.ReturnType.GetGenericArguments();

                    foreach (var gtype in types)
                    {
                        if (!dll.Contains(gtype.Assembly.Location))
                        {
                            dll.Add(gtype.Assembly.Location);
                        }

                    }
                }
                var location = returntype.Assembly.Location;
                if (!dll.Contains(location))
                {
                    dll.Add(location);
                }
            }
            return dll;
        }


        static List<string> CreateMethods(Type type)
        {
            List<string> methodList = new List<string>();
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                List<string> parsList = new List<string>();
                StringBuilder strB = new StringBuilder(100);

                string returnNmae = method.ReturnType.ToString();
                returnNmae = returnNmae.Replace("`1", "").Replace("`2", "").Replace("[", "<").Replace("]", ">");

                strB.Append(string.Format("\r\n public  {0} {1}(", returnNmae, method.Name));
                var pars = method.GetParameters();

                foreach (var par in pars)
                {
                    if (parsList.Count != 0)
                    {
                        strB.Append(",");
                    }
                    strB.Append(string.Format("{0} {1}", par.ParameterType.ToString().Replace("`1", "").Replace("`2", "").Replace("[", "<").Replace("]", ">"), par.Name));
                    parsList.Add(par.Name);

                }
                var returnArgs = method.ReturnType.GetGenericArguments();
                if (returnArgs.Length == 0)
                {
                    throw new Exception("接口返回类型必须为Esmart.Framework.Model.Response<类型>");
                }
                strB.Append("){ \r\n").Append(string.Format("return Esmart.Framework.Soa.SoaCreate<{0}>.Invoke<{1}>", returnArgs[0].ToString().Replace("`1", "").Replace("`2", "").Replace("[", "<").Replace("]", ">"), type.FullName));

                strB.Append(string.Format("(a=>a.{0}({1}));", method.Name, string.Join(",", parsList))).AppendLine("}");
                methodList.Add(strB.ToString());
            }
            return methodList;
        }

        static string CreateClass(Type type, ref string className)
        {
            StringBuilder strClass = new StringBuilder();

            className = "chalie_" + Guid.NewGuid().ToString("N");

            strClass.AppendLine(string.Format(" public class  {0}:{1}", className, type.FullName)).Append("{");

            strClass.AppendLine(string.Join("\r\n", CreateMethods(type)));

            strClass.AppendLine("}");

            return strClass.ToString();
        }


        public static T  CreateObj<T>()
        {
            Type type = typeof(T);

            var obj = Caching.CacheManager.CreateNetCache().Get<T>(type.FullName);
            if (obj != null)
            {
                return obj;
            }


            if (!type.IsInterface)
            {
                throw new Esmart.Framework.Model.TpoBaseException("Soa 创建参数必须为接口");
            }

            CSharpCodeProvider cprovider = new CSharpCodeProvider();

            CompilerParameters cp = new CompilerParameters();

            cp.ReferencedAssemblies.AddRange(GetDll(type).ToArray());

            cp.GenerateInMemory = true; //是否只在内存中生成

            cp.WarningLevel = 4;
            cp.TreatWarningsAsErrors = false;


            string className = string.Empty;

            string[] source = { CreateClass(type, ref className) };

            CompilerResults result = cprovider.CompileAssemblyFromSource(cp, source);

            if (result.Errors.Count == 0)
            {
                obj = (T)result.CompiledAssembly.CreateInstance(className);

                Caching.CacheManager.CreateNetCache().Add<T>(type.FullName, obj);

                return obj;
            }
            else
            {
                throw new Exception("调用soa 的时候，自动生成失败，请开发重新生成");
            }
          
        }
    }
}
