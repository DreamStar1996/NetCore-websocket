using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Util
{
    /// <summary>
    /// 配置帮助类
    /// </summary>
    public class ConfigHelper
    {
        public static IConfiguration Configuration;
        public static IServiceProvider ServiceProvider;
        public static HttpContext HttpCurrent
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(IHttpContextAccessor));
                HttpContext context = ((IHttpContextAccessor)factory).HttpContext;
                return context;
            }
        }
        public static T GetService<T>()
        {
            return (T)ServiceProvider.GetService(typeof(T));
        }
    }
}
