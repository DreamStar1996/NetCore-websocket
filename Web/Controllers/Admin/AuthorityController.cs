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
using Microsoft.AspNetCore.Authorization;
using Web.Controllers;
using Web.Filter;

namespace Web.Controllers.Admin
{
    /// <summary>
    /// 管理员—权限管理
    /// </summary>
    /// <typeparam name="Authority"></typeparam>
    [Route("api/admin/[controller]/[action]")]
    [ApiController]
    [Authorize("admin")]
    [QueryFilter]
    public class AuthorityController : MyBaseController<Authority>
    {
        IAuthorityBll bll;
        public AuthorityController(IAuthorityBll bll)
        {
            this.bll = bll;
        }
        // GET: api/List/Authority
        [HttpGet]
        public Result List([FromQuery] Dictionary<string, string> where)
        {
            //return Result.Success("succeed").SetData(bll.SelectAll(o => true, pageNo, pageSize));
            return Result.Success("succeed").SetData(bll.Query(where));
        }

        // GET: api/Authority/Get/5
        [HttpGet("{id}")]
        public Result Get([FromQuery] Dictionary<string, string> where)
        {
            return Result.Success("succeed").SetData(bll.SelectOne(where));
        }
        // POST: api/Authority/Add
        [HttpPost]
        public Result Add(Authority o)
        {
            return ModelState.IsValid ? (bll.Add(o) ? Result.Success("添加成功") : Result.Error("添加失败")) : Result.Error("添加失败!" + ModelState.GetAllErrMsgStr(";")); ;
        }

        // Post: api/Authority/Update
        [HttpPost]
        public Result Update(Authority o)
        {
            return ModelState.IsValid ? (bll.Update(o) ? Result.Success("修改成功").SetData(o) : Result.Error("修改失败")) : Result.Error("修改失败!" + ModelState.GetAllErrMsgStr(";")); ;
        }

        // Get: api/Authority/Delet/5
        [HttpGet("{id}")]
        public Result Delete([FromQuery] Dictionary<string, string> where)
        {
            return bll.Delete(where) ? Result.Success("删除成功") : Result.Error("删除失败");
        }
        [HttpPost]
        public Result BatchDelete([FromForm] Dictionary<string, string> where)
        {
            return bll.Delete(where) ? Result.Success("删除成功") : Result.Error("删除失败");
        }
    }
}
