# wk_mvc

根据博主[果冻布丁喜之郎](http://yuangang.cnblogs.com)（以下称“博主”）的教程走的Asp.Net MVC项目。  
已更新到 **2016-08-20** 发布的 [（16）源码分享二：登录功能以及UI、数据库、仓储源码分享](http://www.cnblogs.com/yuangang/p/5789748.html)。  
其教程目录为：<http://www.cnblogs.com/yuangang/p/5581423.html>

## 数据库说明

我使用的是**LocalDB**，启动时若无数据库文件会自动创建，位于`\WebPage\App_Data`。  
**因为`App_Data`文件夹被我设置忽略了，若有需要请手动创建，不然无法自动创建LocalDb数据库。**  
  
若你使用的是**Sql Server**，只需修改`\WebPage\Web.config`里的连接字符串即可。  
创建后导入`\sql`文件夹里的sql语句。这些是博主共享的数据库里的部分数据，不需要全部用到，可以等运行时看缺少什么再进行导入。

## 其他
还有我使用的是**VS2015**，有用到部分**C#6.0**的语法。

*用户名密码为：admin/pai415926*

