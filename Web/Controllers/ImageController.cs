using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Bll;
using Common;
using Web.Extension;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Web.Filter;
namespace Web.Users.Controllers
{
    /// <summary>
    /// 图片资源管理（包含普通会员、教师、学生）
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [QueryFilter]
    public class ImageController : MyBaseController<Object>
    {
        IImageBll bll;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ImageController(IImageBll bll, IWebHostEnvironment webHostEnvironment)
        {
            this.bll = bll;
            this.webHostEnvironment = webHostEnvironment;
        }
        // GET: api/List/Image
        [HttpGet]
        public Result List([FromQuery] Dictionary<string, string> where)
        {           
            return Result.Success("succeed").SetData(bll.Query(where));
        }

        // GET: api/Image/Get/5
        [HttpGet("{id}")]
        public Result Get([FromQuery] Dictionary<string, string> where)
        {
            return Result.Success("succeed").SetData(bll.SelectOne(where));
        }
        // POST: api/Image/Add
        [HttpPost]
        public Result Add(Image o)
        {
            return ModelState.IsValid ? (bll.Add(o) ? Result.Success("添加成功") : Result.Error("添加失败")) : Result.Error("添加失败!" + ModelState.GetAllErrMsgStr(";")); ;
        }

        // Post: api/Image/Update
        [HttpPost]
        public Result Update(Image o)
        {
            return ModelState.IsValid ? (bll.Update(o) ? Result.Success("修改成功").SetData(o) : Result.Error("修改失败")) : Result.Error("修改失败!" + ModelState.GetAllErrMsgStr(";")); ;
        }
        // Get: api/Images/Delete/5
        [HttpGet("{id}")]
        public Result Delete(int id)
        { 
            var obj = bll.SelectOne(id);
            string filePath = webHostEnvironment.WebRootPath + obj.Path;
            System.IO.File.Delete(filePath);
            return bll.Delete(id) ? Result.Success("删除成功") : Result.Error("删除失败");
        }              
        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<Result> Upload(IFormFile file)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !ImgExt.Contains(ext))
            {
                return Result.Error("上传失败,请选择jpg|jpeg|png|gif类型图片");
            }
            if (file.Length > 0)
            {
                if (file.Length > ImgSizeLimit)
                {
                    var size = ImgSizeLimit / 1024;
                    return Result.Error($"上传失败,文件超过大小{size}KB");
                }
                //汉字转拼音
                //string filename = Pinyin4Net.GetPinyin(file.FileName, PinyinFormat.LOWERCASE);
                string filePath = "/upload/" + Guid.NewGuid().ToString() + ext;
                //string phypath= webHostEnvironment.WebRootPath + filePath;
                //if (System.IO.File.Exists(phypath))
                // {
                //     Guid.NewGuid().ToString()
                // }
                Image o = new Image
                {
                    Name = file.FileName
                };

                o.UserId = MyUser.Id;

                //是否是系统上传
                if (MyUser.Role.StartsWith("admin") || MyUser.Role.StartsWith("school"))
                {
                    o.Sys = true;
                }
                else
                {
                    o.Sys = false;
                }
                o.Path = filePath;
                using (var stream = System.IO.File.Create(webHostEnvironment.WebRootPath + filePath))
                {
                    await file.CopyToAsync(stream);
                }
                bll.Add(o);
                return Result.Success("上传成功").SetData(o);
            }
            else
            {
                return Result.Success("上传失败,空文件");
            }
        }
    }
}
