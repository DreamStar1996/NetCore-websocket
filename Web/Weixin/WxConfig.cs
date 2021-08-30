using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Weixin
{
    /// <summary>
    /// 微信配置
    /// </summary>
    public class WxConfig
    {
        public WxMiniConfig Mini { get; set; }
        public WxMpConfig Mp { get; set; }
        public WxMiniConfig App { get; set; }
    }
    /// <summary>
    /// 微信小程序配置
    /// </summary>
    public class WxMiniConfig
    {
        public string AppID { get; set; }
        public string AppSecret { get; set; }     
    }
    /// <summary>
    /// 微信公众号配置
    /// </summary>
    public class WxMpConfig: WxMiniConfig
    {       
        public string Token { get; set; }
    }
}
