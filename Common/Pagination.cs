using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    /// <summary>
    /// 分页数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pagination<T>
    {
        public static Pagination<T> Init(int PageCount, int Total, IEnumerable<T> Items)
        {
            return new Pagination<T>() { PageCount = PageCount, Total = Total, Items = Items };
        }
        public IEnumerable<T> Items { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// 总记录数量
        /// </summary>
        public int Total { get; set; }

    }
    /// <summary>
    ///分页参数
    /// </summary>
    public class PageParam
    {
        /// <summary>
        /// 页码
        /// </summary>
        public const string pageNo = "pageNo";
        /// <summary>
        /// 每页显示记录数
        /// </summary>

        public const string pageSize = "pageSize";
        /// <summary>
        /// 排序关键字
        /// </summary>

        public const string sort = "sort";
    }
}
