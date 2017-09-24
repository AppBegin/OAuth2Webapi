# OAuth2Webapi
Dotnet Core2.0开发的一组基于OAuth2的用户注册，短信验证等功能的WebAPI。

### 用户注册
POST /api/account/signup    
Content-Type:"application/json"    
body:   
{    
	"phone":"手机号",    
	"password":"密码",    
	"userName":"昵称"    
}    
成功返回：status code:201    
注册成功后，系统会自动通过短信发送验证码到手机。    

### 判断验证码是否正确
POST /api/account/verify    
Content-Type:"application/json"    
body:    
{    
	"phone":"手机号",    
	"code":"验证码"    
}    
成功返回：status code:200    

### 重新发送验证码
GET /api/account/verifycode/手机号    
成功返回：status code:200    
成功后，系统会自动通过短信重新发送验证码到手机。    

### 重新设置密码    
POST /api/account/resetpassword    
Content-Type:"application/json"    
body:    
{    
	"phone":"手机号",    
    "password":"新密码",    
	"code":"验证码"    
}    
成功返回：status code:200    

### 获取用户名
GET /api/account/手机号    
Content-Type:"application/json"    
成功返回：status code:200    

### 获取access_token
POST /connect/token    
Content-Type:"application/x-www-form-urlencoded"     
body:    
username:手机号    
password:密码    
grant_type:password    
client_id:ro.client    
client_secret:secret    
scope:api1    
成功返回：status code:200  

