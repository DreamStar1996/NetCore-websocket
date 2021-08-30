using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Linq;
using System.Net.Cache;
using System.Net.Http;
using System.Threading.Tasks;

namespace Web.Util
{
    /// <summary>
    /// Http连接操作帮助类
    ///<see cref="https://www.cnblogs.com/johnyong/p/13296793.html"/>
    /// </summary>
    public class HttpHelper
    {
        private static HttpHelper instance=null;
        public static HttpHelper Getinstance()
        {
            if (instance == null)
            {
                instance = new HttpHelper();
            }
            return instance;
        }
        /// <summary>
        /// 注入http请求
        /// </summary>
        private readonly IHttpClientFactory httpClientFactory;
        public HttpHelper()
        {
            httpClientFactory = ConfigHelper.GetService<IHttpClientFactory>();
        }
        public HttpHelper(IHttpClientFactory _httpClientFactory)
        {
            httpClientFactory = _httpClientFactory;
        }
        private void Init(HttpRequestMessage request)
        {
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Add("User-Agent", GetUserAgent());

        }
        // <summary>
        // Get请求数据.弃用
        // <para>最终以url参数的方式提交</para>
        // </summary>
        // <param name="parameters">参数字典,可为空</param>
        // <param name="requestUri">例如/api/Files/UploadFile</param>
        // <returns></returns>
        public async Task<string> Get(string requestUri, Dictionary<string, string> parameters, string token)
        {
            //从工厂获取请求对象
            var client = httpClientFactory.CreateClient();
            //添加请求头
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }
            //这句加上会报Misused header name. Make sure request headers are used with HttpRequestMess 
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json;charset=utf-8");   
            //拼接地址
            if (parameters != null)
            {
                var strParam = string.Join("&", parameters.Select(o => o.Key + "=" + o.Value));
                var seperator = requestUri.Contains("?") ? "&" : "?";//判断是否已经带了参数
                requestUri = string.Concat(requestUri, seperator, strParam);
            }
            //client.BaseAddress = new Uri(requestUri);
            return await client.GetStringAsync(requestUri);
            //return  client.GetStringAsync(requestUri).Result;
        }
        public async Task<string> GetAsync(string requestUri, Dictionary<string, string> headers, Dictionary<string, string> parameters, int timeoutSecond = 120)
        {
            //从工厂获取请求对象
            var client = httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            this.Init(request);
            //添加请求头
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            //这句加上会报Misused header name. Make sure request headers are used with HttpRequestMess 
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json;charset=utf-8");   
            //拼接地址参数
            if (parameters != null)
            {
                var strParam = string.Join("&", parameters.Select(o => o.Key + "=" + o.Value));
                var seperator = requestUri.Contains("?") ? "&" : "?";//判断是否已经带了参数
                requestUri = string.Concat(requestUri, seperator, strParam);
            }
            client.Timeout = TimeSpan.FromSeconds(timeoutSecond);
            //client.BaseAddress = new Uri(requestUri);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                NLogHelper.logger.Info($"接口请求错误,错误代码{response.StatusCode}，错误原因{response.ReasonPhrase}");
                return null;
                //throw new MyHttpException($"接口请求错误,错误代码{response.StatusCode}，错误原因{response.ReasonPhrase}");

            }
            //return await client.GetStringAsync(requestUri);
            //return  client.GetStringAsync(requestUri).Result;
        }
        public async Task<string> PostAsync(string url, Dictionary<string, string> headers, Dictionary<string, string> parameters, int timeoutSecond=120)
        {
            string requestString = "";
            //拼接地址参数
            if (parameters != null)
            {
                requestString = string.Join("&", parameters.Select(o => o.Key + "=" + o.Value));
            }
            return await PostAsync(url, requestString, headers, timeoutSecond);
        }
        public async Task<string> PostAsync(string url, string requestString, Dictionary<string, string> headers, int timeoutSecond)
        {
            var client = httpClientFactory.CreateClient();
            // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var requestContent = new StringContent(requestString, Encoding.UTF8);
            if (headers != null)
            {
                foreach (var head in headers)
                {
                    requestContent.Headers.Add(head.Key, head.Value);
                }
            }
            client.Timeout = TimeSpan.FromSeconds(timeoutSecond);
            var response = await client.PostAsync(url, requestContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                NLogHelper.logger.Info($"接口请求错误,错误代码{response.StatusCode}，错误原因{response.ReasonPhrase}");
                return null;
                //throw new MyHttpException($"接口请求错误,错误代码{response.StatusCode}，错误原因{response.ReasonPhrase}");
            }
        }

        public async Task<string> PutAsync(string url, string requestString, Dictionary<string, string> headers, int timeoutSecond)
        {
            var client = httpClientFactory.CreateClient();
            var requestContent = new StringContent(requestString);
            if (headers != null)
            {
                foreach (var head in headers)
                {
                    requestContent.Headers.Add(head.Key, head.Value);
                }
            }
            client.Timeout = TimeSpan.FromSeconds(timeoutSecond);
            var response = await client.PutAsync(url, requestContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                NLogHelper.logger.Info($"接口请求错误,错误代码{response.StatusCode}，错误原因{response.ReasonPhrase}");
                return null;
                //throw new MyHttpException($"接口请求错误,错误代码{response.StatusCode}，错误原因{response.ReasonPhrase}");
            }
        }


        public async Task<string> PatchAsync(string url, string requestString, Dictionary<string, string> headers, int timeoutSecond)
        {
            var client = httpClientFactory.CreateClient();
            var requestContent = new StringContent(requestString);
            if (headers != null)
            {
                foreach (var head in headers)
                {
                    requestContent.Headers.Add(head.Key, head.Value);
                }
            }
            client.Timeout = TimeSpan.FromSeconds(timeoutSecond);
            var response = await client.PatchAsync(url, requestContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                NLogHelper.logger.Info($"接口请求错误,错误代码{response.StatusCode}，错误原因{response.ReasonPhrase}");
                return null;
                //throw new MyHttpException($"接口请求错误,错误代码{response.StatusCode}，错误原因{response.ReasonPhrase}");
            }
        }
        public async Task<string> DeleteAsync(string url, Dictionary<string, string> headers, int timeoutSecond)
        {
            var client = httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            if (headers != null)
            {
                foreach (var head in headers)
                {
                    request.Headers.Add(head.Key, head.Value);
                }
            }
            client.Timeout = TimeSpan.FromSeconds(timeoutSecond);
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                NLogHelper.logger.Info($"接口请求错误,错误代码{response.StatusCode}，错误原因{response.ReasonPhrase}");
                return null;
                //throw new MyHttpException($"接口请求错误,错误代码{response.StatusCode}，错误原因{response.ReasonPhrase}");
            }
        }
        /// <summary>
        /// 异步请求（通用）
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="requestString"></param>
        /// <param name="headers"></param>
        /// <param name="timeoutSecond">默认120秒</param>
        /// <returns></returns>
        public async Task<string> RequestAsync(string url, HttpMethod method, string requestString, Dictionary<string, string> headers, int timeoutSecond = 120)
        {
            var client = httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(method, url)
            {
                Content = new StringContent(requestString),
            };
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                NLogHelper.logger.Info($"接口请求错误,错误代码{response.StatusCode}，错误原因{response.ReasonPhrase}");
                return null;
                //throw new MyHttpException($"接口请求错误,错误代码{response.StatusCode}，错误原因{response.ReasonPhrase}");
            }
        }
        /// <summary>
        /// 用户代理
        /// </summary>
        /// <returns></returns>
        private static string GetUserAgent()
        {
            var userAgents = new List<string>
             {
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/22.0.1207.1 Safari/537.1",
                "Mozilla/5.0 (X11; CrOS i686 2268.111.0) AppleWebKit/536.11 (KHTML, like Gecko) Chrome/20.0.1132.57 Safari/536.11",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.6 (KHTML, like Gecko) Chrome/20.0.1092.0 Safari/536.6",
                 "Mozilla/5.0 (Windows NT 6.2) AppleWebKit/536.6 (KHTML, like Gecko) Chrome/20.0.1090.0 Safari/536.6",
                 "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Chrome/19.77.34.5 Safari/537.1",
                 "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.9 Safari/536.5",
                 "Mozilla/5.0 (Windows NT 6.0) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.36 Safari/536.5",
                 "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1063.0 Safari/536.3",
                 "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1063.0 Safari/536.3",
                 "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_8_0) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1063.0 Safari/536.3",
                 "Mozilla/5.0 (Windows NT 6.2) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1062.0 Safari/536.3",
                 "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1062.0 Safari/536.3",
                 "Mozilla/5.0 (Windows NT 6.2) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1061.1 Safari/536.3",
                 "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1061.1 Safari/536.3",
                 "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1061.1 Safari/536.3",
                 "Mozilla/5.0 (Windows NT 6.2) AppleWebKit/536.3 (KHTML, like Gecko) Chrome/19.0.1061.0 Safari/536.3",
                "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/535.24 (KHTML, like Gecko) Chrome/19.0.1055.1 Safari/535.24",
                 "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/535.24 (KHTML, like Gecko) Chrome/19.0.1055.1 Safari/535.24",
                 "Mozilla/5.0 (Macintosh; U; Mac OS X Mach-O; en-US; rv:2.0a) Gecko/20040614 Firefox/3.0.0 ",
                 "Mozilla/5.0 (Macintosh; U; PPC Mac OS X 10.5; en-US; rv:1.9.0.3) Gecko/2008092414 Firefox/3.0.3",
                "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10.5; en-US; rv:1.9.1) Gecko/20090624 Firefox/3.5",
                 "Mozilla/5.0 (Macintosh; U; Intel Mac OS X 10.6; en-US; rv:1.9.2.14) Gecko/20110218 AlexaToolbar/alxf-2.0 Firefox/3.6.14",
                 "Mozilla/5.0 (Macintosh; U; PPC Mac OS X 10.5; en-US; rv:1.9.2.15) Gecko/20110303 Firefox/3.6.15",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.6; rv:2.0.1) Gecko/20100101 Firefox/4.0.1",
                "Opera/9.80 (Windows NT 6.1; U; en) Presto/2.8.131 Version/11.11",
                 "Opera/9.80 (Android 2.3.4; Linux; Opera mobi/adr-1107051709; U; zh-cn) Presto/2.8.149 Version/11.10",
                 "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US) AppleWebKit/531.21.8 (KHTML, like Gecko) Version/4.0.4 Safari/531.21.10",
                 "Mozilla/5.0 (Windows; U; Windows NT 5.2; en-US) AppleWebKit/533.17.8 (KHTML, like Gecko) Version/5.0.1 Safari/533.17.8",
                 "Mozilla/5.0 (Windows; U; Windows NT 6.1; en-US) AppleWebKit/533.19.4 (KHTML, like Gecko) Version/5.0.2 Safari/533.18.5",
                 "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0",
                 "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0)",
                 "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)",
                 "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)"
            };
            return userAgents.OrderBy(x => Guid.NewGuid()).First();
        }
    }
    public class MyHttpException : Exception
    {
        public MyHttpException() : base()
        { }
        public MyHttpException(string message) : base(message)
        {

        }
    }

}