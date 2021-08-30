using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BD;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Util;

namespace Web.Controllers
{
    /// <summary>
    /// ai智能接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private IOptions<BDConfig> bdConfig;
        //微信账号信息   
        public AiController(IOptions<BDConfig> bdConfig)
        {
            this.bdConfig = bdConfig;
        }
        /// <summary>
        /// 语音识别
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpGet("speech")]
        public Result Speech(IFormFile file)
        {
            BinaryReader r = new BinaryReader(file.OpenReadStream());
            r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
            byte[] pReadByte = r.ReadBytes((int)r.BaseStream.Length);
            BDResult ret = BDSdk.Speech(bdConfig.Value, pReadByte);
            return ret.IsOk() ? Result.Success(ret.result) : Result.Error(ret.err_msg);
        }
        /// <summary>
        ///  文本翻译。支持get。
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fl">原来语言类型。语言列表：zh:中文，en:英语，uy：维吾尔语</param>
        /// <param name="tl">目标语言类型。语言列表：zh:中文，en:英语，uy：维吾尔语</param>
        /// <returns></returns>
        //[HttpGet("translation")]
        [Route("translation")]
        [AcceptVerbs("GET", "POST")]
        public Result Translation(string text, string fl, string tl)
        {
            var ret = NiuTranslationHelper.TranslationXML(text, fl, tl);
            if (ret.Status())
            {
                return Result.Success(ret.Tgt_text);
            }
            else
            {
                return Result.Error(ret.Error_msg);
            }
        }
        /// <summary>
        /// XML翻译。不能超过5000字
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="fl"></param>
        /// <param name="tl"></param>
        /// <returns></returns>
        [HttpGet("translationXML")]
        public Result TranslationXML(string xml, string fl, string tl)
        {
            var ret = NiuTranslationHelper.TranslationXML(xml, fl, tl);
            if (ret.Status())
            {
                return Result.Success(ret.Tgt_text);
            }
            else
            {
                return Result.Error(ret.Error_msg);
            }
        }
        /// <summary>
        /// 翻译网页，待完善。网页字符数太多，该如何优化？
        /// </summary>
        /// <param name="href"></param>
        /// <param name="fl"></param>
        /// <param name="tl"></param>
        /// <returns></returns>
        [HttpGet("translationHTML")]
        public string TranslationHTML(string href, string fl, string tl)
        {
            HttpHelper httpHelper = HttpHelper.Getinstance();
            var html = httpHelper.GetAsync(href, null, null).Result;
            var len = html.Length;
            var pages = Math.Ceiling(html.Length / 5000.00);
            StringBuilder toHtml = new StringBuilder();
            for (var i = 0; i < pages; i++)
            {
                Console.WriteLine("总共"+pages+"当前"+i);
                var ret = NiuTranslationHelper.TranslationXML(html.Substring(i*5000,5000), fl, tl);//暂时有问题
                if (ret.Status())
                {
                    toHtml.Append(ret.Tgt_text);
                }               
            }           
            return toHtml.ToString();
        }

    }
}