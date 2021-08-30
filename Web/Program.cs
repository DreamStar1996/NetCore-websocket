using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Model;
using Web.Autofac;
using Web.DB;
using Web.CodeGen;
using NLog.Web;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //自动代码生成器
            //models存放的是需要生成dal、bll、controller的model实体类。注意，如果代码是自动生成的，且改过，再生成的话，会把修改的还原，慎重
            List<string> models = new List<string>();
            //models.Add("CourseClass");
            //CreateHostBuilder(args).Build().GenCode(models);   //如果仅仅是需要生成代码不运行项目，运行本语句
            CreateHostBuilder(args).Build().InitDB<MyDbContext>().Run();//运行项目，执行本语句
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            //设置自定义修改启动端口配置文件
            var configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).AddJsonFile("host.json").Build();
            return Host.CreateDefaultBuilder(args)
                .UseAutofacServiceProviderFactory()//将默认ServiceProviderFactory指定为AutofacServiceProviderFactory
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        //应用配置文件
                        webBuilder.UseConfiguration(configuration).UseStartup<Startup>();
                    });
        }
    }
}
