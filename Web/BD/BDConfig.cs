/* author:QinYongcheng */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BD
{

    /// <summary>
    /// 百度配置
    /// 申请 https://console.bce.baidu.com/
    /// https://ai.baidu.com/ai-doc/SPEECH/sk4o0bnzp
    /// </summary>
    public class BDConfig
    {
        public string AppID { get; set; }
        public string APIKey { get; set; }
        public string SecretKey { get; set; }
    }
}
