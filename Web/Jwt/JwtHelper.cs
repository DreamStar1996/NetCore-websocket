using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Web.Util;

namespace Web.Jwt
{
    /// <summary>
    /// Jwt帮助类
    /// https://www.cnblogs.com/RayWang/p/9255093.html
    /// https://blog.csdn.net/sd7o95o/article/details/78488556
    /// </summary>
    public class JwtHelper
    {

        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <param name="jwtConfig"></param>
        /// <returns></returns>
        public static string IssueJWT(Token tokenModel, JwtConfig jwtConfig)
        {
            var dateTime = DateTime.UtcNow;
            var claims = new Claim[]
            {
                //https://blog.csdn.net/ahilll/article/details/83821853
                new Claim(JwtClaimTypes.Audience,jwtConfig.Audience),
                new Claim(JwtClaimTypes.Issuer,jwtConfig.Issuer),
                new Claim(JwtClaimTypes.Id, tokenModel.Uid.ToString()),//用户Id
                //new Claim(JwtClaimTypes.Name, tokenModel.Uname.ToString()),
                new Claim(ClaimTypes.Name, tokenModel.Uname.ToString()), 
                //new Claim(JwtClaimTypes.Role, tokenModel.Role),//角色
                //这个Role是官方UseAuthentication要要验证的Role，我们就不用手动设置Role这个属性了
                new Claim(ClaimTypes.Role, tokenModel.Role),//角色 要用ClaimTypes.Role 否则[Authorize("admin")]不起作用
                new Claim("Project", tokenModel.Project),//角色
                new Claim("Type", tokenModel.Type),//身份类型，角色类型
                new Claim("ClasssId", Convert.ToString(tokenModel.ClasssId)),//班级编号
                new Claim("SchoolId", Convert.ToString(tokenModel.SchoolId)),//学校编号
                new Claim(JwtRegisteredClaimNames.Iat,dateTime.ToString(),ClaimValueTypes.Integer64)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //过期时间
            double exp = 0;
            switch (tokenModel.TokenType)
            {
                case TokenType.Web:
                    exp = jwtConfig.WebExp;
                    break;
                case TokenType.App:
                    exp = jwtConfig.AppExp;
                    break;
                case TokenType.MiniProgram:
                    exp = jwtConfig.MiniProgramExp;
                    break;
                case TokenType.Other:
                    exp = jwtConfig.OtherExp;
                    break;
                default:
                    exp = jwtConfig.AppExp;
                    break;
            }
            var jwt = new JwtSecurityToken(
                issuer: jwtConfig.Issuer,
                claims: claims, //声明集合
                expires: dateTime.AddHours(exp),
                signingCredentials: creds);
            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt = jwtHandler.WriteToken(jwt);

            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(claims),
            //    Expires = dateTime.AddHours(exp),
            //    SigningCredentials = creds
            //};
            //var jwtHandler = new JwtSecurityTokenHandler();
            //var token = jwtHandler.CreateToken(tokenDescriptor);
            //var encodedJwt = jwtHandler.WriteToken(token);

            return encodedJwt;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static Token SerializeJWT(string jwtStr)
        {
            NLogHelper.logger.Info($"序列化token:{jwtStr}");
            //Console.WriteLine($"序列化token:{jwtStr}");
            //var jwtHandler = new JwtSecurityTokenHandler();
            var jwtHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
            object id = new object();
            object name = new object(); ;
            object role = new object(); ;
            object project = new object();
            object Type = new object();
            object ClasssId = new object();
            object SchoolId = new object();
            try
            {
                jwtToken.Payload.TryGetValue(JwtClaimTypes.Id, out id);
                jwtToken.Payload.TryGetValue(ClaimTypes.Name, out name);
                jwtToken.Payload.TryGetValue(ClaimTypes.Role, out role);
                jwtToken.Payload.TryGetValue("Project", out project);
                jwtToken.Payload.TryGetValue("Type", out Type);
                jwtToken.Payload.TryGetValue("ClasssId", out ClasssId);
                jwtToken.Payload.TryGetValue("SchoolId", out SchoolId);

                //遗留问题
                if (name == null || role == null)
                {
                    jwtToken.Payload.TryGetValue(JwtClaimTypes.Name, out name);
                    jwtToken.Payload.TryGetValue(JwtClaimTypes.Role, out role);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            var tm = new Token
            {
                Uid = Convert.ToInt32(id),
                Uname = name.ToString(),
                Role = Convert.ToString(role),
                Project = Convert.ToString(project),
                Type = Convert.ToString(Type),
                ClasssId = Convert.ToInt32(ClasssId),
                SchoolId = Convert.ToInt32(SchoolId)
            };
            return tm;
        }
    }

}
