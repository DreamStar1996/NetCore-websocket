/* author:QinYongcheng */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Autofac
{
    /// <summary>
    /// 指明那些类不需要被Autofac注入的注解
    /// </summary>
    public class NotAutofacAttribute : Attribute
    {
        public static bool IsNotAutofac(Type type)
        {
            var objs = type.GetCustomAttributes(typeof(NotAutofacAttribute), false);//获取字段          
            return objs.Length > 0;
        }
    }
}
