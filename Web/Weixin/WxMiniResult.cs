using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Weixin
{
    /// <summary>
    /// 微信小程序结果
    /// </summary>
    public class Code2SessionResult : WxError
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string session_key { get; set; }
        /// <summary>
        /// 用户在开放平台的唯一标识符，在满足 UnionID 下发条件的情况下会返回，详见 UnionID 机制说明。
        /// </summary>
        public string unionid { get; set; }
        //errcode 的合法值  40029	code 无效   45011	频率限制，每个用户每分钟100次
    }
}
