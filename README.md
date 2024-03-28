# lightpoint-identityserver
Identity authentication template built using Identity Server 4 and Blazor, Razor (UI：Ant Blazor)

一个使用身份认证基架，使用Blazor ServerSide + Rezor Page搭建OAuth流程中使用的视图，分离业务模型，接口，期望以一套代码应对后续复杂的情况(如从IdentityServe4切换至OpenidDict(working hard))。

`目前默认以IdentityServer4来作为模板实现，包括端点、基础实体、接口，并再提供接口支持自定义实现`

|编号|技术内容|依赖产品|简要说明 
|----|:----|:----|:----|
|1|基础框架|.Net 8|后续依据项目的进展，随着 .Net 的版本延申。|
|2|关系数据库| PostgreSQL及一切可使用ORM仓库映射的数据库 |[官网链接]( https://www.postgresql.org/)，项目开始时当时的最新版本。|
|3|ORM|Entity Framework及一切可实现数据访问接口的ORM框架 |依据 .NET 的版本同步使用|
|4|ORM|Dapper及一切可实现数据访问接口的ORM框架 |[代码仓库](https://github.com/DapperLib/Dapper)，[指南](https://dapper-tutorial.net/dapper)，另外一个 .NET 的 ORM 框架，与 EF 配合使用。|
|5|缓存|MemoryCache可通过实现切换至redis或者其他缓存技术|
|6|用户身份体系|ASP.NET Core Identity |[代码仓库](https://github.com/dotnet/AspNetCore/tree/main/src/Identity)，[指南](https://learn.microsoft.com/zh-cn/aspnet/core/security/authentication/identity?view=aspnetcore-8.0&tabs=visual-studio)|
|7|用户认证与安全访问|IdentityServer4及一切可以实现OAuth2.0的框架 |[代码仓库](https://github.com/DuendeSoftware/IdentityServer)，[指南](https://docs.duendesoftware.com/identityserver/v6/overview/)|
|8|后台作业|Hangfire|[代码仓库](https://github.com/HangfireIO/Hangfire)，[指南](https://docs.hangfire.io/en/latest/)，在 .NET  应用程序中执行后台处理的简单方法。无需 Windows 服务或单独的进程，由持久存储支持.|
|9|日志|Serilog|[代码仓库](https://github.com/serilog/serilog)，[指南](https://github.com/serilog/serilog/wiki)|
|10|与邮件处理相关|Mailkit/Minekit|[代码仓库](https://github.com/jstedfast/MailKit)，[指南](http://www.mimekit.net/docs/html/Introduction.htm)|
|11|使用动态 Linq |System.Linq.Dynamic.Core|[代码仓库](https://github.com/zzzprojects/System.Linq.Dynamic.Core)，[指南](https://dynamic-linq.net/overview)|
|12|规约模式应用支持|Ardalis.Specification|[代码仓库](https://github.com/ardalis/Specification)，[指南](http://specification.ardalis.com/) 本项目中主要采用 DDD，其中对于规约模式主要用户将业务规则（通常是隐式业务规则）封装成独立的逻辑单元，从而将隐式业务规则提炼为显示概念，并达到代码复用的目的。| 
|13|中介模式应用支持|MediatR|[代码仓库](https://github.com/jbogard/MediatR)，[指南](https://github.com/jbogard/MediatR/wiki)|
|14|简洁架构设计参考|简洁架构|[代码仓库及指南](https://github.com/ardalis/cleanarchitecture)|
