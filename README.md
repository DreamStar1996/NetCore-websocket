# .Net Core Api脚手架，拿来即用

## 介绍
基于EF Core之Code First模式的NetCoreApi快速开发框架。本脚手架支持代码自动生成，以快速开发.Net Core Api接口，实现前后端分离，支持开发多租户系统。极简至上，拒绝厚重。
## 软件架构
- Asp.net core 3.1+EF Core 3.1(Code First模式)
- JWT+IdentityModel权限控制
- Autofac依赖注入
- Swagger API文档
- StackExchange.Redis/csredis
- Quartz定时器
- Mustachio模板引擎代码生成器
- DotNetCore.NPOI：word/excel支持
- NLog日志系统
- 其他：ChakraCore.NET JavaScript引擎

## 开发环境
- VS2019 / net core 3.1

## 支持数据库
- SQL Server
- MySQL

## 使用教程
#### 1. [快速开发](doc/Base.md)
#### 2. [权限控制](doc/Authority.md)
#### 3. [登录与请求、获取用户信息](doc/Login.md)
#### 4. [查询过滤器](doc/Query.md)
#### 5. [定时器任务Quartz](doc/Quartz.md)
#### 6. [Excel导入与导出](doc/Excel.md) 
## 目录结构
  Model：模型层   
  DAL：数据访问层   
  BLL：业务逻辑层  
  Web：控制器层   
  ImCore：基于Redis的即时聊天websocket核心

