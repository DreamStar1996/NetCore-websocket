using ChakraCore.NET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace Web.Util
{
    /// <summary>
    /// 谷歌翻译帮助类。现在有问题
    /// 引入的包有：ChakraCore.NET
    /// <see cref="https://www.cnblogs.com/marso/p/google_translate_api.html"/>
    /// <see cref="https://www.cnblogs.com/WattWang/p/csharpjs.html"/>
    /// <see cref="https://github.com/remixjc/googletranslate-wx"/>
    /// <see cref="https://www.52pojie.cn/thread-707169-1-1.html"/>
    /// </summary>
    public class GoogleTranslationHelper
    {
        /// <summary>
        /// Chakra 上下文
        /// </summary>
        private static readonly ChakraContext _chakraContext;

        /// <summary>
        /// Cookie
        /// </summary>
        private static readonly CookieContainer _cookieContainer;

        /// <summary>
        /// 请求地址
        /// </summary>
        private static readonly string _baseUrl;

        /// <summary>
        /// 静态
        /// </summary>
        static GoogleTranslationHelper()
        {
            var runtime = ChakraRuntime.Create();

            _baseUrl = "https://translate.google.cn/translate_a/single";
            _cookieContainer = new CookieContainer();
            _chakraContext = runtime.CreateContext(true);

            //var basePath = PlatformServices.Default.Application.ApplicationBasePath;
            //var jsFileText = File.ReadAllText("./gettk.js");
            var jsFileText = File.ReadAllText("gettk.js");
            //var jsFileText = File.ReadAllText($@"{basePath}\gettk.js");

            _chakraContext.RunScript(jsFileText); //运行脚本
        }
        /// <summary>
        /// 获取翻译结果(需要翻译的文字默认为自动)
        /// </summary>
        /// <param name="toLang">语言</param>
        /// <param name="originalText">待翻译的文本</param>
        /// <returns></returns>
        public static string Translation(string toLang, string originalText)
        {
            if (string.IsNullOrEmpty(toLang))
            {
                return toLang;
            }
            if (string.IsNullOrEmpty(originalText))
            {
                return originalText;
            }

            //return Translation("zh-cn", toLang, originalText);
            return Translation("auto", toLang, originalText);

        }

        /// <summary>
        /// 获取翻译结果
        /// </summary>
        /// <param name="fromLang">需要翻译的语言</param>
        /// <param name="toLang">翻译结果的语言</param>
        /// <param name="originalText">待翻译文本</param>
        /// <returns></returns>
        public static string Translation(string fromLang, string toLang, string originalText)
        {
            var httpHelper = new HttpHelper();
            string GoogleTransBaseUrl = "https://translate.google.cn";

            var BaseResultHtml = httpHelper.GetAsync(GoogleTransBaseUrl, null, null).Result;
            //匹配tkk
            Regex re = new Regex(@"(tkk:')(.*?)(?=')");

            var TKK = re.Match(BaseResultHtml).ToString().Substring(5);//在返回的HTML中正则匹配TKK的值
            Console.WriteLine(TKK + "tkk3:" + GetTK(originalText, TKK));
            var args = new Dictionary<string, string>
              {
                 { "client", "t" }, 
                 { "sl", fromLang },
                 { "tl", toLang },
                 { "hl", "en" },
                 { "dt", "at" },
                 {"ie","UTF-8" },{"oe","UTF-8" },{"otf","1" },{"ssel","0" },{ "tsel","0"},{ "kc" , "1"},
                 { "tk", GetTK(originalText, TKK) },//tk--ticket即使发车车票，谷歌就靠这个来防止我们免费调用的，这是本API最难的地方。
                 { "q", HttpUtility.UrlEncode(originalText) }
        };

            //var result = HttpHelper.GetRequest(_baseUrl, _cookieContainer, args);
            return httpHelper.GetAsync(_baseUrl, null, args).Result;
            // return result.FormattedJson();
        }
        /// <summary>
        /// 获取TK
        /// </summary>
        /// <param name="originalText"></param>
        /// <param name="TKK"></param>
        /// <returns></returns>
        private static string GetTK(string originalText, string TKK)
        {
            _chakraContext.GlobalObject.WriteProperty("originalText", originalText);
            _chakraContext.GlobalObject.WriteProperty("TKK", TKK);
            return _chakraContext.RunScript("tk(originalText,TKK)");
        }

        /// <summary>
        /// 格式化Json
        /// </summary>
        /// <param name="jsonStr">Json</param>
        /// <returns></returns>
        private static string FormattedJson(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return string.Empty;
            }

            var array = JsonConvert.DeserializeObject<JArray>(jsonStr);

            var result = array[0][0][0].ToString();

            return result;
        }

    }
}
