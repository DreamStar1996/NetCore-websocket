using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Weixin;
using Web.Util;

namespace Web.Controllers.Weixin
{
    /// <summary>
    /// 微信公众平台接入入口
    /// </summary>
    [Route("api/weixin/")]
    public class WeChatController : Controller
    {
        //private ILog log = LogManager.GetLogger(Startup.repository.Name, typeof(WeChatController));
        /// <summary>
        /// 获取http请求
        /// </summary>
        private HttpHelperOld httpHelper = null;
        /// <summary>
        /// redis缓存
        /// </summary>
        private IDistributedCache redisCache;

        private IOptions<WxConfig> wxConfig;
        //微信账号信息   
        public WeChatController(IDistributedCache cache, IOptions<WxConfig> wxConfig)
        {
            httpHelper = new HttpHelperOld();
            redisCache = cache;
            this.wxConfig = wxConfig;
        }
        /// <summary>
        /// 验证微信签名
        /// </summary>
        /// * 将token、timestamp、nonce三个参数进行字典序排序
        /// * 将三个参数字符串拼接成一个字符串进行sha1加密
        /// * 开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。
        /// <returns></returns>
        [HttpGet("gateway")]
      //  [Route("gateway")]
        public string Gateway(string signature, string timestamp, string nonce, string echostr)
        {
            string token= wxConfig.Value.Mp.Token;
            string[] ArrTmp = { token, timestamp, nonce };
            //字典排序
            Array.Sort(ArrTmp);
            string tmpStr = string.Join("", ArrTmp);
            //字符加密
            var sha1 = new EncryptHelper().HmacSha1Sign(tmpStr);
            if (sha1.Equals(signature))
            {
                // var accessToken = GetAccessToken();
                // return Content(echostr);
                return echostr;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 获取微信access_token
        /// </summary>
        [HttpPost]
        [Route("getaccesstoken")]
        public string GetAccessToken()
        {
            string Appid = wxConfig.Value.Mp.AppID;
            string AppSecret = wxConfig.Value.Mp.AppSecret;
            HttpItem item = new HttpItem
            {
                URL = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={Appid}&secret={AppSecret}"
            };
            var accessToken = httpHelper.GetHtml(item);
            var jObject = JsonConvert.DeserializeObject<WxAaccessToken>(accessToken.Html);
            if (!string.IsNullOrEmpty(jObject.access_token))
            {
                redisCache.SetString("WxAccessToken", jObject.access_token);
            }
            return jObject.access_token;
        }
        #region
        //public ActionResult GetOpenIdList()
        //{
        //    HttpItem item = new HttpItem
        //    {
        //        URL = "https://api.weixin.qq.com/cgi-bin/user/get?" + "access_token=" + GetAccessToken() + "&next_openid="
        //    };
        //    var weChatOpenidlist = httpHelper.GetHtml(item);
        //    return Content(weChatOpenidlist.Html);
        //}
        ///// <summary>
        ///// 创建菜单
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("api/wechat/createmenu")]
        //public ActionResult CreateMenu([FromBody]WeChatMenu menu)
        //{
        //    HttpItem item = new HttpItem
        //    {
        //        URL = $" https://api.weixin.qq.com/cgi-bin/menu/create?access_token={redisCache.GetString("AccessToken")}",
        //        Postdata = Json(menu).ToString()
        //    };
        //    var weChatOpenidlist = httpHelper.GetHtml(item);
        //    return null;
        //}
        #endregion
    }
}