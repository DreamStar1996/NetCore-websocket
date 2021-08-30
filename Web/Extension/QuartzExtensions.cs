using Autofac.Extensions.DependencyInjection;
using Bll;
using Bll.Impl;
using DAL;
using DAL.Impl;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Controllers.Job;
using Web.Security;
using Web.Util;

namespace Web.Extension
{
    /// <summary>
    /// 启动定时器服务，后台服务
    /// </summary>
    public static class QuartzExtensions
    {
        /// <summary>
        /// 使用定时器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection UseQuartz(this IServiceCollection services)
        {
            //添加Quartz服务
            //services.AddSingleton<IJobFactory, IOCJobFactory>();//已经注入
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<QuartzJobRunner>();
            //在托管服务Startup.ConfigureServices中注入我们的后台服务：
            services.AddHostedService<QuartzHostedService>();
            return services;
        }
    }

}
