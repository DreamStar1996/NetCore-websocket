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

namespace Web.Controllers
{
    /// <summary>
    /// 公共的属性、方法、变量
    /// </summary>
    [Controller]
    public abstract class MyBaseController<E> : ControllerBase
    {
        /// <summary>
        /// 授课资源简介长度
        /// </summary>
        public static int IntroLen = 150;
        public static int ArticleIntroLen = 100;
        public string[] ExcelExt = { ".xlsx", ".xls" };
        public int ExcelSizeLimit = 5000 * 1024;
        public string[] ImgExt = { ".jpg", ".jpeg", ".png", ".gif" };
        public int ImgSizeLimit = 500 * 1024;
        /// <summary>
        /// 注入身份信息
        /// </summary>
        public IClaimsAccessor MyUser { get; set; }

        public MyBaseController()
        {
          
        }   
        
    }
}