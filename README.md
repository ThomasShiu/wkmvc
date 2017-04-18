# wk_mvc

根據博主[果凍布丁喜之郎](http://yuangang.cnblogs.com)（以下稱“博主”）的教程走的Asp.Net MVC項目。  
已更新到 **2016-08-20** 發佈的 [（16）源碼分享二：登錄功能以及UI、資料庫、倉儲源碼分享](http://www.cnblogs.com/yuangang/p/5789748.html)。   
並且根據博主的[SignalR教程](http://www.cnblogs.com/yuangang/p/5617704.html)實現了**線上聊天室**的功能。

其教程目錄為：<http://www.cnblogs.com/yuangang/p/5581423.html>

## 資料庫說明

我使用的是**LocalDB**，啟動時若無資料庫檔會自動創建，位於`\WebPage\App_Data`。  
**因為`App_Data`資料夾被我設置忽略了，若有需要請手動創建，不然無法自動創建LocalDb資料庫。**  
  
若你使用的是**Sql Server**，只需修改`\WebPage\Web.config`裡的連接字串即可。  
創建後導入`\sql`資料夾裡的sql語句。這些是博主共用的資料庫裡的部分資料，不需要全部用到，可以等運行時看缺少什麼再進行導入。

## 其他
還有我使用的是**VS2015**，有用到部分**C#6.0**的語法。

*用戶名密碼為：admin/pai415926*

2017-4-14 Thomas 進行修改


