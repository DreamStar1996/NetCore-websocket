using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Extension
{
    /// <summary>
    /// 分布式缓存扩展方法
    /// </summary>
    public static class DistributedCacheExtensions
    {
        public static T GetObject<T>(this IDistributedCache cache, string key)
        {
            var buff= cache.Get(key);
            // byte[]先转换为json对象字符串
            var strJson = Encoding.UTF8.GetString(buff);
            // 把json对象字符串转换为指定对象 需要using Newtonsoft.Json;            
            return JsonConvert.DeserializeObject<T>(strJson);
        }
        public static void SetObject<T>(this IDistributedCache cache, string key,T val)
        {
            // 对象先转换为 json对象字符串
            string json = JsonConvert.SerializeObject(val);
            //json对象字符串转为byte[]
            byte[] serializedResult = Encoding.UTF8.GetBytes(json);
            cache.Set(key, serializedResult);
        }
    }
}
