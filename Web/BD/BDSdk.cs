/* author:QinYongcheng */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BD
{
    /// <summary>
    /// 百度SDK
    /// </summary>
    public class BDSdk
    {
        public static BDResult Speech(BDConfig cfg,byte[] data )
        {
            var client = new Baidu.Aip.Speech.Asr(cfg.AppID, cfg.APIKey, cfg.SecretKey);
            //语音二进制数据, 语音文件的格式，pcm 或者 wav 或者 amr。不区分大小写
            //var data = File.ReadAllBytes("语音pcm文件地址");
            // 可选参数
            var options = new Dictionary<string, object>
     {
        {"dev_pid", 1537}
     };
            client.Timeout = 120000; // 若语音较长，建议设置更大的超时时间. ms
            var result = client.Recognize(data, "pcm", 16000, options).ToObject<BDResult>();
            return result;
        }
    }
}
