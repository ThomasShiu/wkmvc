using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using Common.JsonHelper;
using Service.IService;
using WebPage.Areas.ComManage.Models;
using WebPage.Controllers;

namespace WebPage.Areas.ComManage.Controllers
{
    public class UploadController : BaseController
    {
        IUploadManage UploadManage { get; set; }
        /// <summary>
        /// 檔管理預設頁面
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "View")]
        public ActionResult Home()
        {
            var fileExt = Request.QueryString["fileExt"] ?? "";
            ViewData["fileExt"] = fileExt;
            return View();
        }

        /// <summary>
        /// 通過路徑獲取所有檔
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllFileData()
        {
            string fileExt = Request.Form["fileExt"];
            var jsonM = new JsonHelper() { Status = "y", Msg = "success" };
            try
            {
                var images = ConfigurationManager.AppSettings["Image"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => "." + p).ToList();
                var videos = ConfigurationManager.AppSettings["Video"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => "." + p).ToList();
                var musics = ConfigurationManager.AppSettings["Music"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => "." + p).ToList();
                var documents = ConfigurationManager.AppSettings["Document"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => "." + p).ToList();

                switch (fileExt)
                {
                    case "images":

                        jsonM.Data = Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(ConfigurationManager.AppSettings["uppath"]))).OrderByDescending(p => p.name).Where(p => images.Any(e => e == p.ext)).ToList();
                        break;
                    case "videos":

                        jsonM.Data = Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(ConfigurationManager.AppSettings["uppath"]))).OrderByDescending(p => p.name).Where(p => videos.Any(e => e == p.ext)).ToList();
                        break;
                    case "musics":

                        jsonM.Data = Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(ConfigurationManager.AppSettings["uppath"]))).OrderByDescending(p => p.name).Where(p => musics.Any(e => e == p.ext)).ToList();
                        break;
                    case "files":

                        jsonM.Data = Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(ConfigurationManager.AppSettings["uppath"]))).OrderByDescending(p => p.name).Where(p => documents.Any(e => e == p.ext)).ToList();
                        break;
                    case "others":

                        jsonM.Data = Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(ConfigurationManager.AppSettings["uppath"]))).OrderByDescending(p => p.name).Where(p => !images.Contains(p.ext) && !videos.Contains(p.ext) && !musics.Contains(p.ext) && !documents.Contains(p.ext)).ToList();
                        break;
                    default:
                        jsonM.Data = Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(ConfigurationManager.AppSettings["uppath"]))).OrderByDescending(p => p.name).ToList();
                        break;
                }

            }
            catch (Exception e)
            {
                jsonM.Status = "err";
                jsonM.Msg = "獲取檔失敗！";
            }
            return Content(JsonConverter.Serialize(jsonM, true));
        }


        /// <summary>
        /// 刪除檔或資料夾
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Remove")]
        public ActionResult DeleteBy()
        {
            var jsonM = new JsonHelper() { Status = "y", Msg = "success" };
            try
            {
                var path = Request.Form["path"].Trim(';').Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();

                foreach (var file in path)
                {
                    //刪除檔
                    FileHelper.DeleteFile(Server.MapPath(file));
                }

                WriteLog(Common.Enums.enumOperator.Remove, "刪除檔：" + path, Common.Enums.enumLog4net.WARN);
            }
            catch (Exception ex)
            {
                jsonM.Status = "err";
                jsonM.Msg = "刪除過程中發生錯誤！";
                WriteLog(Common.Enums.enumOperator.Remove, "刪除檔發生錯誤：", ex);
            }
            return Json(jsonM);
        }



        /// <summary>
        /// 複製檔到資料夾
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Copy")]
        public ActionResult Copy(string files)
        {
            ViewData["Files"] = files;
            ViewData["spath"] = ConfigurationManager.AppSettings["uppath"];
            return View();
        }
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Copy")]
        public ActionResult CopyFiles()
        {
            var json = new JsonHelper() { Msg = "複製檔完成", Status = "n" };

            try
            {
                var files = Request.Form["files"].Trim(';').Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
                var path = Request.Form["path"];
                foreach (var file in files)
                {
                    FileHelper.Copy(Server.MapPath(file), Server.MapPath(path) + FileHelper.GetFileName(Server.MapPath(file)));
                }
                WriteLog(Common.Enums.enumOperator.None, "複製檔：" + Request.Form["files"].Trim(';') + "，結果：" + json.Msg, Common.Enums.enumLog4net.WARN);
                json.Status = "y";
            }
            catch (Exception e)
            {
                json.Msg = "複製檔失敗！";
                WriteLog(Common.Enums.enumOperator.None, "複製檔：", e);
            }

            return Json(json);
        }


        /// <summary>
        /// 移動文件到資料夾
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Cut")]
        public ActionResult Cut(string files)
        {
            ViewData["Files"] = files;
            ViewData["spath"] = ConfigurationManager.AppSettings["uppath"];
            return View();
        }
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Cut")]
        public ActionResult CutFiles()
        {
            var json = new JsonHelper() { Msg = "移動檔完成", Status = "n" };

            try
            {
                var files = Request.Form["files"].Trim(';').Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
                var path = Request.Form["path"];
                foreach (var file in files)
                {
                    FileHelper.Move(Server.MapPath(file), Server.MapPath(path));
                }
                WriteLog(Common.Enums.enumOperator.None, "移動文件：" + Request.Form["files"].Trim(';') + "，結果：" + json.Msg, Common.Enums.enumLog4net.WARN);
                json.Status = "y";
            }
            catch (Exception e)
            {
                json.Msg = "移動檔失敗！";
                WriteLog(Common.Enums.enumOperator.None, "移動文件：", e);
            }

            return Json(json);
        }


        /// <summary>
        /// 壓縮檔
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Compress")]
        public ActionResult Compress(string files)
        {
            ViewData["Files"] = files;
            ViewData["spath"] = ConfigurationManager.AppSettings["uppath"];
            return View();
        }
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Compress")]
        public ActionResult CompressFiles()
        {
            var json = new JsonHelper() { Msg = "壓縮檔完成", Status = "n" };

            try
            {
                var files = Request.Form["files"].Trim(';').Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
                var path = Request.Form["path"];
                foreach (var file in files)
                {
                    ZipHelper.ZipFile(Server.MapPath(file), Server.MapPath(path));
                }
                //ZipHelper.ZipDirectory(Server.MapPath("/upload/files/"), Server.MapPath(path));
                WriteLog(Common.Enums.enumOperator.None, "壓縮檔：" + Request.Form["files"].Trim(';') + "，結果：" + json.Msg, Common.Enums.enumLog4net.WARN);
                json.Status = "y";
            }
            catch (Exception e)
            {
                json.Msg = "壓縮檔失敗！";
                WriteLog(Common.Enums.enumOperator.None, "壓縮檔：", e);
            }

            return Json(json);
        }

        /// <summary>
        /// 解壓文件
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Expand")]
        public ActionResult Expand(string files)
        {
            ViewData["Files"] = files;
            ViewData["spath"] = ConfigurationManager.AppSettings["uppath"];
            return View();
        }
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Expand")]
        public ActionResult ExpandFiles()
        {
            var json = new JsonHelper() { Msg = "解壓檔完成", Status = "n" };

            try
            {
                var files = Request.Form["files"];
                var path = Request.Form["path"];
                var password = Request.Form["password"];

                if (string.IsNullOrEmpty(password))
                {
                    json.Msg = "請輸入解壓密碼！";
                    return Json(json);
                }

                string CurrentPassword = ConfigurationManager.AppSettings["ZipPassword"] != null ? new Common.CryptHelper.AESCrypt().Decrypt(ConfigurationManager.AppSettings["ZipPassword"].ToString()) : "guodongbudingxizhilang";

                if (password != CurrentPassword)
                {
                    json.Msg = "解壓密碼無效！";
                    return Json(json);
                }

                ZipHelper.UnZip(Server.MapPath(files), Server.MapPath(path), password);

                WriteLog(Common.Enums.enumOperator.None, "解壓文件：" + Request.Form["files"].Trim(';') + "，結果：" + json.Msg, Common.Enums.enumLog4net.WARN);
                json.Status = "y";
            }
            catch (Exception e)
            {
                json.Msg = "解壓檔失敗！";
                WriteLog(Common.Enums.enumOperator.None, "解壓文件：", e);
            }

            return Json(json);
        }


        /// <summary>
        /// 單文件上傳視圖
        /// </summary>
        /// <returns></returns>
        public ActionResult FileMain()
        {
            ViewData["spath"] = ConfigurationManager.AppSettings["uppath"];
            return View();
        }

        /// <summary>
        /// 單個文件上傳
        /// </summary>
        [HttpPost]
        public ActionResult SignUpFile()
        {
            var jsonM = new JsonHelper() { Status = "n", Msg = "success" };
            try
            {
                //取得上傳檔
                HttpPostedFileBase upfile = Request.Files["fileUp"];

                //原始檔路徑
                string delpath = Request.QueryString["delpath"];

                //縮略圖
                bool isThumbnail = string.IsNullOrEmpty(Request.QueryString["isThumbnail"]) ? false : Request.QueryString["isThumbnail"].ToLower() == "true" ? true : false;
                //浮水印
                bool isWater = string.IsNullOrEmpty(Request.QueryString["isWater"]) ? false : Request.QueryString["isWater"].ToLower() == "true" ? true : false;


                if (upfile == null)
                {
                    jsonM.Msg = "請選擇要上傳檔！";
                    return Json(jsonM);
                }
                jsonM = FileSaveAs(upfile, isThumbnail, isWater);

                #region 移除原始檔
                if (jsonM.Status == "y" && !string.IsNullOrEmpty(delpath))
                {
                    if (System.IO.File.Exists(Utils.GetMapPath(delpath)))
                    {
                        System.IO.File.Delete(Utils.GetMapPath(delpath));
                    }
                }
                #endregion

                if (jsonM.Status == "y")
                {
                    #region 記錄上傳資料
                    string unit = string.Empty;
                    var jsonValue = JsonConverter.ConvertJson(jsonM.Data.ToString());
                    var entity = new Domain.COM_UPLOAD()
                    {
                        ID = Guid.NewGuid().ToString(),
                        FK_USERID = CurrentUser.Id.ToString(),
                        UPOPEATOR = CurrentUser.Name,
                        UPTIME = DateTime.Now,
                        UPOLDNAME = jsonValue.oldname,
                        UPNEWNAME = jsonValue.newname,
                        UPFILESIZE = FileHelper.GetDiyFileSize(long.Parse(jsonValue.size), out unit),
                        UPFILEUNIT = unit,
                        UPFILEPATH = jsonValue.path,
                        UPFILESUFFIX = jsonValue.ext,
                        UPFILETHUMBNAIL = jsonValue.thumbpath,
                        UPFILEIP = Utils.GetIP(),
                        UPFILEURL = "http://" + Request.Url.AbsoluteUri.Replace("http://", "").Substring(0, Request.Url.AbsoluteUri.Replace("http://", "").IndexOf('/'))
                    };
                    this.UploadManage.Save(entity);
                    #endregion

                    #region 返回檔資訊
                    jsonM.Data = "{\"oldname\": \"" + jsonValue.oldname + "\","; //原始名稱
                    jsonM.Data += " \"newname\":\"" + jsonValue.newname + "\","; //新名稱
                    jsonM.Data += " \"path\": \"" + jsonValue.path + "\", ";  //路徑
                    jsonM.Data += " \"thumbpath\":\"" + jsonValue.thumbpath + "\","; //縮略圖
                    jsonM.Data += " \"size\": \"" + jsonValue.size + "\",";   //大小
                    jsonM.Data += " \"id\": \"" + entity.ID + "\",";   //上傳文件ID
                    jsonM.Data += " \"uptime\": \"" + entity.UPTIME + "\",";   //上傳時間
                    jsonM.Data += " \"operator\": \"" + entity.UPOPEATOR + "\",";   //上傳人
                    jsonM.Data += " \"unitsize\": \"" + entity.UPFILESIZE + unit + "\",";   //帶單位大小
                    jsonM.Data += "\"ext\":\"" + jsonValue.ext + "\"}";//尾碼名
                    #endregion
                }

            }
            catch (Exception ex)
            {
                jsonM.Msg = "上傳過程中發生錯誤，消息：" + ex.Message;
                jsonM.Status = "n";
            }
            return Json(jsonM);
        }



        /// <summary>
        /// 通過路徑獲取檔
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFileData()
        {
            string path = Request.Form["path"];
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
                    jsonM.Data = Utils.DataTableToList<FileModel>(FileHelper.GetFileTable(Server.MapPath(path))).OrderByDescending(p => p.name).ToList();
                }
            }
            catch (Exception)
            {
                jsonM.Status = "err";
                jsonM.Msg = "獲取檔失敗！";
            }
            return Content(JsonConverter.Serialize(jsonM, true));
        }

        #region private
        /// <summary>
        /// 檔上傳方法
        /// </summary>
        /// <param name="postedFile">文件流</param>
        /// <param name="isThumbnail">是否生成縮略圖</param>
        /// <param name="isWater">是否生成浮水印</param>
        /// <returns>上傳後檔資訊</returns>
        private JsonHelper FileSaveAs(HttpPostedFileBase postedFile, bool isThumbnail, bool isWater)
        {
            var jsons = new JsonHelper { Status = "n" };
            try
            {
                string fileExt = Utils.GetFileExt(postedFile.FileName); //文件副檔名，不含“.”
                int fileSize = postedFile.ContentLength; //獲得檔大小，以位元組為單位
                string fileName = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(@"\") + 1); //取得原檔案名
                string newFileName = Utils.GetRamCode() + "." + fileExt; //隨機生成新的檔案名
                string upLoadPath = GetUpLoadPath(fileExt); //上傳目錄相對路徑
                string fullUpLoadPath = Utils.GetMapPath(upLoadPath); //上傳目錄的物理路徑
                string newFilePath = upLoadPath + newFileName; //上傳後的路徑
                string newThumbnailFileName = "thumb_" + newFileName; //隨機生成縮略圖檔案名

                //檢查檔副檔名是否合法
                if (!CheckFileExt(fileExt))
                {
                    jsons.Msg = "不允許上傳" + fileExt + "類型的檔！";
                    return jsons;
                }

                //檢查檔大小是否合法
                if (!CheckFileSize(fileExt, fileSize))
                {
                    jsons.Msg = "檔超過限制的大小啦！";
                    return jsons;
                }

                //檢查上傳的物理路徑是否存在，不存在則創建
                if (!Directory.Exists(fullUpLoadPath))
                {
                    Directory.CreateDirectory(fullUpLoadPath);
                }

                //檢查檔是否真實合法
                //如果檔真實合法 則 保存檔 關閉文件流
                //if (!CheckFileTrue(postedFile, fullUpLoadPath + newFileName))
                //{
                //    jsons.Msg = "不允許上傳不可識別的檔!";
                //    return jsons;
                //}

                //保存檔
                postedFile.SaveAs(fullUpLoadPath + newFileName);

                string thumbnail = string.Empty;

                //如果是圖片，檢查是否需要生成縮略圖，是則生成
                if (IsImage(fileExt) && isThumbnail && ConfigurationManager.AppSettings["ThumbnailWidth"].ToString() != "0" && ConfigurationManager.AppSettings["ThumbnailHeight"].ToString() != "0")
                {
                    Thumbnail.MakeThumbnailImage(fullUpLoadPath + newFileName, fullUpLoadPath + newThumbnailFileName,
                       int.Parse(ConfigurationManager.AppSettings["ThumbnailWidth"]), int.Parse(ConfigurationManager.AppSettings["ThumbnailHeight"]), "W");
                    thumbnail = upLoadPath + newThumbnailFileName;
                }
                //如果是圖片，檢查是否需要打浮水印
                if (IsImage(fileExt) && isWater)
                {
                    switch (ConfigurationManager.AppSettings["WatermarkType"].ToString())
                    {
                        case "1":
                            WaterMark.AddImageSignText(newFilePath, newFilePath,
                                ConfigurationManager.AppSettings["WatermarkText"], int.Parse(ConfigurationManager.AppSettings["WatermarkPosition"]),
                                int.Parse(ConfigurationManager.AppSettings["WatermarkImgQuality"]), ConfigurationManager.AppSettings["WatermarkFont"], int.Parse(ConfigurationManager.AppSettings["WatermarkFontsize"]));
                            break;
                        case "2":
                            WaterMark.AddImageSignPic(newFilePath, newFilePath,
                                ConfigurationManager.AppSettings["WatermarkPic"], int.Parse(ConfigurationManager.AppSettings["WatermarkPosition"]),
                                int.Parse(ConfigurationManager.AppSettings["WatermarkImgQuality"]), int.Parse(ConfigurationManager.AppSettings["WatermarkTransparency"]));
                            break;
                    }
                }

                string unit = string.Empty;

                //處理完畢，返回JOSN格式的檔資訊
                jsons.Data = "{\"oldname\": \"" + fileName + "\",";     //原始檔案名
                jsons.Data += " \"newname\":\"" + newFileName + "\",";  //檔新名稱
                jsons.Data += " \"path\": \"" + newFilePath + "\", ";   //檔路徑
                jsons.Data += " \"thumbpath\":\"" + thumbnail + "\",";  //縮略圖路徑
                jsons.Data += " \"size\": \"" + fileSize + "\",";       //文件大小
                jsons.Data += "\"ext\":\"" + fileExt + "\"}";           //檔案格式
                jsons.Status = "y";
                return jsons;
            }
            catch
            {
                jsons.Msg = "上傳過程中發生意外錯誤！";
                return jsons;
            }
        }

        private bool CheckFileExt(string _fileExt)
        {
            string[] array = new[]
            {
                "asp",
                "aspx",
                "php",
                "jsp",
                "htm",
                "html"
            };
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].ToLower() == _fileExt.ToLower())
                {
                    return false;
                }
            }
            List<string> list = (from p in ConfigurationManager.AppSettings["AttachExtension"].Trim(',').Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                 select p).ToList();
            return list.Contains(_fileExt.ToLower());
        }
        private bool IsImage(string _fileExt)
        {
            List<string> list = (from p in ConfigurationManager.AppSettings["Image"].Trim(',').Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                 select p).ToList();
            return list.Contains(_fileExt.ToLower());
        }
        private bool IsDocument(string _fileExt)
        {
            List<string> list = (from p in ConfigurationManager.AppSettings["Document"].Trim(',').Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                 select p).ToList<string>();
            return list.Contains(_fileExt.ToLower());
        }
        private bool IsVideos(string _fileExt)
        {
            List<string> list = (from p in ConfigurationManager.AppSettings["Video"].Trim(',').Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                 select p).ToList<string>();
            return list.Contains(_fileExt.ToLower());
        }
        private bool IsMusics(string _fileExt)
        {
            List<string> list = (from p in ConfigurationManager.AppSettings["Music"].Trim(',').Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                 select p).ToList();
            return list.Contains(_fileExt.ToLower());
        }

        private bool CheckFileSize(string _fileExt, int _fileSize)
        {
            if (IsImage(_fileExt))
            {
                if (_fileSize / 1024 > int.Parse(ConfigurationManager.AppSettings["AttachImagesize"].ToString()))
                {
                    return false;
                }
            }
            else if (IsVideos(_fileExt))
            {
                if (_fileSize / 1024 > int.Parse(ConfigurationManager.AppSettings["AttachVideosize"].ToString()))
                {
                    return false;
                }
            }
            else if (IsDocument(_fileExt))
            {
                if (_fileSize / 1024 > int.Parse(ConfigurationManager.AppSettings["AttachDocmentsize"].ToString()))
                {
                    return false;
                }
            }
            else if (_fileSize / 1024 > int.Parse(ConfigurationManager.AppSettings["AttachFilesize"].ToString()))
            {
                return false;
            }
            return true;
        }
        private string GetUpLoadPath(string _fileExt)
        {
            string text = ConfigurationManager.AppSettings["uppath"];
            if (IsImage(_fileExt))
            {
                text += "images/";
            }
            else if (IsVideos(_fileExt))
            {
                text += "videos/";
            }
            else if (IsDocument(_fileExt))
            {
                text += "files/";
            }
            else if (IsMusics(_fileExt))
            {
                text += "musics/";
            }
            else if (_fileExt == "bak")
            {
                text = "/App_Data/BackUp/DataBaseBackUp/";
            }
            else
            {
                text += "others/";
            }
            if (!CurrentUser.IsAdmin)
            {
                text = text + CurrentUser.PinYin + "/";
            }
            string text2 = text;
            return string.Concat(new[]
            {
                text2,
                DateTime.Now.ToString("yyyy"),
                "/",
                DateTime.Now.ToString("MM"),
                "/"
            });
        }


        #endregion


    }
}