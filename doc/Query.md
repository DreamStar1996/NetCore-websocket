# 查询过滤器
查询过滤器Web.Filter.QueryFilterAttribute主要用于给**添加修改、查询删除**操作方法设置条件。   

## 1、查询删除过滤器  
### (1)服务端实现步骤
#### 第1步、给控制器添加[QueryFilter]属性注解
#### 第2步、操作方法命名包含["list", "delete", "batchdelete", "get", "query", "delete"]任何一个（不区分大小写）或者方法添加[Queryable]注解
#### 第3步、操作方法添加[FromQuery] Dictionary<string, string> 类型参数，用于接收客户端查询参数及QueryFilterAttribute过滤器设置预定义查询参数
#### 第4步、调用业务类具有Dictionary<string, string>参数类型的查询方法查询数据
```
 [Route("api/[controller]/[action]")]
 [ApiController]
 [QueryFilter]
public class AreaController: MyBaseController<Area>
  [HttpGet]
  public Result List([FromQuery] Dictionary<string, string> where)
  {
     return Result.Success("succeed").SetData(bll.Query(where));
  }
}
```
> QueryFilterAttribute内置预设查询参数有：     
> 1、对于删除方法"delete"|"batchdelete"：如果用户登录了但非管理员角色admin，
> 则向条件where添加Sys为false，表示非管理员不能删除系统数据，Sys为true表示系统数据   
> 2、对于查询方法"list", "get", "query"：   
> a、设置sort为-Added_time，表示默认按照添加时间降序排列   
> b、如果登录了，根据角色类型添加UserId的值，表示每个会员只能查询自己添加的数据，而管理员可以查询所有数据
> 
### (2)客户端查询写法
#### a、list查询，查询列表
```
/api/area/list?sort=-Id&name=武汉
```
#### b、get查询，查询单个
```
/api/area/get/1
```
#### c、id单个删除
```
/api/area/delete/1
或
/api/area/delete?id=1
```
#### d、id批量删除
```
/api/area/delete/1,2,3
或
/api/area/delete?id=1,2,3
```
#### e、条件批量删除
```
/api/area/delete?name=武汉
```
#### 
## 2、增加修改过滤器  
### (1)服务端实现步骤
#### 第1步、给控制器添加[QueryFilter]属性注解
#### 第2步、操作方法命名包含"add", "update"任何一个（不区分大小写）
#### 第3步、操作方法添加实体类型参数
```
 [Route("api/[controller]/[action]")]
 [ApiController]
 [QueryFilter]
public class AreaController: MyBaseController<Area>
   [HttpPost]
   public Result Add(Area o)
   {
      return ModelState.IsValid ? (bll.Add(o) ? Result.Success("添加成功") : Result.Error("添加失败")) : Result.Error("添加失败!" + ModelState.GetAllErrMsgStr(";")); ;
   }
}
```
> QueryFilterAttribute内置预设查询参数有：     
> 1、如果方法参数实体实现了IUser接口，并且用户登录，则给实体设置UserId值，给数据设置一个标志，表示登录用户进行了增加或修改   
> 2、如果登录用户角色不是管理员，则将实体的Sys设置为false，表示非系统资源，登录用户角色是管理员设置为true，表示是系统资源
> 