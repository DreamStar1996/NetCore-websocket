using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Filter
{
    /// <summary>。
    /// 是否启用查询过滤器。自定义特性
    /// （1）用于判断是否需要给model模型实体类添加身份标志
    /// （2）用于是否给方法参数where字典添加查询条件
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,
    AllowMultiple = false)]
    public class QueryableAttribute : System.Attribute
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;
    }
}
