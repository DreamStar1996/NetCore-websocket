using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Common.Util
{
    /// <summary>
    /// 枚举拓展类
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        　　/// 根据System.ComponentModel.DataAnnotations下的DisplayAttribute特性获取显示文本
        　　/// </summary>
        　　/// <param name="t"></param>
        　　/// <returns></returns>
        public static string GetText(this Enum t)
        {
            var t_type = t.GetType();
            var fieldName = Enum.GetName(t_type, t);
            var objs = fieldName!=null? t_type?.GetField(fieldName)?.GetCustomAttributes(typeof(DisplayAttribute), false):new object[] { };
            return objs.Length > 0 ? ((DisplayAttribute)objs[0]).Name : null;
        }
        /// <summary>
        /// 通过display注解解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static T Parse<T>(this Enum t, string text)
        {
            var t_type = t.GetType();
            FieldInfo[] fields = t_type.GetFields();
            foreach (var field in fields)
            {
                var objs = field.GetCustomAttributes(typeof(DisplayAttribute), false);//获取字段
                if (objs.Length > 0)
                {
                    DisplayAttribute dis = (DisplayAttribute)objs[0];
                    if (dis.Name == text)
                    {
                        return (T)Enum.Parse(t_type, field.Name);
                        // return (T) field.GetValue(null);
                    }
                }
            }
            return default(T);
        }
    }
}
