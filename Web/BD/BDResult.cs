/* author:QinYongcheng */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BD
{
    /// <summary>
    /// 百度返回结果
    /// </summary>
    public class BDResult
    {
        public int err_no { get; set; }
        public string err_msg { get; set; }
        public string corpus_no { get; set; }
        public string sn { get; set; }
        public string result { get; set; }
        public bool IsOk()
        {
            return this.err_no == 0;
        }
    }
}
