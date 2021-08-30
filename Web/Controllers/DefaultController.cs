using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Bll;
using Common;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Web.Extension;
using Web.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Web.Util;
using System.Security.Claims;
using Web.Security;
using System.Drawing.Imaging;
using hyjiacan.py4n;
using Weixin;
using Newtonsoft.Json;
using System.Net.Http;
using Quartz;
using Web.Filter;

namespace Web.Controllers
{
    /// <summary>
    ///常用的接口
    /// </summary>
    [Route("api/")]
    [ApiController]
    public class DefaultController : MyBaseController<Object>
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly JwtConfig jwtConfig;
        public IUserBll userBll { get; set; }//通过属性依赖注入       
        public IImageBll imageBll { get; set; }
        //  public IClaimsAccessor MyUser { get; set; }
        IConfiguration configuration { get; set; }
        private IHttpClientFactory _httpClientFactory; //注入HttpClient工厂类
    
        private IOptions<WxConfig> wxConfig;
        public DefaultController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration, IOptions<JwtConfig> jwtConfig, IOptions<WxConfig> wxConfig, IHttpClientFactory httpClientFactory)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.jwtConfig = jwtConfig.Value;
            this.configuration = configuration;
            this.wxConfig = wxConfig;       
            _httpClientFactory = httpClientFactory;
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="o">接收username、password、type三个参数</param>
        /// <returns></returns>
        // POST: api/login
        [HttpPost("login")]
        //public Result Login([FromForm] Userinfo o)
        public Result Login(Userinfo o)
        //public Result Login([FromForm]string username, [FromForm]string password,[FromForm]string type="student")
        {
            string project = "", type = "";
            int classid = 0, schoolid = 0;
            Person obj = null;
            obj = userBll.Login(o.Username, o.Password);

            if (obj != null)
            {
                type = o.Type;
                var t = new Token() { Uid = obj.Id, Uname = obj.Username, Role = obj.Role, Type = type, TokenType = TokenType.App, Project = project, ClasssId = classid, SchoolId = schoolid };
                return Result.Success("登录成功")
                    .SetData(new Userinfo() { Id = obj.Id, Username = obj.Username, Avatar = obj.Photo, Role = obj.Role, Type = o.Type, Realname = obj.Realname, Tel = obj.Tel, Email = obj.Email, Birthday = obj.Birthday, Token = JwtHelper.IssueJWT(t, this.jwtConfig) });
                //return Result.Success("登录成功").SetData(new Userinfo() { Token = "admin" });
            }
            return Result.Error("登录失败,用户名密码错误").SetData(new Userinfo() { Token = "" });
        }
        /// <summary>
        /// 微信小程序登录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="username">注册用户名</param>
        /// <param name="avatar">头像</param>
        /// <param name="type">用户类型</param>
        /// <returns></returns>
        [HttpPost("wxMplogin")]
        public Result WxMpLogin([FromForm] string code, [FromForm]string username, [FromForm]string avatar, string type = "user")
        {
            string Appid = wxConfig.Value.Mini.AppID;
            string AppSecret = wxConfig.Value.Mini.AppSecret;
            var url = $"https://api.weixin.qq.com/sns/jscode2session?appid={Appid}&secret={AppSecret}&js_code={code}&grant_type=authorization_code";
            //HttpItem item = new HttpItem
            //{
            //    URL = $"https://api.weixin.qq.com/sns/jscode2session?appid={Appid}&secret={AppSecret}&js_code={code}&grant_type=authorization_code"
            //};           
            var httpHelper = new HttpHelper(_httpClientFactory);
            var jObject = JsonConvert.DeserializeObject<Code2SessionResult>(httpHelper.Get(url, null, null).Result);
            if (!string.IsNullOrEmpty(jObject.openid))
            {
                //微信用户
                var obj = userBll.SelectOne(o => o.Wx_openid == jObject.openid);
                if (obj == null)//先注册
                {
                    obj = new User() { Username = username, Pswd = username, Wx_openid = jObject.openid, Photo = avatar, Role = "user", Sys = false };
                    userBll.Add(obj);
                }

                var t = new Token() { Uid = obj.Id, Uname = obj.Username, Role = obj.Role, TokenType = TokenType.App, Project = "", Type = RoleType.user.ToString(), ClasssId = 0, SchoolId = 0 };
                return Result.Success("登录成功")
                    .SetData(new Userinfo() { Id = obj.Id, Username = obj.Username, Avatar = obj.Photo, Role = obj.Role, Type = RoleType.user.ToString(), Realname = obj.Realname, Tel = obj.Tel, Email = obj.Email, Birthday = obj.Birthday, Token = JwtHelper.IssueJWT(t, this.jwtConfig) });
                //return Result.Success("登录成功").SetData(new Userinfo() { Token = "admin" });

            }
            return Result.Error("登录失败" + jObject.errmsg);
        }
        /// <summary>
        /// 微信APP登录。参考
        /// https://developers.weixin.qq.com/doc/oplatform/Mobile_App/WeChat_Login/Development_Guide.html
        /// 
        /// https://www.cnblogs.com/zhaozi/p/5527739.html
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="openid"></param>
        /// <param name="type"></param>
        /// <returns></returns>

        [HttpPost("wxApplogin")]
        public Result WxAppLogin([FromForm] string access_token, [FromForm] string openid, [FromForm] string type = "user")
        {
            //Console.WriteLine("access_token:" + access_token);
            //Console.WriteLine("openid:" + openid);
            var httpHelper = new HttpHelper(_httpClientFactory);
            //第三步：拉取用户信息(需scope为 snsapi_userinfo)
            var url = $"https://api.weixin.qq.com/sns/userinfo?access_token={access_token}&openid={openid}&lang=zh_CN";
            var userJSON = JsonConvert.DeserializeObject<Dictionary<string, object>>(httpHelper.Get(url, null, null).Result);
            //Console.WriteLine("userJSON:" + JsonConvert.SerializeObject(userJSON));
            if (!userJSON.ContainsKey("errcode"))//请求成功
            {
                string username = userJSON["nickname"].ToString(), avatar = userJSON["headimgurl"].ToString(), sex = userJSON["sex"].ToString();
                //Console.WriteLine("username:" + username);
                //Console.WriteLine(" avatar:" + avatar);
                //微信用户
                var obj = userBll.SelectOne(o => o.Wx_openid == openid);
                if (obj == null)//先注册
                {
                    obj = new User() { Username = username, Pswd = username, Wx_openid = openid, Photo = avatar, Role = "user", Sys = false, Gender = (Gender)Enum.Parse(typeof(Gender), sex) };
                    userBll.Add(obj);
                }

                var t = new Token() { Uid = obj.Id, Uname = obj.Username, Role = obj.Role, TokenType = TokenType.App, Project = "", Type = RoleType.user.ToString(), ClasssId = 0, SchoolId = 0 };
                return Result.Success("登录成功")
                    .SetData(new Userinfo() { Id = obj.Id, Username = obj.Username, Avatar = obj.Photo, Role = obj.Role, Type = RoleType.user.ToString(), Realname = obj.Realname, Tel = obj.Tel, Email = obj.Email, Birthday = obj.Birthday, Token = JwtHelper.IssueJWT(t, this.jwtConfig) });
                //return Result.Success("登录成功").SetData(new Userinfo() { Token = "admin" });

            }
            else
            {
                return Result.Error("登录失败" + userJSON["errmsg"]);
            }

        }
        //public Result WxAppLogin([FromForm] string code, string type = "user")
        //{
        //    Console.WriteLine("code:" + code);
        //    //第一步：通过回调地址获取code
        //    //第二步：通过code换取网页授权access_token
        //    string Appid = wxConfig.Value.App.AppID;
        //    Console.WriteLine("Appid:" + Appid);
        //    string AppSecret = wxConfig.Value.App.AppSecret;
        //    Console.WriteLine("AppSecret:" + AppSecret);
        //    var url = $"https://api.weixin.qq.com/sns/oauth2/access_token?appid={Appid}&secret={AppSecret}&code={code}&grant_type=authorization_code";
        //    var httpHelper = new HttpHelper(_httpClientFactory);
        //    var tokenJSON = JsonConvert.DeserializeObject<Dictionary<string, string>>(httpHelper.Get(url, null, null).Result);
        //    Console.WriteLine("tokenJSON:" + JsonConvert.SerializeObject(tokenJSON));
        //    if (!tokenJSON.ContainsKey("errcode"))//请求成功
        //    {
        //        string access_token = tokenJSON["access_token"];
        //        string openid = tokenJSON["openid"];
        //        Console.WriteLine("access_token:" + access_token);
        //        Console.WriteLine("openid:" + openid);
        //        //第三步：拉取用户信息(需scope为 snsapi_userinfo)
        //        url = $"https://api.weixin.qq.com/sns/userinfo?access_token={access_token}&openid={openid}&lang=zh_CN";
        //        var userJSON = JsonConvert.DeserializeObject<Dictionary<string, string>>(httpHelper.Get(url, null, null).Result);
        //        Console.WriteLine("userJSON:" + userJSON.ToString());
        //        if (!userJSON.ContainsKey("errcode"))//请求成功
        //        {
        //            string username = userJSON["nickname"], avatar = userJSON["headimgurl"];
        //            Console.WriteLine("username:" + username);
        //            Console.WriteLine(" avatar:" + avatar);
        //            //微信用户
        //            var obj = userBll.SelectOne(o => o.Wx_openid == openid);
        //            if (obj == null)//先注册
        //            {
        //                obj = new User() { Username = username, Pswd = username, Wx_openid = openid, Photo = avatar, Role = "user", Sys = false };
        //                userBll.Add(obj);
        //            }

        //            var t = new Token() { Uid = obj.Id, Uname = obj.Username, Role = obj.Role, TokenType = TokenType.App, Project = "", Type = RoleType.user.ToString(), ClasssId = 0, SchoolId = 0 };
        //            return Result.Success("登录成功")
        //                .SetData(new Userinfo() { Id = obj.Id, Username = obj.Username, Avatar = obj.Photo, Role = obj.Role, Type = RoleType.user.ToString(), Realname = obj.Realname, Tel = obj.Tel, Email = obj.Email, Birthday = obj.Birthday, Token = JwtHelper.IssueJWT(t, this.jwtConfig) });
        //            //return Result.Success("登录成功").SetData(new Userinfo() { Token = "admin" });

        //        }
        //        else
        //        {
        //            return Result.Error("登录失败" + userJSON["errmsg"]);
        //        }
        //    }
        //    return Result.Error("登录失败" + JsonConvert.SerializeObject(tokenJSON));
        //}


        // GET: api/getuserinfo
        /// <summary>
        /// 获取登录信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("getuserinfo")]
        [Authorize]
        public Result GetUserinfo()
        {
            Person obj = null;

            obj = this.userBll.SelectOne(MyUser.Id);

            if (obj != null)
            {
                obj.Pswd = "";
                return Result.Success("获取成功").SetData(obj);
            }
            return Result.Error("获取失败").SetData(new Userinfo() { });
        }

      
        /// <summary>
        /// 生成二维码图片
        /// </summary>
        /// <param name="text">生成的文字</param>
        /// <param name="size">生成二维码图片的像素大小</param>
        // [HttpGet("qrcode/{text}/{size}")]
        [HttpGet("qrcode")]
        public void GetQRCode(string text, int size = 5)
        {
            Response.ContentType = "image/jpeg";
            var bitmap = QRCodeHelper.GetQRCode(text, size);
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Jpeg);
            Response.Body.WriteAsync(ms.GetBuffer(), 0, Convert.ToInt32(ms.Length));
            Response.Body.Close();
        }
        /// <summary>
        /// APP更新检查
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="version"></param>
        /// <param name="imei"></param>
        /// <param name="platform"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        [HttpGet("app_update")]
        //[Route("app_update")]
        public Result AppUpdate(string appid, string version, string imei, string platform, string source)
        {
            if ("app1名称" == source)
            {
                var v = configuration["App:app1:version"];
                var m = configuration["App:app1:msg"];
                var u = configuration["App:app1:url"];
                if (Convert.ToInt32(v) > Convert.ToInt32(version))
                {
                    return Result.Success("有新版本啦，要更新吗").SetData(new { msg = m, url = u });
                }
                return Result.Error("无更新版本");
            }
            else
            {
                var v = configuration["App:app2:version"];
                var m = configuration["App:app2:msg"];
                var u = configuration["App:app2:url"];
                if (Convert.ToInt32(v) > Convert.ToInt32(version))
                { 
                    return Result.Success("有新版本啦，要更新吗").SetData(new { msg = m, url = u });
                }
                return Result.Error("无更新版本");
            }

        }
        [HttpGet("test")]
        public Result Test()
        {
            // Console.WriteLine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
            // NLogHelper.logger.Error("XXX");
            return Result.Error("测试");
        }
    }
}

