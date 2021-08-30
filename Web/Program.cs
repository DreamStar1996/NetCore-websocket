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
            //�Զ�����������
            //models��ŵ�����Ҫ����dal��bll��controller��modelʵ���ࡣע�⣬����������Զ����ɵģ��ҸĹ��������ɵĻ�������޸ĵĻ�ԭ������
            List<string> models = new List<string>();
            //models.Add("CourseClass");
            //CreateHostBuilder(args).Build().GenCode(models);   //�����������Ҫ���ɴ��벻������Ŀ�����б����
            CreateHostBuilder(args).Build().InitDB<MyDbContext>().Run();//������Ŀ��ִ�б����
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            //�����Զ����޸������˿������ļ�
            var configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory).AddJsonFile("host.json").Build();
            return Host.CreateDefaultBuilder(args)
                .UseAutofacServiceProviderFactory()//��Ĭ��ServiceProviderFactoryָ��ΪAutofacServiceProviderFactory
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        //Ӧ�������ļ�
                        webBuilder.UseConfiguration(configuration).UseStartup<Startup>();
                    });
        }
    }
}
