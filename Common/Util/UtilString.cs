using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Common.Util
{
    public class UtilString
    {
        /// <summary>
        /// 生成随机数，包含数字和字母
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomStr(int length)
        {
            byte[] random = new Byte[length / 2];
            // 使用加密服务提供程序 (CSP) 提供的实现来实现加密随机数生成器 (RNG)。无法继承此类
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(random);
            StringBuilder sb = new StringBuilder(length);
            int i;
            for (i = 0; i < random.Length; i++)
            {
                // 以16进制格式输出
                sb.Append(String.Format("{0:X2}", random[i]));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 生成制定位数的随机码（数字）
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomNum(int length)
        {
            var result = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var r = new Random(Guid.NewGuid().GetHashCode());
                result.Append(r.Next(0, 10));
            }
            return result.ToString();
        }
    }
}
