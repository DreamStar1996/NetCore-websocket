using System;
using System.Text.Json;
using Autofac;
using BD;
using Common.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Model;
using Web.Autofac;
using Web.Extension;
using Web.Jwt;
using Web.Redis;
using Web.Util;
using Weixin;
namespace Web
{
    /// <summary>
    /// ��ܲο�https://gitee.com/AprilBlank/April.Util.Public/blob/master/April.Simple.WebApi/Startup.cs
    /// 
    /// �ο� https://gitee.com/laozhangIsPhi/Blog.Core/blob/master/Blog.Core/Startup.cs
    /// https://www.cnblogs.com/laozhang-is-phi/p/9511869.html
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigHelper.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        //��ConfigureServices��ע��������������ConfigureContainer����֮ǰ������ʱ���á��κ�IServiceProvider��ConfigureContainer�����ᱻ���á�
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // ����Controllerȫ����Autofac����Ĭ������£�Controller�Ĳ�������������������Controller�Ĵ�������AspNetCore���ʵ�ֵġ�Ҫͨ����������Controller����Ҫ��Startup������һ�£�
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            //��ȡ���֤
            //services.AddSingleton<IPrincipalAccessor, PrincipalAccessor>();
            //services.AddSingleton<IPrincipalAccessor, PrincipalAccessor>();
            //services.AddSingleton<IClaimsAccessor, ClaimsAccessor>();

