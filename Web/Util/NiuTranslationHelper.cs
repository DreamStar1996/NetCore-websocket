using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Web.Util
{
    /// <summary>
    /// 小牛翻译
    /// <see cref="https://niutrans.com/documents/develop/develop_text/free"/>
    /// </summary>
    public static class NiuTranslationHelper
    {
        private static IConfiguration configuration;
        private static String apikey = "";
        static NiuTranslationHelper()
        {
            configuration = ConfigHelper.GetService<IConfiguration>();
            var _apikey = configuration["Niutrans:apikey"];
            if (_apikey != null) apikey = _apikey;
        }
        private static Dictionary<string, string> Init(string originalText, string fromLang = "zh", string toLang = "en")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("from", fromLang);
            dic.Add("to", toLang);
            dic.Add("apikey", apikey);
            dic.Add("src_text", HttpUtility.UrlEncode(originalText));
            return dic;
        }
        /// <summary>
        /// 文本翻译。支持免费/收费高级接口。默认免费接口，除非配置文件设置了参数free为false
        /// 语言列表：zh:中文，en:英语，uy：维吾尔语
        /// </summary>
        /// <param name="originalText"></param>
        /// <param name="fromLang">默简体中文zh</param>
        /// <param name="toLang"></param>
        /// <returns></returns>
        public static NiuResult Translation(string originalText, string fromLang = "zh", string toLang = "en")
        {
            //String querys = "from=zh&src_text=%E4%BD%A0%E5%A5%BD&to=en&apikey=" + apikey; 
            String url = "https://api.niutrans.com/NiuTransServer/translation";//高级收费接口
            if (configuration["Niutrans:free"] == null || Convert.ToBoolean(configuration["Niutrans:free"]) ==true)
            {
                url = "https://free.niutrans.com/NiuTransServer/translation";//默认免费接口
            }
            var httpHelper = new HttpHelper();
            return FormattedJson(httpHelper.PostAsync(url, null, Init(originalText, fromLang, toLang)).Result);
        }
        /// <summary>
        /// 翻译xml.文本翻译。支持免费/收费高级接口。默认免费接口，除非配置文件设置了参数free为false
        /// 语言列表：zh:中文，en:英语，uy：维吾尔语
        /// </summary>
        /// <param name="originalXML"></param>
        /// <param name="fromLang"></param>
        /// <param name="toLang"></param>
        /// <returns></returns>
        public static NiuResult TranslationXML(string originalXML, string fromLang = "zh", string toLang = "en")
        {
            //String querys = "from=zh&src_text=%E4%BD%A0%E5%A5%BD&to=en&apikey=" + apikey; 
            String url = "https://api.niutrans.com/NiuTransServer/translationXML";//高级收费接口
            if (configuration["Niutrans:free"] == null || Convert.ToBoolean(configuration["Niutrans:free"]) == true)
            {
                url = "https://free.niutrans.com/NiuTransServer/translationXML";//免费接口
            }
            var httpHelper = new HttpHelper();
            return FormattedJson(httpHelper.PostAsync(url, null, Init(originalXML, fromLang, toLang)).Result);
        }
        private static NiuResult FormattedJson(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
            {
                return new NiuResult();
            }
            var result = JsonConvert.DeserializeObject<NiuResult>(jsonStr);
            //var result = array[0][0][0].ToString();
            return result;
        }

    }
    public class NiuResult
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Apikey { get; set; }
        public string Src_text { get; set; }
        /// <summary>
        /// 翻译结果
        /// </summary>
        public string Tgt_text { get; set; }
        /// <summary>
        /// 翻译状态码
        /// </summary>
        public string Error_code { get; set; }
        public string Error_msg { get; set; }
        /// <summary>
        /// 判断是否翻译成功
        /// </summary>
        /// <returns></returns>
        public bool Status()
        {
            return Error_code == null;
        }
    }
}
