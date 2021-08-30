# 登录与请求、获取用户信息
## 1、登录。获取令牌token
实现登录的控制器为**Web/Controllers/DefaultController**
### （1）普通账号密码登录（JSON请求）
```
var token //登录令牌，请求授权操作方法需携带此令牌
axios.post("http://127.0.0.1:5000/api/login",{username:"admin",password:"123456",type:"user"}).then(r=>{		
	//console.log(r.data)
	if(r.status&&r.data.status){ 
		console.log(r.data.data)//登录用户信息
		token=r.data.data.token//获取登录令牌
	} 
})
```
### （2）微信小程序登录（表单请求）
```
var token //登录令牌，请求授权操作方法需携带此令牌
axios.post("http://127.0.0.1:5000/api/wxMplogin",{code:"小程序code",username:"xioayouyu",avatar:"头像"},
{headers: { 'content-type': 'application/x-www-form-urlencoded' }}).then(r=>{		
	//console.log(r.data)
	if(r.status&&r.data.status){ 
		console.log(r.data.data)//登录用户信息
		token=r.data.data.token//获取登录令牌
	} 
})
```
### （3）APP微信登录（表单请求）
```
var token //登录令牌，请求授权操作方法需携带此令牌
axios.post("http://127.0.0.1:5000/api/wxApplogin",{access_token:"access_token",openid:"openid"},
{headers: { 'content-type': 'application/x-www-form-urlencoded' }}).then(r=>{		
	//console.log(r.data)
	if(r.status&&r.data.status){ 
		console.log(r.data.data)//登录用户信息
		token=r.data.data.token//获取登录令牌
	} 
})
```
## 2、请求接口
请求需要授权才能访问的操作方法（**加[Authorize]注解的方法**）需要在header携带token，如访问**获取用户信息**的操作方法**GetUserinfo**
```
axios.get("http://127.0.0.1:5000/api/getuserinfo",
{headers: { 'Authorization': 'Bearer '+token }}).then(r=>{	//token为登录获取的token值	
	//console.log(r.data)
	if(r.status&&r.data.status){ 
		console.log(r.data.data)//获取用户信息		
	} 
})
```