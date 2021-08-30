using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Model;
using Microsoft.Extensions.Hosting;
using Web.DB;
namespace Web.DB
{
    /// <summary>
    /// 初始化数据库、执行迁移等功能。
    /// 1、Add-Migration 版本名  //令创建迭代版本
    /// 2、Remove-Migration  删除迁移
    /// </summary>
    public static class WebHostExtension
    {
        public static IHost InitDB<TContext>(this IHost host/*, Action<TContext> sedder*/)
            where TContext : MyDbContext
        {
            //创建数据库实例在本区域有效
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetService<TContext>();
                try
                {
                    //这种方式初始创建数据库不行 ，必须先执行Add-Migration 命令然后运行
                    //if (!context.Database.CanConnect())
                    //{
                    //    context.Database.EnsureCreated();
                    //} 
                    if (context.Database.GetPendingMigrations().GetEnumerator().MoveNext())
                    {//获取待升级的版本
                        context.Database.Migrate();//自动执行迁移
                    }

                    bool ret = InitDatabase.Initialize(context);
                    if (ret)
                    {
                        logger.LogInformation($"执行DbContext{typeof(TContext).Name} 初始化数据成功");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"执行dbcontext {typeof(TContext).Name}  seed失败");
                }
            }
            return host;
        }
    }

}
