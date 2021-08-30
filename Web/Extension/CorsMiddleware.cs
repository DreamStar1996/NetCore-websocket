using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Extension
{
    /// <summary>
    /// 跨域中间件，允许所有域访问。生产环境慎用
    /// </summary>
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;

        public CorsMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public Task Invoke(HttpContext httpContext)
        {

            if (httpContext.Request.Method == "OPTIONS")
            {
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", httpContext.Request.Headers["Origin"]);
                httpContext.Response.Headers.Add("Access-Control-Allow-Headers", httpContext.Request.Headers["Access-Control-Request-Headers"]);
                httpContext.Response.Headers.Add("Access-Control-Allow-Methods", httpContext.Request.Headers["Access-Control-Request-Method"]);
                httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                httpContext.Response.Headers.Add("Access-Control-Max-Age", "86400");//缓存一天
                httpContext.Response.StatusCode = 200;
                return httpContext.Response.WriteAsync("OK");
            }
            if (httpContext.Request.Headers["Origin"] != "")
            {
                httpContext.Response.Headers.Add("Access-Control-Allow-Origin", httpContext.Request.Headers["Origin"]);
            }
            //Console.WriteLine("来源："+ httpContext.Request.Headers["Origin"]);
            httpContext.Response.Headers.Add("Access-Control-Allow-Headers", httpContext.Request.Headers["Access-Control-Request-Headers"]);
            httpContext.Response.Headers.Add("Access-Control-Allow-Methods", httpContext.Request.Headers["Access-Control-Request-Method"]);
            httpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
            httpContext.Response.Headers.Add("Access-Control-Max-Age", "86400");//缓存一天
            return _next.Invoke(httpContext);
        }
    }
    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CorsMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorsMiddleware>();
        }
    }
}
