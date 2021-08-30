using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class Userinfo
    {
        public int Id{ get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// 登录类型teacher、student、user
        /// </summary>
        public string Type { get; set; } = "user";
       
        public String Token { get; set; }
        public String Avatar { get; set; }
        public string Realname { get; set; }//真实姓名
        public String Role { get; set; }
        public String Tel { get; set; }//电话
        public String Email { get; set; }//电子邮箱
        public DateTime Birthday { get; set; }//出生日期
    }
}
