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
        #region 声明容器
        /// <summary>
        /// 公司客户管理
        /// </summary>
        IBussinessCustomerManage BussinessCustomerManage { get; set; }
        /// <summary>
        /// 省市区管理
        /// </summary>
        ICodeAreaManage CodeAreaManage { get; set; }
        /// <summary>
        /// 大数据字段管理
        /// </summary>
        IContentManage ContentManage { get; set; }
        /// <summary>
        /// 编码管理
        /// </summary>
        ICodeManage CodeManage { get; set; }
        #endregion



        /// <summary>
        /// 客户管理加载主页
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "BussinessCustomer", OperaAction = "View")]
        public ActionResult Index()
        {
            try
            {

                #region 处理查询参数
                //接收省份
                string Province = Request.QueryString["Province"];
                ViewData["Province"] = Province;
                //接收客户类型
                string CustomerStyle = Request.QueryString["CustomerStyle"];
                ViewData["CustomerStyle"] = CustomerStyle;
                //文本框输入查询关键字
                ViewBag.Search = base.keywords;
                #endregion

                ViewData["ProvinceList"] = CodeAreaManage.LoadListAll(p => p.LEVELS == 1);
                ViewBag.KHLX = this.CodeManage.LoadAll(p => p.CODETYPE == "LXRLX").OrderBy(p => p.SHOWORDER).ToList();

                //输出客户分页列表
                return View(BindList(Province, CustomerStyle));
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "客户管理加载主页：", e);
                throw e.InnerException;
            }
        }
        #region 帮助方法及其他控制器调用
        /// <summary>
        /// 分页查询公司客户列表
        /// </summary>
        private Common.PageInfo BindList(string Province, string CustomerStyle)
        {
            //基础数据（缓存）
            var query = this.BussinessCustomerManage.LoadAll(null);

            //非超级管理员只允许查看用户所在部门客户
            if (!CurrentUser.IsAdmin)
            {
                query = query.Where(p => p.Fk_DepartId == CurrentUser.DptInfo.ID);
            }

            //客户所在省份
            if (!string.IsNullOrEmpty(Province))
            {
                query = query.Where(p => p.CompanyProvince == Province);
            }

            //客户类型
            if (!string.IsNullOrEmpty(CustomerStyle))
            {
                int styleId = int.Parse(CustomerStyle);
                query = query.Where(p => p.CustomerStyle == styleId);
            }

            //查询关键字
            if (!string.IsNullOrEmpty(keywords))
            {
                keywords = keywords.ToLower();
                query = query.Where(p => p.CompanyName.Contains(keywords) || p.ChargePersionName.Contains(keywords));
            }
            //排序
            query = query.OrderByDescending(p => p.UpdateDate).OrderByDescending(p => p.ID);
            //分页
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
        /// 加载详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "BussinessCustomer", OperaAction = "Detail")]
        public ActionResult Detail(int? id)
        {
            //初始化客户
            var entity = new Domain.SYS_BUSSINESSCUSTOMER() { ChargePersionSex = 1 };

            if (id != null && id > 0)
            {
                //客户实体
                entity = BussinessCustomerManage.Get(p => p.ID == id);
                //公司介绍
                ViewData["CompanyInstroduce"] = ContentManage.Get(p => p.FK_RELATIONID == entity.FK_RELATIONID && p.FK_TABLE == "SYS_BUSSINESSCUSTOMER") ?? new Domain.COM_CONTENT();
            }

            //客户类型
            ViewBag.KHLX = this.CodeManage.LoadAll(p => p.CODETYPE == "LXRLX").OrderBy(p => p.SHOWORDER).ToList();

            return View(entity);
        }




        /// <summary>
        /// 保存客户信息
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
                    //公司简介数据ID
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
                    //同部门下 客户名称不能重复
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
                                json.Msg = "保存客户信息发生内部错误！";
                                WriteLog(Common.Enums.enumOperator.None, "保存客户错误：", e);
                            }

                        }
                    }
                    else
                    {
                        json.Msg = "客户已经存在，请不要重复添加!";
                    }
                }
                else
                {
                    json.Msg = "未找到要操作的客户记录";
                }
                if (isEdit)
                {
                    WriteLog(Common.Enums.enumOperator.Edit, "修改客户[" + entity.CompanyName + "]，结果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
                else
                {
                    WriteLog(Common.Enums.enumOperator.Add, "添加客户[" + entity.CompanyName + "]，结果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
            }
            catch (Exception e)
            {
                json.Msg = "保存客户信息发生内部错误！";
                WriteLog(Common.Enums.enumOperator.None, "保存客户错误：", e);
            }
            return Json(json);

        }

        /// <summary>
        /// 客户信息
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
        /// 删除客户
        /// 删除原则：1、删除客户信息
        ///           2、删除客户公司简介数据
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "Remove")]
        public ActionResult Delete(string idList)
        {
            var json = new JsonHelper() { Status = "n", Msg = "删除客户成功" };
            try
            {
                //是否为空
                if (string.IsNullOrEmpty(idList)) { json.Msg = "未找到要删除的客户"; return Json(json); }

                var id = idList.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();

                using (TransactionScope ts = new TransactionScope())
                {
                    try
                    {
                        foreach (var item in id)
                        {
                            //删除客户公司简介
                            var entity = BussinessCustomerManage.Get(p => p.ID == item);
                            ContentManage.Delete(p => p.FK_RELATIONID == entity.FK_RELATIONID && p.FK_TABLE == "SYS_BUSSINESSCUSTOMER");
                        }
                        //删除客户信息
                        BussinessCustomerManage.Delete(p => id.Contains(p.ID));

                        WriteLog(Common.Enums.enumOperator.Remove, "删除客户：" + json.Msg, Common.Enums.enumLog4net.WARN);

                        ts.Complete();

                    }
                    catch (Exception e)
                    {
                        json.Msg = "删除客户发生内部错误！";
                        WriteLog(Common.Enums.enumOperator.Remove, "删除客户：", e);
                    }

                }
            }
            catch (Exception e)
            {
                json.Msg = "删除客户发生内部错误！";
                WriteLog(Common.Enums.enumOperator.Remove, "删除客户：", e);
            }
            return Json(json);
        }
        #endregion
    }
}