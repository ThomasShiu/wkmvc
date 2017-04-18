using Common;
using Common.Enums;
using Domain;
using Microsoft.CSharp.RuntimeBinder;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Transactions;
using System.Web.Mvc;
using WebPage.Controllers;
using Common.JsonHelper;

namespace WebPage.Areas.SysManage.Controllers
{
    public class BussinessCustomerController : BaseController
    {
        #region 聲明容器
        /// <summary>
        /// 公司客戶管理
        /// </summary>
        IBussinessCustomerManage BussinessCustomerManage { get; set; }
        /// <summary>
        /// 省市區管理
        /// </summary>
        ICodeAreaManage CodeAreaManage { get; set; }
        /// <summary>
        /// 大資料欄位管理
        /// </summary>
        IContentManage ContentManage { get; set; }
        /// <summary>
        /// 編碼管理
        /// </summary>
        ICodeManage CodeManage { get; set; }
        #endregion



        /// <summary>
        /// 客戶管理載入主頁
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "BussinessCustomer", OperaAction = "View")]
        public ActionResult Index()
        {
            try
            {

                #region 處理查詢參數
                //接收省份
                string Province = Request.QueryString["Province"];
                ViewData["Province"] = Province;
                //接收客戶類型
                string CustomerStyle = Request.QueryString["CustomerStyle"];
                ViewData["CustomerStyle"] = CustomerStyle;
                //文字方塊輸入查詢關鍵字
                ViewBag.Search = base.keywords;
                #endregion

                ViewData["ProvinceList"] = CodeAreaManage.LoadListAll(p => p.LEVELS == 1);
                ViewBag.KHLX = this.CodeManage.LoadAll(p => p.CODETYPE == "LXRLX").OrderBy(p => p.SHOWORDER).ToList();

                //輸出客戶分頁清單
                return View(BindList(Province, CustomerStyle));
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "客戶管理載入主頁：", e);
                throw e.InnerException;
            }
        }
        #region 幫助方法及其他控制器調用
        /// <summary>
        /// 分頁查詢公司客戶清單
        /// </summary>
        private Common.PageInfo BindList(string Province, string CustomerStyle)
        {
            //基礎資料（緩存）
            var query = this.BussinessCustomerManage.LoadAll(null);

            //非超級管理員只允許查看使用者所在部門客戶
            if (!CurrentUser.IsAdmin)
            {
                query = query.Where(p => p.Fk_DepartId == CurrentUser.DptInfo.ID);
            }

            //客戶所在省份
            if (!string.IsNullOrEmpty(Province))
            {
                query = query.Where(p => p.CompanyProvince == Province);
            }

            //客戶類型
            if (!string.IsNullOrEmpty(CustomerStyle))
            {
                int styleId = int.Parse(CustomerStyle);
                query = query.Where(p => p.CustomerStyle == styleId);
            }

            //查詢關鍵字
            if (!string.IsNullOrEmpty(keywords))
            {
                keywords = keywords.ToLower();
                query = query.Where(p => p.CompanyName.Contains(keywords) || p.ChargePersionName.Contains(keywords));
            }
            //排序
            query = query.OrderByDescending(p => p.UpdateDate).OrderByDescending(p => p.ID);
            //分頁
            var result = this.BussinessCustomerManage.Query(query, page, pagesize);

            var list = result.List.Select(p => new
            {
                p.ID,
                p.CompanyName,
                p.IsValidate,
                CompanyProvince = this.CodeAreaManage.Get(m => m.ID == p.CompanyProvince)?.NAME,
                CompanyCity = this.CodeAreaManage.Get(m => m.ID == p.CompanyCity)?.NAME,
                CompanyArea = this.CodeAreaManage.Get(m => m.ID == p.CompanyArea)?.NAME,
                p.CompanyTel,
                p.ChargePersionName,
                p.CreateUser,
                CreateDate = p.CreateDate.ToString("yyyy-MM-dd"),
                CustomerStyle = GetCustomerStyle(p.CustomerStyle)


            }).ToList();

            return new Common.PageInfo(result.Index, result.PageSize, result.Count, JsonConverter.JsonClass(list));
        }
        private string GetCustomerStyle(int codevalue)
        {
            string nAMETEXT = this.CodeManage.Get((SYS_CODE p) => p.CODEVALUE == codevalue.ToString() && p.CODETYPE == "LXRLX").NAMETEXT;
            switch (codevalue)
            {
                case 1:
                    return "<span class=\"btn btn-danger btn-xs p210\">" + nAMETEXT + "</span>";
                case 2:
                    return "<span class=\"btn btn-warning btn-xs p210\">" + nAMETEXT + "</span>";
                default:
                    return "<span class=\"btn btn-white btn-xs p210\">" + nAMETEXT + "</span>";
            }
        }

        /// <summary>
        /// 載入詳情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "BussinessCustomer", OperaAction = "Detail")]
        public ActionResult Detail(int? id)
        {
            //初始化客戶
            var entity = new Domain.SYS_BUSSINESSCUSTOMER() { ChargePersionSex = 1 };

            if (id != null && id > 0)
            {
                //客戶實體
                entity = BussinessCustomerManage.Get(p => p.ID == id);
                //公司介紹
                ViewData["CompanyInstroduce"] = ContentManage.Get(p => p.FK_RELATIONID == entity.FK_RELATIONID && p.FK_TABLE == "SYS_BUSSINESSCUSTOMER") ?? new Domain.COM_CONTENT();
            }

            //客戶類型
            ViewBag.KHLX = this.CodeManage.LoadAll(p => p.CODETYPE == "LXRLX").OrderBy(p => p.SHOWORDER).ToList();

            return View(entity);
        }




        /// <summary>
        /// 保存客戶資訊
        /// </summary>
        [ValidateInput(false)]
        [UserAuthorizeAttribute(ModuleAlias = "BussinessCustomer", OperaAction = "Add,Edit")]
        public ActionResult Save(Domain.SYS_BUSSINESSCUSTOMER entity)
        {
            bool isEdit = false;
            var FK_RELATIONID = "";
            var json = new JsonHelper() { Msg = "保存成功", Status = "n" };
            try
            {
                if (entity != null)
                {
                    //公司簡介資料ID
                    var contentId = Request["ContentId"] == null ? 0 : Int32.Parse(Request["ContentId"].ToString());

                    if (entity.ID <= 0) //添加
                    {
                        FK_RELATIONID = Guid.NewGuid().ToString();
                        entity.FK_RELATIONID = FK_RELATIONID;
                        entity.Fk_DepartId = this.CurrentUser.DptInfo == null ? "" : this.CurrentUser.DptInfo.ID;
                        entity.CreateUser = CurrentUser.Name;
                        entity.CreateDate = DateTime.Now;
                        entity.UpdateUser = CurrentUser.Name;
                        entity.UpdateDate = DateTime.Now;

                    }
                    else //修改
                    {
                        FK_RELATIONID = entity.FK_RELATIONID;
                        entity.UpdateUser = CurrentUser.Name;
                        entity.UpdateDate = DateTime.Now;
                        isEdit = true;
                    }
                    //同部門下 客戶名稱不能重複
                    if (!this.BussinessCustomerManage.IsExist(p => p.CompanyName.Equals(entity.CompanyName) && p.ID != entity.ID && p.Fk_DepartId == entity.Fk_DepartId))
                    {
                        using (TransactionScope ts = new TransactionScope())
                        {
                            try
                            {
                                if (this.BussinessCustomerManage.SaveOrUpdate(entity, isEdit))
                                {
                                    if (contentId <= 0)
                                    {
                                        this.ContentManage.Save(new Domain.COM_CONTENT()
                                        {
                                            CONTENT = Request["Content"],
                                            FK_RELATIONID = FK_RELATIONID,
                                            FK_TABLE = "SYS_BUSSINESSCUSTOMER",
                                            CREATEDATE = DateTime.Now
                                        });
                                    }
                                    else
                                    {
                                        this.ContentManage.Update(new Domain.COM_CONTENT()
                                        {
                                            ID = contentId,
                                            CONTENT = Request["Content"],
                                            FK_RELATIONID = FK_RELATIONID,
                                            FK_TABLE = "SYS_BUSSINESSCUSTOMER",
                                            CREATEDATE = DateTime.Now
                                        });
                                    }
                                    json.Status = "y";

                                }

                                ts.Complete();

                            }
                            catch (Exception e)
                            {
                                json.Msg = "保存客戶資訊發生內部錯誤！";
                                WriteLog(Common.Enums.enumOperator.None, "保存客戶錯誤：", e);
                            }

                        }
                    }
                    else
                    {
                        json.Msg = "客戶已經存在，請不要重複添加!";
                    }
                }
                else
                {
                    json.Msg = "未找到要操作的客戶記錄";
                }
                if (isEdit)
                {
                    WriteLog(Common.Enums.enumOperator.Edit, "修改客戶[" + entity.CompanyName + "]，結果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
                else
                {
                    WriteLog(Common.Enums.enumOperator.Add, "添加客戶[" + entity.CompanyName + "]，結果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
            }
            catch (Exception e)
            {
                json.Msg = "保存客戶資訊發生內部錯誤！";
                WriteLog(Common.Enums.enumOperator.None, "保存客戶錯誤：", e);
            }
            return Json(json);

        }

        /// <summary>
        /// 客戶資訊
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(ModuleAlias = "BussinessCustomer", OperaAction = "View")]
        public ActionResult CustomerInfo(int id)
        {
            SYS_BUSSINESSCUSTOMER entity = this.BussinessCustomerManage.Get((SYS_BUSSINESSCUSTOMER p) => p.ID == id) ?? new SYS_BUSSINESSCUSTOMER();
            base.ViewData["CompanyInstroduce"] = ((this.ContentManage.Get((COM_CONTENT p) => p.FK_RELATIONID == entity.FK_RELATIONID && p.FK_TABLE == "SYS_BUSSINESSCUSTOMER") == null) ? "" : this.ContentManage.Get((COM_CONTENT p) => p.FK_RELATIONID == entity.FK_RELATIONID && p.FK_TABLE == "SYS_BUSSINESSCUSTOMER").CONTENT);
            return base.View(entity);
        }

        /// <summary>
        /// 刪除客戶
        /// 刪除原則：1、刪除客戶資訊
        ///           2、刪除客戶公司簡介資料
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "Remove")]
        public ActionResult Delete(string idList)
        {
            var json = new JsonHelper() { Status = "n", Msg = "刪除客戶成功" };
            try
            {
                //是否為空
                if (string.IsNullOrEmpty(idList)) { json.Msg = "未找到要刪除的客戶"; return Json(json); }

                var id = idList.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();

                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        foreach (var item in id)
                        {
                            //刪除客戶公司簡介
                            var entity = BussinessCustomerManage.Get(p => p.ID == item);
                            ContentManage.Delete(p => p.FK_RELATIONID == entity.FK_RELATIONID && p.FK_TABLE == "SYS_BUSSINESSCUSTOMER");
                        }
                        //刪除客戶資訊
                        BussinessCustomerManage.Delete(p => id.Contains(p.ID));

                        WriteLog(Common.Enums.enumOperator.Remove, "刪除客戶：" + json.Msg, Common.Enums.enumLog4net.WARN);

                        ts.Complete();

                    }
                    catch (Exception e)
                    {
                        json.Msg = "刪除客戶發生內部錯誤！";
                        WriteLog(Common.Enums.enumOperator.Remove, "刪除客戶：", e);
                    }

                }
            }
            catch (Exception e)
            {
                json.Msg = "刪除客戶發生內部錯誤！";
                WriteLog(Common.Enums.enumOperator.Remove, "刪除客戶：", e);
            }
            return Json(json);
        }
        #endregion
    }
}