            //ע��������
            //services.AddCors(corsOptions =>
            //{
            //    //��ӿ������
            //    corsOptions.AddPolicy("any", buidler =>
            //    {
            //        //�������з�����������������ͷ���������е����󷽷�
            //        // Console.WriteLine(Configuration.GetValue<string>("CorsOrigins").Split(',').Length);
            //        buidler.WithOrigins(Configuration.GetValue<string>("CorsOrigins").Split(',')).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            //        //buidler.WithOrigins("http://localhost:8080").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            //    });
            //});
            //asp.net core 3.1 MVC/WebApi JSON ȫ������ 
            //https://docs.microsoft.com/zh-cn/aspnet/core/web-api/advanced/formatting?view=aspnetcore-3.1
            //System.Text.Json(default) https://cloud.tencent.com/developer/article/1597403
            services.AddControllers(options =>
            {
                // options.Filters.Add(new UserModelActionFilterAttribute()); //ע��ȫȫ��ģ�͹�����
            }).AddJsonOptions(options =>
            {
                //��ʽ������ʱ���ʽ
                options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new TimeSpanJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new EnumJsonConverter());
                //���ݸ�ʽ����ĸСд
                //options.JsonSerializerOptions.PropertyNamingPolicy =JsonNamingPolicy.CamelCase;
                //���ݸ�ʽԭ�����
                //options.JsonSerializerOptions.PropertyNamingPolicy = null;
                //ȡ��Unicode����
                //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                //���Կ�ֵ
                //options.JsonSerializerOptions.IgnoreNullValues = true;              
                //����������
                //options.JsonSerializerOptions.AllowTrailingCommas = true;
                //�����л����������������Ƿ�ʹ�ò����ִ�Сд�ıȽ�
                //options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
            });
            //ע�������Ķ���
            //services.AddDbContext<MyDbContext>(options =>
            //{
            //    options.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
            //    //options.EnableSensitiveDataLogging();
            //});//ServiceLifetime.Transient   https://www.cnblogs.com/yaopengfei/p/11349644.html   https://docs.microsoft.com/zh-cn/ef/core/miscellaneous/configuring-dbcontext
            //ʹ�����ݳ��������https://docs.microsoft.com/zh-cn/ef/core/what-is-new/ef-core-2.0/#explicitly-compiled-queries
            services.AddDbContextPool<MyDbContext>(options =>
            {
                //֧��sqlserver��mysql���ݿ⡣
                var sqlserverconstr = Configuration.GetConnectionString("SqlServer");
                var mysqlconstr = Configuration.GetConnectionString("MySql");
                //Console.WriteLine($"���ݿ⣺{sqlserverconstr}");
                if (!string.IsNullOrEmpty(sqlserverconstr))
                {
                    options.UseSqlServer(sqlserverconstr);
                }
                else if (!string.IsNullOrEmpty(mysqlconstr))
                {
                    //mysql������MySql.EntityFrameworkCore����������https://docs.microsoft.com/zh-cn/ef/core/providers/?tabs=dotnet-core-cli
                    //mysql�����ַ���ssl���⣺ https://blog.csdn.net/qq_41165844/article/details/80372160?utm_medium=distribute.pc_relevant_t0.none-task-blog-2%7Edefault%7EBlogCommendFromMachineLearnPai2%7Edefault-1.control&depth_1-utm_source=distribute.pc_relevant_t0.none-task-blog-2%7Edefault%7EBlogCommendFromMachineLearnPai2%7Edefault-1.control
                    //https://docs.microsoft.com/zh-cn/ef/core/providers/?tabs=dotnet-core-cli
                    //https://segmentfault.com/a/1190000039821570
                    options.UseMySQL(mysqlconstr);
                }
                //����EF Core���ӳټ��ع���  ʹ���ӳټ��ص���򵥷�ʽ�ǰ�װ Microsoft.EntityFrameworkCore.Proxies ������ͨ������ UseLazyLoadingProxies �����á�
                options.UseLazyLoadingProxies();
                //options.EnableSensitiveDataLogging();
            });

            //�� Swagger ��������ӵ� Startup.ConfigureServices �����еķ��񼯺��У� Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerService();

            //��appsettings.json�е�JwtSettings�����ļ���ȡ��JwtSettings�У����Ǹ������ط��õ�
            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));

            //���ڳ�ʼ����ʱ�����Ǿ���Ҫ�ã�����ʹ��Bind�ķ�ʽ��ȡ����
            //�����ð󶨵�JwtConfigʵ����
            var jwtConfig = new JwtConfig();
            Configuration.Bind("JwtConfig", jwtConfig);
            //������֤��Ȩ
            services.ConfigureJwt(jwtConfig);

            //��ʼ��redis
            services.InitRedisConnect(Configuration);
            //RedisClient.redisClient.InitConnect(Configuration);

            //ע��΢������
            services.Configure<WxConfig>(Configuration.GetSection("Weixin"));
            //ע��ٶ����� 
            services.Configure<BDConfig>(Configuration.GetSection("Baidu"));
            //���redis  https://docs.microsoft.com/zh-cn/aspnet/core/performance/caching/distributed?view=aspnetcore-3.1
            //�ڴ滺�� �����Ͳ���
            services.AddDistributedMemoryCache();
            //�ǿ��������е�
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = RedisConnectionHelp.RedisConnectionString;
                options.InstanceName = "_cache_";//InstanceName ���Զ����ʵ�����ƣ���������ʱ���Դ����ƿ�ͷ��
            });
            //��HttpClientע��IOC����
            services.AddHttpClient();
            //���÷���ʱ��
            services.UseQuartz();
        }
        //����autofac����ע��
        //��������ConfigureContainer��ֱ��ע������ʹ��Autofac�� ����ConfigureServices֮�����У����ｫ������ConfigureServices�н��е�ע�ᡣ    
        public void ConfigureContainer(ContainerBuilder builder)
        {

            builder.ConfigureAutofac();
        }
        //����м����λ�á�
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            ConfigHelper.ServiceProvider = app.ApplicationServices;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // using Microsoft.AspNetCore.HttpOverrides;
            //https://docs.microsoft.com/zh-cn/aspnet/core/host-and-deploy/linux-nginx?view=aspnetcore-2.2
            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //});
            //�� Startup.Configure �����У������м��Ϊ���ɵ� JSON �ĵ��� Swagger UI �ṩ����
            app.UseSwaggerService();
            //app.UseCors("any");//������������
            //app.UseOptions(); // ������������  Ԥ���  
            app.UseCorsMiddleware();//�Զ������
            // ��תhttps
            //app.UseHttpsRedirection();

            //DefaultFilesOptions options = new DefaultFilesOptions();
            //options.DefaultFileNames.Add("index2.html");    //��index.html��Ϊ��ҪĬ����ʼҳ���ļ���.
            //app.UseDefaultFiles(options); 
            // ʹ�þ�̬�ļ�
            app.UseStaticFiles();//���ڷ���wwwroot�µ��ļ�             
            app.UseRouting();
            // ��������֤�м��
            app.UseAuthentication();
            // Ȼ������Ȩ�м��
            app.UseAuthorization();//������Ȩ�м��  �������   app.UseRouting(); �� app.UseEndpoints֮��      
            // �Զ�����Ȩ������ app.UseAuthorization()֮�� ��Ҫ����PrincipalAccessor 
            app.UseMiddleware<JwtAuthorizationFilter>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            #region IM���������
            app.UseImServer(new ImServerOptions
            {
                Redis = new CSRedis.CSRedisClient(Configuration["ImServerOption:CSRedisClient"]),
                Servers = Configuration["ImServerOption:Servers"].Split(";"),
                Server = Configuration["ImServerOption:Server"],
                PathMatch = "/wss"
            });
            #endregion
            #region IM�ͻ������ã�WebApiҵ���
            ImHelper.Initialization(new ImClientOptions
            {
                Redis = new CSRedis.CSRedisClient(Configuration["ImClientOption:CSRedisClient"]),
                Servers = Configuration["ImServerOption:Servers"].Split(";"),
                PathMatch = "/wss"
            });

            ImHelper.Instance.OnSend += (s, e) =>
            {
                // Console.WriteLine($"ImClient.SendMessage(server={e.Server},data={JsonSerializer.Serialize(e.Message)})");
            };

            ImHelper.EventBus(
                t =>
                {
                    //Console.WriteLine(t.clientId + "������");
                    var onlineUids = ImHelper.GetClientListByOnline();
                    //$"�û�{t.clientId}������"
                    ImHelper.SendMessage(t.clientId, onlineUids, JsonSerializer.Serialize(ImMsg.Online(null, new Sender() { ClientId = t.clientId }), DefaultJsonOptions.Get()));
                },
                t =>
                {
                    //Console.WriteLine(t.clientId + "������");
                    var onlineUids = ImHelper.GetClientListByOnline();
                    ImHelper.SendMessage(t.clientId, onlineUids, JsonSerializer.Serialize(ImMsg.Offline(null, new Sender() { ClientId = t.clientId }), DefaultJsonOptions.Get()));
                });
            ImHelper.EventChan(
               t =>
               {
                   var onlineUids = ImHelper.GetChanClientList(t.chan);
                   //Console.WriteLine(t.clientId + "����Ƶ��" + t.chan + "�����б�" + onlineUids.Length + "����" + ImHelper.GetChanOnline(t.chan));
                   //Console.WriteLine(t.clientId + "����Ƶ��" + t.chan + "��������" + onlineUids.Length);
                   if (onlineUids.Length > 0)
                   {
                       ImHelper.SendMessage(t.clientId, onlineUids, JsonSerializer.Serialize(ImMsg.Hello(t.chan, onlineUids.Length, new Sender() { ClientId = t.clientId }), DefaultJsonOptions.Get()));
                   }
               },
               t =>
               {
                   var onlineUids = ImHelper.GetChanClientList(t.chan);
                   // Console.WriteLine(t.clientId + "�뿪Ƶ��" + t.chan + "�����б�" + onlineUids.Length + "����" + ImHelper.GetChanOnline(t.chan)) ;
                   //Console.WriteLine(t.clientId + "�뿪Ƶ��" + t.chan + "��������" + onlineUids.Length);
                   if (onlineUids.Length > 0)
                   {
                       ImHelper.SendMessage(t.clientId, onlineUids, JsonSerializer.Serialize(ImMsg.Bye(t.chan, onlineUids.Length, new Sender() { ClientId = t.clientId }), DefaultJsonOptions.Get()));
                   }
               });
            #endregion
        }
    }
}
