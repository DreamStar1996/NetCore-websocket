using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
namespace Common.Util
{
  public  class Security
    {
        //32位md5加密
        public static string Md5(string inputValue)
        {
            //32位大写
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(inputValue));
                var strResult = BitConverter.ToString(result);
                string result3 = strResult.Replace("-", "");
                return result3;
            }
        }
    }
}
