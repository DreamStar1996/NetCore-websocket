using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Extension;
namespace Web.Jwt
{
    /// <summary>
    /// 授权中间件
    /// https://www.cnblogs.com/whuanle/p/12497614.html
    /// 
    /// </summary>
    public class JwtAuthorizationFilter
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// redis缓存
        /// </summary>
        public IDistributedCache cache { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public JwtAuthorizationFilter(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext httpContext)
        {
            //检测是否包含'Authorization'请求头，如果不包含则直接放行
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                return _next(httpContext);
            }
            var tokenHeader = httpContext.Request.Headers["Authorization"];
            tokenHeader = tokenHeader.ToString().Substring("Bearer ".Length).Trim();

            Token tm = JwtHelper.SerializeJWT(tokenHeader);

            //BaseBLL.TokenModel = tm;//将tokenModel存入baseBll
            //cache.GetObject();
            //授权
            var claimList = new List<Claim>();
            claimList.Add(new Claim(ClaimTypes.Role, tm.Role));
            claimList.Add(new Claim(ClaimTypes.Name, tm.Uname));
            claimList.Add(new Claim(JwtClaimTypes.Id, tm.Uid.ToString()));
            claimList.Add(new Claim("Project", tm.Project));
            claimList.Add(new Claim("Type", tm.Type));
            claimList.Add(new Claim("ClasssId", tm.ClasssId.ToString()));
            claimList.Add(new Claim("SchoolId", tm.SchoolId.ToString()));
            var identity = new ClaimsIdentity(claimList);
            var principal = new ClaimsPrincipal(identity);
            httpContext.User = principal;
            //httpContext.Response.StatusCode = 403; 
            return _next(httpContext);
        }
    }
}
