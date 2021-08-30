# 权限控制

## 一、简单使用
* 登录就能访问  
控制器类或操作方法action添加**[Authorize()]**注解,表示需要经过登录才能访问，而不管登录的角色
```       
        [Route("api/[controller]/[action]")]  
        [ApiController]  
        [Authorize()] //加在控制器，控制器内所有方法需登录访问        
        public class AreaController : MyBaseController  
        {
            IAreaBll bll{geet;set;};  
            [HttpGet]  
            public Result List([FromQuery] Dictionary<string, string> where)  
            {  
                return Result.Success("succeed").SetData(bll.Query(where));  
            }      
        }  
```
```
		[Route("api/[controller]/[action]")]  
		[ApiController]  	  	 
		public class AreaController : MyBaseController  
		{  
		    IAreaBll bll{geet;set;};  
		    [HttpGet]    
			[Authorize()] //加在操作方法，某个方法需登录访问            
		    public Result List([FromQuery] Dictionary<string, string> where)  
		    {   
		        return Result.Success("succeed").SetData(bll.Query(where));  
		    }      
		}  
```	
* 指定**策略**才能访问      
控制器类或操作方法action添加**[Authorize("策略名称")]**注解，表示需要某个策略才能访问。
> 内置策略名称有：admin对应角色admin，user对应角色user
```
        //加在控制器，控制器内所有方法需登录访问   
        [Route("api/[controller]/[action]")]   
        [ApiController]   
        [Authorize("admin")]    //策略为admin才能访问   
        public class AreaController : MyBaseController   
        {   
            IAreaBll bll{geet;set;};    
            [HttpGet]   
            public Result List([FromQuery] Dictionary<string, string> where)   
            {   
                return Result.Success("succeed").SetData(bll.Query(where));   
            }           
        }   
```		
> 注：如何自定义策略名称      
> 1.**策略名称**在Web/Jwt/JwtExtension.cs文件定义注册。一个策略名称对应一个或多个__角色__    
>     #region 授权  
```	     
      services.AddAuthorization(options =>        
      {      
		//AddPolicy("admin" 自定义策略名称         
		//policy => policy.RequireRole("admin")策略要求角色，一个策略可以有多个角色    
        options.AddPolicy("admin", policy => policy.RequireRole("admin").Build());    
        options.AddPolicy("teacher", policy => policy.RequireRole("teacher","admin").Build());    
        options.AddPolicy("student", policy => policy.RequireRole("student").Build());    
      });    
      #endregion 
```			  
> 2.**角色名称**与登录时候生成**token**的角色名称保持一致。在Web/Controllers/DefaultController.cs登录方法生成__token__      
```	
    public class DefaultController : MyBaseController     
      {              
        [HttpPost("login")]             
        public Result Login(Userinfo o)     
        //public Result Login([FromForm]string username, [FromForm]string password,[FromForm]string type="student")            
        {      
            ……          
            if (obj != null)        
            {       
                type = o.Type;       
                var t = new Token() { Uid = obj.Id, Uname = obj.Username, Role = obj.Role, Type = type, TokenType = TokenType.App, Project = project,ClasssId= classid,SchoolId = schoolid };           
                return Result.Success("登录成功")          
                    .SetData(new Userinfo() { Id = obj.Id, Username = obj.Username, Avatar = obj.Photo, Role = obj.Role, Type = o.Type, Realname = obj.Realname, Tel = obj.Tel, Email = obj.Email, Birthday = obj.Birthday, Token = JwtHelper.IssueJWT(t, this.jwtConfig) });                            
            }      
            return Result.Error("登录失败,用户名密码错误").SetData(new Userinfo() { Token = "" });     
        }    
	  }  
```			
>  这里**Role = obj.Role**就是生成token的角色名称。token生成后传递给客户端，客户端每次请求服务端api时携带这个token，然后服务端收到token并解析出角色。      
>  而服务端控制器或方法的**[Authorize("admin")]**注解根据策略名称在上面注册的策略查询到策略对应的角色，与客户端所带token的角色对比，看控制器或方法所需的角色是否包含token的角色，
>  如果包含表示有权限，反之，无权限。  
