
namespace Web.Jwt
{
    /// <summary>
    /// 令牌
    /// </summary>
    public class Token
    {
        /// <summary>
        /// 登录用户Id
        /// </summary>
        public int  Uid { get; set; }
        /// <summary>
        /// 登录用户名
        /// </summary>
        public string Uname { get; set; }
        /// <summary>
        /// 身份角色
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Project { get; set; }
        /// <summary>
        /// 令牌类型（终端类型）
        /// </summary>
        public TokenType TokenType { get; set; }
        /// <summary>
        /// 班级id
        /// </summary>
        public int? ClasssId { get; set; } = 0;
        /// <summary>
        /// 学校id
        /// </summary>
        public int? SchoolId { get; set; } = 0;
        /// <summary>
        /// 身份类型
        /// </summary>
        public string Type{ get; set; } 
    }
}
