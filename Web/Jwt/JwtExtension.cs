using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Web.Jwt
{
    /// <summary>
    /// https://www.cnblogs.com/RainingNight/p/authorization-in-asp-net-core.html
    /// </summary>
    public static class JwtExtension
    {
        public static void ConfigureJwt(this IServiceCollection services, JwtConfig jwtConfig)
        {
            #region jwt配置 
            //认证
            services.AddAuthentication(x =>
            {
                // 认证middleware配置
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    //jwt token参数设置                 
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = ClaimTypes.Name,
                        //NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role,
                        //RoleClaimType = JwtClaimTypes.Role,
                        // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
                        ValidateIssuer = true, // //Token颁发机构 发行人验证，这里要和token类中Claim类型的发行人保持一致
                        ValidIssuer = jwtConfig.Issuer,//发行人 
                        ValidateAudience = false,  // 接收人验证
                        ValidAudience = jwtConfig.Audience,//订阅人  //颁发给谁
                        ValidateIssuerSigningKey = true,// 是否开启签名认证 //这里的key要进行加密
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                        ValidateLifetime =false//是否验证超时  当设置exp和nbf时有效 同时启用ClockSkew
                        /***********************************TokenValidationParameters的参数默认值***********************************/
                        // RequireSignedTokens = true,
                        // SaveSigninToken = false,
                        // ValidateActor = false,
                        // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
                        // ValidateAudience = true,
                        // ValidateIssuer = true, 
                        // ValidateIssuerSigningKey = false,
                        // 是否要求Token的Claims中必须包含Expires
                        // RequireExpirationTime = true,
                        // 允许的服务器时间偏移量  注意这是缓冲过期时间，总的有效时间等于这个时间加上jwt的过期时间，如果不配置，默认是5分钟
                        // ClockSkew = TimeSpan.FromSeconds(300),
                        // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                        // ValidateLifetime = true
                    };
                    //自定义Token获取方式
                    #region
                    //o.Events = new JwtBearerEvents()
                    //{
                    //    OnMessageReceived = context =>
                    //    {
                    //        context.Token = context.Request.Query["access_token"];
                    //        return Task.CompletedTask;
                    //    }
                    //};
                    #endregion
                });
            #endregion

            #region 授权
            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy => policy.RequireRole("admin").Build());
                options.AddPolicy("user", policy => policy.RequireRole("user").Build());
                options.AddPolicy("teacher", policy => policy.RequireRole("teacher","admin").Build());
                options.AddPolicy("student", policy => policy.RequireRole("student").Build());
            });
            #endregion
        }
    }
}
