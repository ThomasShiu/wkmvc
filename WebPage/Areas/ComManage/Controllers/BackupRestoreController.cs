using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using Common.Enums;
using Common.JsonHelper;
using WebPage.Areas.ComManage.Models;
using WebPage.Controllers;

namespace WebPage.Areas.ComManage.Controllers
{
    public class BackupRestoreController : BaseController
    {
        /// <summary>
        /// 系統備份載入清單
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Backup", OperaAction = "View")]
        public ActionResult Index()
        {
            return View();
        }

        [UserAuthorize(ModuleAlias = "Backup", OperaAction = "BackUpApplication")]
        public ActionResult BackUpApplication()
        {
            return base.View();
        }

        [UserAuthorize(ModuleAlias = "Backup", OperaAction = "BackUpDataBase")]
        public ActionResult BackUpDataBase()
        {
            return base.View();
        }


        [UserAuthorize(ModuleAlias = "Restore", OperaAction = "View")]
        public ActionResult Restore()
        {
            return base.View();
        }

        [UserAuthorize(ModuleAlias = "Backup", OperaAction = "Remove")]
        public ActionResult DeleteBy()
        {
            JsonHelper jsonHelper = new JsonHelper
            {
                Status = "y",
                Msg = "success"
            };
            try
            {
                List<string> list = (from p in base.Request.Form["path"].Trim(new char[]
                {
            ';'
                }).Split(new string[]
                {
            ";"
                }, StringSplitOptions.RemoveEmptyEntries)
                                     select p).ToList<string>();
                foreach (string current in list)
                {
                    FileHelper.DeleteFile(base.Server.MapPath(current));
                }
                base.WriteLog(enumOperator.Remove, "刪除檔：" + list, enumLog4net.WARN);
            }
            catch (Exception e)
            {
                jsonHelper.Status = "err";
                jsonHelper.Msg = "刪除過程中發生錯誤！";
                base.WriteLog(enumOperator.Remove, "刪除檔發生錯誤：", e);
            }
            return base.Json(jsonHelper);
        }
        /// <summary>
        /// 獲取備份檔案資訊
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBackUpData()
        {
            string fileExt = Request.Form["fileExt"];
            string path = "/App_Data/BackUp/";
            var jsonM = new JsonHelper() { Status = "y", Msg = "success" };
            try
            {
                if (!FileHelper.IsExistDirectory(Server.MapPath(path)))
                {
                    jsonM.Status = "n";
                    jsonM.Msg = "目錄不存在！";
                }
                else if (FileHelper.IsEmptyDirectory(Server.MapPath(path)))
                {
                    jsonM.Status = "empty";
                }
                else
                {
                    if (fileExt == "*" || string.IsNullOrEmpty(fileExt))
                    {
                        jsonM.Data = Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(path))).OrderByDescending(p => p.time).ToList();
                    }
                    else
                    {
                        jsonM.Data = Common.Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(path))).OrderByDescending(p => p.time).Where(p => p.ext == fileExt).ToList();
                    }

                }

            }
            catch (Exception)
            {
                jsonM.Status = "err";
                jsonM.Msg = "獲取檔失敗！";
            }
            return Content(JsonConverter.Serialize(jsonM, true));
        }


        /// <summary>
        /// 備份程式檔
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Backup", OperaAction = "BackUpApplication")]
        public ActionResult BackUpFiles()
        {
            var json = new JsonHelper() { Msg = "程式備份完成", Status = "n" };

            try
            {
                //檢查上傳的物理路徑是否存在，不存在則創建
                if (!Directory.Exists(Server.MapPath("/App_Data/BackUp/ApplicationBackUp/")))
                {
                    Directory.CreateDirectory(Server.MapPath("/App_Data/BackUp/ApplicationBackUp/"));
                }

                ZipHelper.ZipDirectory(Server.MapPath("/"), Server.MapPath("/App_Data/BackUp/ApplicationBackUp/"), "App_" + this.CurrentUser.PinYin + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"), true, new List<string>() { Server.MapPath("/App_Data/") });
                WriteLog(Common.Enums.enumOperator.None, "程式備份：" + json.Msg, Common.Enums.enumLog4net.WARN);
                json.Status = "y";
            }
            catch (Exception e)
            {
                json.Msg = "程式備份失敗！";
                WriteLog(Common.Enums.enumOperator.None, "程式備份：", e);
            }

            return Json(json);
        }



        /// <summary>
        /// 備份資料
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Backup", OperaAction = "BackUpDataBase")]
        public ActionResult BackUpData()
        {
            var json = new JsonHelper() { Msg = "資料備份完成", Status = "n" };

            try
            {
                //檢查上傳的物理路徑是否存在，不存在則創建
                if (!Directory.Exists(Server.MapPath("/App_Data/BackUp/DataBaseBackUp/")))
                {
                    Directory.CreateDirectory(Server.MapPath("/App_Data/BackUp/DataBaseBackUp/"));
                }
                //備份資料庫                
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
                {
                    var bakPath = Server.MapPath("/App_Data/BackUp/DataBaseBackUp/");
                    var sql = @"declare @dbname varchar(100);set @dbname = (SELECT db_name())" +
                               "backup database @dbname to disk='" + bakPath + "Data_" + this.CurrentUser.PinYin + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bak'";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        try
                        {
                            conn.Open();
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        finally
                        {
                            conn.Close();
                            cmd.Dispose();
                        }
                    }
                }

                WriteLog(Common.Enums.enumOperator.None, "資料備份：" + json.Msg, Common.Enums.enumLog4net.WARN);
                json.Status = "y";
            }
            catch (Exception e)
            {
                json.Msg = "資料備份失敗！";
                WriteLog(Common.Enums.enumOperator.None, "資料備份：", e);
            }

            return Json(json);
        }


        /// <summary>
        /// 還原資料
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Restore", OperaAction = "RestoreData")]
        public ActionResult RestoreData()
        {
            var json = new JsonHelper() { Msg = "資料還原完成", Status = "n" };

            var path = Request.Form["path"];

            try
            {
                //檢查還原備份的物理路徑是否存在
                if (!System.IO.File.Exists(Server.MapPath(path)))
                {
                    json.Msg = "還原資料失敗，備份檔案不存在或已損壞！";
                    return Json(json);
                }
                //還原資料庫                
                using (SqlConnection Con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
                {

                    try
                    {
                        Con.Open();
                        //todo 因怕資料出錯，還原未修改
                        SqlCommand Com = new SqlCommand("use master restore database wkmvc_comnwes  from disk='" + Server.MapPath(path) + "'", Con);
                        Com.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }

                WriteLog(Common.Enums.enumOperator.None, "資料還原：" + json.Msg, Common.Enums.enumLog4net.WARN);
                json.Status = "y";
            }
            catch (Exception e)
            {
                json.Msg = "資料還原失敗！";
                WriteLog(Common.Enums.enumOperator.None, "資料還原：", e);
            }

            return Json(json);
        }

    }
}