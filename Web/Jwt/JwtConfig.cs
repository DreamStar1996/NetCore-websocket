using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Jwt
{
    /// <summary>
    /// Jwt配置
    /// </summary>
    public class JwtConfig
    {
  
        public JwtConfig()
        {
           
        }
  
        /// <summary>
        /// token是谁颁发的
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// token可以给那些客户端使用
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 安全密钥  加密的key（SecretKey必须大于16个,是大于，不是大于等于）
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Web端过期时间
        /// </summary>
        public double WebExp { get; set; }

        /// <summary>
        /// 移动端过期时间
        /// </summary>
        public double AppExp { get; set; }

        /// <summary>
        /// 小程序过期时间
        /// </summary>
        public double MiniProgramExp { get; set; }

        /// <summary>
        /// 其他端过期时间
        /// </summary>
        public double OtherExp { get; set; }
    }
}
