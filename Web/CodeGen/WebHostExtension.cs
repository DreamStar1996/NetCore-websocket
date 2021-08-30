/* author:QinYongcheng */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Model;
using Microsoft.Extensions.Hosting;
using Web.DB;
using System.Dynamic;
using System.IO;

namespace Web.CodeGen  
{
    /// <summary>
    ///代码生成器。 自动生成dal、bll、controller部分代码
    /// </summary>
    public static class WebHostExtension
    {
        public static IHost GenCode(this IHost host, List<string> models)
          
        {
            //Console.WriteLine(Environment.CurrentDirectory);
            string projectDir = Environment.CurrentDirectory;
            string baseDir = Environment.CurrentDirectory.Replace("\\Web", "");
            string[] projects = new string[] { "DAL", "BLL" };
            string content = "";                    
            foreach (var m in models)
            {
                dynamic model = new ExpandoObject();
                model.entityName = m;
                foreach (var p in projects)
                {
                    //接口
                    using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + $"/CodeGen/I{p}.txt"))
                    {
                        var template = Mustachio.Parser.Parse(sr.ReadToEnd());
                        // 创建模板数据，也可以是 Dictionary<string,object> 类型的             
                        // 解析获取最终数据
                        content = template(model);
                    }
                    using (StreamWriter sw = new StreamWriter(baseDir + $"\\{p}\\I{m}{p}.cs", false))
                    {
                        //保存数据到文件
                        sw.Write(content);
                    }
                    //实现
                    using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + $"/CodeGen/{p}.txt"))
                    {
                        var template = Mustachio.Parser.Parse(sr.ReadToEnd());
                        // 创建模板数据，也可以是 Dictionary<string,object> 类型的             
                        // 解析获取最终数据
                        content = template(model);
                    }
                    using (StreamWriter sw = new StreamWriter(baseDir + $"\\{p}\\Impl\\{m}{p}.cs", false))
                    {
                        //保存数据到文件
                        sw.Write(content);
                    }
                }
                using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + "/CodeGen/Controller.txt"))
                {
                    var template = Mustachio.Parser.Parse(sr.ReadToEnd());
                    // 创建模板数据，也可以是 Dictionary<string,object> 类型的             
                    // 解析获取最终数据
                    content = template(model);
                }
                using (StreamWriter sw = new StreamWriter(baseDir + $"\\Web\\Controllers\\Admin\\{m}Controller.cs", false))
                {
                    //保存数据到文件
                    sw.Write(content);
                    // Console.WriteLine(content);
                }
            }
            return host;
        }
    }

}
