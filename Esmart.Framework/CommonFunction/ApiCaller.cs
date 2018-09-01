using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esmart.Framework.Model;

namespace Esmart.Framework.Utilities
{
    public class ApiCaller
    {
        public object _instance;
        public ApiCaller(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            _instance = instance;
        }

        public T Call<T>(string moudelName, string apiFlag, params object[] @params)
        {
            var method = ReflectionUtils.GetMethod<PortalMoudelAttribute, ApiFlagAttribute>
                 ((a, t) => a.Name == moudelName,
                  api =>api != null && !string.IsNullOrEmpty(api.Name) &&  api.Name.ToLower() == apiFlag.ToLower());

            if (method != null)
            {
                var obj = Activator.CreateInstance(method.ReflectedType);
                return (T)method.Invoke(obj, @params);
            }
            return default(T);
        }
    }
}

//AppDomain.CurrentDomain.GetAssemblies()获取所有程序集合（只要是用到的）
//Assembly.GetEntryAssembly()(获取已加载)
//Assembly.GetExecutingAssembly()(获取当前正在执行的)
//Assembly.LoadFrom(dllFileName)(获取指定)
//GetReferencedAssemblies 引用dll

// string assemblyFilePath= @"C:\Dog.dll";

//  Assembly ass= Assembly.LoadFile(assemblyFilePath);

//  Type t = ass.GetType("Dog",false,false);

//  MethodInfo  info = t.GetMethod("Sound");

// object instance = Activator.CreateInstance(t);

// info.Invoke(instance,new object[]{2});//狗叫了两声



//        Type objType = A.GetType();
//        Assembly objassembly = objType.Assembly;
//        Type[] types = objassembly.GetTypes();
//        foreach (Type type in types)
//        {
//            Console.WriteLine("类名 " + type.FullName);
//            // 获取类型的结构信息
//            ConstructorInfo[] myConstructor = type.GetConstructors();
//            Show(myConstructor);
//            // 获取类型的字段信息
//            FieldInfo[] myField = type.GetFields();
//            Show(myField);
//            // 获取方法的方法
//            MethodInfo[] myMethod = type.GetMethods();
//            Show(myMethod);
//            // 获取属性的方法
//            PropertyInfo[] myProperty = type.GetProperties();
//            Show(myProperty);
//            // 获取事件信息，这个Demo没有事件，所以就不写了 EventInfo




//2.1、假设你要反射一个 DLL 中的类，并且没有引用它（即未知的类型）： 
//Assembly assembly = Assembly.LoadFile("程序集路径，不能是相对路径"); // 加载程序集（EXE 或 DLL） 
//object obj = assembly.CreateInstance("类的完全限定名（即包括命名空间）"); // 创建类的实例 

//2.2、若要反射当前项目中的类可以为：
//Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
//object obj = assembly.CreateInstance("类的完全限定名（即包括命名空间）"); // 创建类的实例，返回为 object 类型，
//需要强制类型转换
//2.3、也可以为：
//Type type = Type.GetType("类的完全限定名"); 
