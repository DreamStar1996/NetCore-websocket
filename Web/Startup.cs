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
    /// 框架参考https://gitee.com/AprilBlank/April.Util.Public/blob/master/April.Simple.WebApi/Startup.cs
    /// 
    /// 参考 https://gitee.com/laozhangIsPhi/Blog.Core/blob/master/Blog.Core/Startup.cs
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
        //在ConfigureServices中注册依赖项。在下面的ConfigureContainer方法之前由运行时调用。任何IServiceProvider或ConfigureContainer方法会被调用。
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // 配置Controller全部由Autofac创建默认情况下，Controller的参数会由容器创建，但Controller的创建是有AspNetCore框架实现的。要通过容器创建Controller，需要在Startup中配置一下：
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
            //获取身份证
            //services.AddSingleton<IPrincipalAccessor, PrincipalAccessor>();
            //services.AddSingleton<IPrincipalAccessor, PrincipalAccessor>();
            //services.AddSingleton<IClaimsAccessor, ClaimsAccessor>();

            //注册跨域服务
            //services.AddCors(corsOptions =>
            //{
            //    //添加跨域策略
            //    corsOptions.AddPolicy("any", buidler =>
            //    {
            //        //允许所有访问域、允许所有请求头、允许所有的请求方法
            //        // Console.WriteLine(Configuration.GetValue<string>("CorsOrigins").Split(',').Length);
            //        buidler.WithOrigins(Configuration.GetValue<string>("CorsOrigins").Split(',')).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            //        //buidler.WithOrigins("http://localhost:8080").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            //    });
            //});
            //asp.net core 3.1 MVC/WebApi JSON 全局配置 
            //https://docs.microsoft.com/zh-cn/aspnet/core/web-api/advanced/formatting?view=aspnetcore-3.1
            //System.Text.Json(default) https://cloud.tencent.com/developer/article/1597403
            services.AddControllers(options =>
            {
                // options.Filters.Add(new UserModelActionFilterAttribute()); //注册全全局模型过滤器
            }).AddJsonOptions(options =>
            {
                //格式化日期时间格式
                options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new TimeSpanJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new EnumJsonConverter());
                //数据格式首字母小写
                //options.JsonSerializerOptions.PropertyNamingPolicy =JsonNamingPolicy.CamelCase;
                //数据格式原样输出
                //options.JsonSerializerOptions.PropertyNamingPolicy = null;
                //取消Unicode编码
                //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                //忽略空值
                //options.JsonSerializerOptions.IgnoreNullValues = true;              
                //允许额外符号
                //options.JsonSerializerOptions.AllowTrailingCommas = true;
                //反序列化过程中属性名称是否使用不区分大小写的比较
                //options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
            });
            //注册上下文对象
            //services.AddDbContext<MyDbContext>(options =>
            //{
            //    options.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
            //    //options.EnableSensitiveDataLogging();
            //});//ServiceLifetime.Transient   https://www.cnblogs.com/yaopengfei/p/11349644.html   https://docs.microsoft.com/zh-cn/ef/core/miscellaneous/configuring-dbcontext
            //使用数据池提高性能https://docs.microsoft.com/zh-cn/ef/core/what-is-new/ef-core-2.0/#explicitly-compiled-queries
            services.AddDbContextPool<MyDbContext>(options =>
            {
                //支持sqlserver和mysql数据库。
                var sqlserverconstr = Configuration.GetConnectionString("SqlServer");
                var mysqlconstr = Configuration.GetConnectionString("MySql");
                //Console.WriteLine($"数据库：{sqlserverconstr}");
                if (!string.IsNullOrEmpty(sqlserverconstr))
                {
                    options.UseSqlServer(sqlserverconstr);
                }
                else if (!string.IsNullOrEmpty(mysqlconstr))
                {
                    //mysql驱动：MySql.EntityFrameworkCore。更多驱动https://docs.microsoft.com/zh-cn/ef/core/providers/?tabs=dotnet-core-cli
                    //mysql连接字符串ssl问题： https://blog.csdn.net/qq_41165844/article/details/80372160?utm_medium=distribute.pc_relevant_t0.none-task-blog-2%7Edefault%7EBlogCommendFromMachineLearnPai2%7Edefault-1.control&depth_1-utm_source=distribute.pc_relevant_t0.none-task-blog-2%7Edefault%7EBlogCommendFromMachineLearnPai2%7Edefault-1.control
                    //https://docs.microsoft.com/zh-cn/ef/core/providers/?tabs=dotnet-core-cli
                    //https://segmentfault.com/a/1190000039821570
                    options.UseMySQL(mysqlconstr);
                }
                //开启EF Core的延迟加载功能  使用延迟加载的最简单方式是安装 Microsoft.EntityFrameworkCore.Proxies 包，并通过调用 UseLazyLoadingProxies 来启用。
                options.UseLazyLoadingProxies();
                //options.EnableSensitiveDataLogging();
            });

            //将 Swagger 生成器添加到 Startup.ConfigureServices 方法中的服务集合中： Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerService();

            //将appsettings.json中的JwtSettings部分文件读取到JwtSettings中，这是给其他地方用的
            services.Configure<JwtConfig>(Configuration.GetSection("JwtConfig"));

            //由于初始化的时候我们就需要用，所以使用Bind的方式读取配置
            //将配置绑定到JwtConfig实例中
            var jwtConfig = new JwtConfig();
            Configuration.Bind("JwtConfig", jwtConfig);
            //配置认证授权
            services.ConfigureJwt(jwtConfig);

            //初始化redis
            services.InitRedisConnect(Configuration);
            //RedisClient.redisClient.InitConnect(Configuration);

            //注册微信配置
            services.Configure<WxConfig>(Configuration.GetSection("Weixin"));
            //注册百度配置 
            services.Configure<BDConfig>(Configuration.GetSection("Baidu"));
            //添加redis  https://docs.microsoft.com/zh-cn/aspnet/core/performance/caching/distributed?view=aspnetcore-3.1
            //内存缓存 开发和测试
            services.AddDistributedMemoryCache();
            //非开发环境中的
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = RedisConnectionHelp.RedisConnectionString;
                options.InstanceName = "_cache_";//InstanceName 是自定义的实例名称，创建缓存时会以此名称开头。
            });
            //将HttpClient注入IOC容器
            services.AddHttpClient();
            //启用服务定时器
            services.UseQuartz();
        }
        //配置autofac依赖注入
        //您可以在ConfigureContainer中直接注册内容使用Autofac。 这在ConfigureServices之后运行，这里将覆盖在ConfigureServices中进行的注册。    
        public void ConfigureContainer(ContainerBuilder builder)
        {

            builder.ConfigureAutofac();
        }
        //添加中间件的位置。
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
            //在 Startup.Configure 方法中，启用中间件为生成的 JSON 文档和 Swagger UI 提供服务：
            app.UseSwaggerService();
            //app.UseCors("any");//启动跨域请求
            //app.UseOptions(); // 启动跨域请求  预检测  
            app.UseCorsMiddleware();//自定义跨域
            // 跳转https
            //app.UseHttpsRedirection();

            //DefaultFilesOptions options = new DefaultFilesOptions();
            //options.DefaultFileNames.Add("index2.html");    //将index.html改为需要默认起始页的文件名.
            //app.UseDefaultFiles(options); 
            // 使用静态文件
            app.UseStaticFiles();//用于访问wwwroot下的文件             
            app.UseRouting();
            // 先启动认证中间件
            app.UseAuthentication();
            // 然后是授权中间件
            app.UseAuthorization();//启用授权中间件  必须放在   app.UseRouting(); 和 app.UseEndpoints之间      
            // 自定义授权，放在 app.UseAuthorization()之后 主要用于PrincipalAccessor 
            app.UseMiddleware<JwtAuthorizationFilter>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            #region IM服务端配置
            app.UseImServer(new ImServerOptions
            {
                Redis = new CSRedis.CSRedisClient(Configuration["ImServerOption:CSRedisClient"]),
                Servers = Configuration["ImServerOption:Servers"].Split(";"),
                Server = Configuration["ImServerOption:Server"],
                PathMatch = "/wss"
            });
            #endregion
            #region IM客户端配置，WebApi业务端
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
                    //Console.WriteLine(t.clientId + "上线了");
                    var onlineUids = ImHelper.GetClientListByOnline();
                    //$"用户{t.clientId}上线了"
                    ImHelper.SendMessage(t.clientId, onlineUids, JsonSerializer.Serialize(ImMsg.Online(null, new Sender() { ClientId = t.clientId }), DefaultJsonOptions.Get()));
                },
                t =>
                {
                    //Console.WriteLine(t.clientId + "下线了");
                    var onlineUids = ImHelper.GetClientListByOnline();
                    ImHelper.SendMessage(t.clientId, onlineUids, JsonSerializer.Serialize(ImMsg.Offline(null, new Sender() { ClientId = t.clientId }), DefaultJsonOptions.Get()));
                });
            ImHelper.EventChan(
               t =>
               {
                   var onlineUids = ImHelper.GetChanClientList(t.chan);
                   //Console.WriteLine(t.clientId + "加入频道" + t.chan + "在线列表" + onlineUids.Length + "人数" + ImHelper.GetChanOnline(t.chan));
                   //Console.WriteLine(t.clientId + "加入频道" + t.chan + "在线人数" + onlineUids.Length);
                   if (onlineUids.Length > 0)
                   {
                       ImHelper.SendMessage(t.clientId, onlineUids, JsonSerializer.Serialize(ImMsg.Hello(t.chan, onlineUids.Length, new Sender() { ClientId = t.clientId }), DefaultJsonOptions.Get()));
                   }
               },
               t =>
               {
                   var onlineUids = ImHelper.GetChanClientList(t.chan);
                   // Console.WriteLine(t.clientId + "离开频道" + t.chan + "在线列表" + onlineUids.Length + "人数" + ImHelper.GetChanOnline(t.chan)) ;
                   //Console.WriteLine(t.clientId + "离开频道" + t.chan + "在线人数" + onlineUids.Length);
                   if (onlineUids.Length > 0)
                   {
                       ImHelper.SendMessage(t.clientId, onlineUids, JsonSerializer.Serialize(ImMsg.Bye(t.chan, onlineUids.Length, new Sender() { ClientId = t.clientId }), DefaultJsonOptions.Get()));
                   }
               });
            #endregion
        }
    }
}
