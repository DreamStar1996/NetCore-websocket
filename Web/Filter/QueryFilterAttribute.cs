using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model;
using Web.Security;
using Microsoft.AspNetCore.Mvc.Controllers;
using IdentityModel;
using System.Security.Claims;

namespace Web.Filter
{
    /// <summary>
    /// 查询过滤器。
    /// (1)为模型添加用户标志,添加修改设置用户标志。
    /// a、对于add和update方法，如实体类继承了IUser并且用户已经登录，则给实体类自动添加UserId值
    /// b、对于add和update方法，如添加或修改的用户为管理员（角色以"admin"开头），则给实体类sys值设置为true，表示系统资源，否则为false
    /// (2)用于查询添加过滤条件
    /// </summary>
    public class QueryFilterAttribute : ActionFilterAttribute, IActionFilter
    {
        private string[] addupdatemethods = new string[] { "add", "update" };
        private string[] querygetdeletemethods = new string[] { "list", "delete", "batchdelete", "get", "query" };
        private string[] deletegetmethods = new string[] { "delete", "batchdelete", "get" };
        private string[] deletemethods = new string[] { "delete", "batchdelete" };
        // public IClaimsAccessor MyUser { get; set; }
        /// <summary>
        /// OnActionExecuting方法在Controller的Action执行前执行
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //循环获取在Controller的Action方法中定义的参数
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var actionName = actionDescriptor.ActionName.ToLower();
            //Console.WriteLine(actionName); 
            //获取自定义特性 ，用于判断是否需要给model添加身份标志
            var customAttributes = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(QueryableAttribute), false);
            QueryableAttribute queryableAttribute = customAttributes.Length > 0 ? (QueryableAttribute)customAttributes[0] : null;
            var queryable = queryableAttribute != null && queryableAttribute.Enable;
            //用户id
            var userIdStr = context.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Id)?.Value;
            int userId = 0;
            int.TryParse(userIdStr, out userId);
            //用户角色类型
            var roleType = context.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "Type")?.Value;
            //角色
            var role = context.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            //学校编号
            var SchoolIdStr = context.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "SchoolId")?.Value;
            int SchoolId = 0;
            int.TryParse(SchoolIdStr, out SchoolId);
            //如果是增加和修改方法  根据角色类和角色添加 标志id 、添加系统资源标志
            if (addupdatemethods.Any(o => actionName.Contains(o)) || queryable)
            {
                foreach (var parameter in actionDescriptor.Parameters)
                {
                    var parameterName = parameter.Name;//获取Action方法中参数的名字
                    var parameterType = parameter.ParameterType;//获取Action方法中参数的类型
                    if (!typeof(ID).IsAssignableFrom(parameterType))//如果不是ID类型
                    {
                        continue;
                    }
                    //自动添加用户id
                    if (typeof(IUser).IsAssignableFrom(parameterType) && RoleType.user.ToString() == roleType)
                    {
                        var model = context.ActionArguments[parameterName] as IUser;
                        if (userId != 0)
                        {
                            model.UserId = userId;
                        }
                    }

                    //添加系统资源标志  非系统管理员或者非校区管理员、学生、老师添加的资源为非系统资源。系统管理员、校区管理员添加的资源为系统资源                   
                    if (RoleType.user.ToString() == roleType && role != null && !role.StartsWith("admin") && !role.StartsWith("school") || RoleType.student.ToString() == roleType || RoleType.teacher.ToString() == roleType)
                    {
                        var model = context.ActionArguments[parameterName] as ID;
                        model.Sys = false;
                    }
                    else
                    {
                        var model = context.ActionArguments[parameterName] as ID;
                        model.Sys = true;
                    }

                }
            }
            //查询删除方法 过滤条件查询。只能查自己的，除非管理员
            if (querygetdeletemethods.Any(o => actionName.Contains(o)) || queryable)
            {
                foreach (var parameter in actionDescriptor.Parameters)
                {
                    var parameterName = parameter.Name;//获取Action方法中参数的名字
                    var parameterType = parameter.ParameterType;//获取Action方法中参数的类型
                    //判断该Controller的Action方法是否有类型为的参数
                    if (typeof(Dictionary<string, string>).IsAssignableFrom(parameterType))
                    {
                        var where = context.ActionArguments[parameterName] as Dictionary<string, string>;
                        if (deletegetmethods.Any(t => actionName.Contains(t)))//删除\查询单个
                        {
                            where.Remove("id");
                            //首先从路由参数获取
                            string id = context.RouteData.Values["id"] as string;//id=1单个删除,id=1,2,3,4批量删除
                            //再从查询字符串获取
                            id = id ?? context.HttpContext.Request.Query["id"];
                            //再从表单域获取
                            id = id ?? context.HttpContext.Request.Form["id"];
                            if (id != null)
                            {

                                where.Add("Id__in", id);
                            }
                            if (deletemethods.Any(t => actionName.Contains(t)) && role != null && !role.StartsWith("admin"))//非管理员不能删除系统数据
                            {
                                where.Add("Sys", false.ToString());
                            }
                        }
                        else//查询排序
                        {
                            if (!where.ContainsKey("sort")) where.Add("sort", "-Added_time");//排序
                        }
                        if (SchoolId != 0)
                        {
                            where.Add("SchoolId", SchoolId.ToString());
                        }
                        if (userId != 0 && RoleType.user.ToString() == roleType && "admin" != role)//管理员查看所有信息
                        {
                            where.Add("UserId", userId.ToString());
                        }
                        if (userId != 0 && RoleType.student.ToString() == roleType)
                        {
                            where.Add("StudentId", userId.ToString());
                        }

                        //允许显示到客户端前台  这个地方不能加此条件 因为管理后台需要显示
                        /*
                        where.Add("Open", true.ToString());
                        where.Add("Passed", true.ToString());
                        */
                    }
                }
            }
        }

        /// <summary>
        /// OnActionExecuted方法在Controller的Action执行后执行
        /// </summary>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //TODO
        }
    }
}
