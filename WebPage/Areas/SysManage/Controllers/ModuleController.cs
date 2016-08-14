using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Common.Enums;
using Common.JsonHelper;
using Service.IService;
using WebPage.Controllers;

namespace WebPage.Areas.SysManage.Controllers
{
    public class ModuleController : BaseController
    {
        IModuleManage ModuleManage { get; set; }
        IPermissionManage PermissionManage { get; set; }

        ISystemManage SystemManage { get; set; }
        // GET: Module
        [UserAuthorize(ModuleAlias ="Module",OperaAction ="View")] //验证是否有查看权限
        public ActionResult Index()
        {
            try
            {
                //处理查询参数
                string systems = Request.QueryString["System"];
                ViewBag.Search = base.keywords;
                ViewData["System"] = systems;
                ViewData["Systemlist"] = SystemManage.LoadSystemInfo(CurrentUser.System_Id);

                return View(BindList(systems));
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select,"模块管理加载主页：",e);
                throw e.InnerException;
            }
        }

        /// <summary>
        /// 加载模块详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(ModuleAlias ="Module",OperaAction = "Detail")]
        public ActionResult Detail(int? id)
        {
            try
            {
                var _entity = new Domain.SYS_MODULE()
                {
                    ISSHOW = true,
                    MODULEPATH = "javascript:void(0)",
                    MODULETYPE = 1
                };

                //父模块
                string parentId = Request.QueryString["parentId"];
                if (!string.IsNullOrEmpty(parentId))
                {
                    _entity.PARENTID = int.Parse(parentId);
                }
                else
                {
                    _entity.PARENTID = 0;
                }
                //所属系统
                string sys = Request.QueryString["sys"];
                if (!string.IsNullOrEmpty(sys))
                {
                    _entity.FK_BELONGSYSTEM = sys;
                }
                //详情
                if(id != null && id > 0)
                {
                    _entity = ModuleManage.Get(p => p.ID == id);
                }
                //页面类型
                ViewData["ModuleType"] = Enum.GetNames(typeof(enumModuleType));
                //加载用户可操作的系统
                ViewData["Systemlist"] = SystemManage.LoadSystemInfo(CurrentUser.System_Id);

                ViewData["Modules"] = BindList(_entity.FK_BELONGSYSTEM);

                return View(_entity);
            }
            catch(Exception e)
            {
                WriteLog(enumOperator.Select, "模块管理加载详情", e);
                throw;
            }
        }

        /// <summary>
        /// 保存模块
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [UserAuthorize(ModuleAlias ="Module",OperaAction ="Add,Edit")]
        public ActionResult Save(Domain.SYS_MODULE entity)
        {
            bool isEdit = false;
            var json = new JsonHelper { Msg="保存成功",Status="n" };


            try
            {
                if (entity == null)                
                {
                    json.Msg = "未找到需要保存的模块";
                    return Json(json);
                }
                //验证
                if (!Regex.IsMatch(entity.ALIAS, @"^[A-Za-z0-9]{1,20}$"))
                {
                    json.Msg = "模块别名只能以字母数字组成，长度不能超过20个字符";
                    return Json(json);
                }
                //级别加1，一级模块默认0
                if (entity.PARENTID <= 0)
                {
                    entity.LEVELS = 0;
                }
                else
                {
                    entity.LEVELS = this.ModuleManage.Get(p => p.ID == entity.PARENTID).LEVELS + 1;
                }
                //添加
                if (entity.ID <= 0)
                {
                    entity.CREATEDATE = DateTime.Now;
                    entity.CREATEUSER = this.CurrentUser.Name;
                    entity.UPDATEDATE = DateTime.Now;
                    entity.UPDATEUSER = this.CurrentUser.Name;
                }
                else //修改
                {
                    entity.UPDATEDATE = DateTime.Now;
                    entity.UPDATEUSER = this.CurrentUser.Name;
                    isEdit = true;
                }
                //模块别名同系统下不能重复
                if (this.ModuleManage.IsExist(p => p.FK_BELONGSYSTEM == entity.FK_BELONGSYSTEM && p.ALIAS == entity.ALIAS && p.ID != entity.ID))
                {
                    json.Msg = "同系统下模块别名不能重复";
                    return Json(json);
                }
                //判断同一个父模块下，是否重名 
                if (!this.ModuleManage.IsExist(p => p.PARENTID == entity.PARENTID && p.FK_BELONGSYSTEM == entity.FK_BELONGSYSTEM && p.NAME == entity.NAME && p.ID != entity.ID))
                {

                    if (this.ModuleManage.SaveOrUpdate(entity, isEdit))
                    {

                        json.Status = "y";
                    }
                    else
                    {
                        json.Msg = "保存失败";
                    }

                    //如果模块修改成功，我们变更下级模块的级别
                    if (isEdit)
                        ModuleManage.MoreModifyModule(entity.ID, entity.LEVELS);
                }
                else
                {
                    json.Msg = "模块" + entity.NAME + "已存在，不能重复添加";
                }               

                
            }
            catch (Exception e)
            {
                json.Msg = "保存模块发生内部错误！";
                WriteLog(Common.Enums.enumOperator.None, "保存模块：", e);
            }
            return Json(json);
        }
        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [UserAuthorize(ModuleAlias ="Module",OperaAction ="Remove")]
        public ActionResult Delete(string idList)
        {
            JsonHelper json = new JsonHelper() { Msg = "删除模块成功", ReUrl = "", Status = "n" };
            try
            {
                if (!string.IsNullOrEmpty(idList))
                {
                    var idlist1 = idList.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();


                    //判断权限里面有没有
                    if (!this.PermissionManage.IsExist(p => idlist1.Any(e => e == p.MODULEID)))
                    {
                        //存在子模块与否
                        if (!this.ModuleManage.IsExist(p => idlist1.Any(e => e == p.PARENTID)))
                        {
                            this.ModuleManage.Delete(p => idlist1.Any(e => e == p.ID));
                            json.Status = "y";
                        }
                        else
                        {
                            json.Msg = "该模块下有子模块，不能删除";
                        }
                    }
                    else
                    {
                        json.Msg = "该模块下有权限节点，不能删除";
                    }

                }
                else
                {
                    json.Msg = "未找到要删除的模块";
                }
                WriteLog(Common.Enums.enumOperator.Remove, "删除模块，结果：" + json.Msg, Common.Enums.enumLog4net.WARN);
            }
            catch (Exception e)
            {
                json.Msg = "删除模块发生内部错误！";
                WriteLog(Common.Enums.enumOperator.Remove, "删除模块：", e);
            }
            return Json(json);
        }
               


        /// <summary>
        /// 查询模块列表
        /// </summary>
        /// <param name="systems">系统ID</param>
        /// <returns></returns>
        private object BindList(string systems)
        {
            var query = ModuleManage.LoadAll(null);
            if (!string.IsNullOrEmpty(systems))
            {
                query = query.Where(p => p.FK_BELONGSYSTEM == systems);
            }else
            {
                query = query.Where(p => CurrentUser.System_Id.Any(e => e == p.FK_BELONGSYSTEM));
            }
            //递归排序
            var entity = ModuleManage.RecursiveModule(query.ToList())
                .Select(p => new
                {
                    p.ID,
                    MODULENAME = GetModuleName(p.NAME,p.LEVELS),
                    p.ALIAS,
                    p.MODULEPATH,
                    p.SHOWORDER,
                    p.ICON,
                    MODULETYPE = ((Common.Enums.enumModuleType)p.MODULETYPE).ToString(),
                    ISSHOW = p.ISSHOW ? "显示":"隐藏",
                    p.NAME,
                    SYSNAME = p.SYS_SYSTEM.NAME,
                    p.FK_BELONGSYSTEM
                });
            if (!string.IsNullOrEmpty(base.keywords))
            {
                entity = entity.Where(p => p.NAME.Contains(keywords));
            }
            return Common.JsonHelper.JsonConverter.JsonClass(entity);
        }

        private object GetModuleName(string name,decimal? level)
        {
            if (level > 0)
            {
                string nbsp = "";
                for (int i = 0; i < level; i++)
                {
                    nbsp += "　　";
                }
                name = nbsp + "|--" + name;
            }
            return name;
        }




        public ActionResult ShowIcon()
        {
            ViewData["icon"] = GetIm();
            return View();
        }


        public string[] GetIm()
        {
            string text = "fa fa-bluetooth,fa fa-bluetooth-b,fa fa-codiepie,fa fa-credit-card-alt,fa fa-edge,fa fa-fort-awesome,fa fa-hashtag,fa fa-mixcloud,fa fa-modx,fa fa-pause-circle,fa fa-pause-circle-o,fa fa-percent,fa fa-product-hunt,fa fa-reddit-alien,fa fa-scribd,fa fa-shopping-bag,fa fa-shopping-basket,fa fa-stop-circle,fa fa-stop-circle-o,fa fa-usb,fa fa-adjust,fa fa-anchor,fa fa-archive,fa fa-area-chart,fa fa-arrows,fa fa-arrows-h,fa fa-arrows-v,fa fa-asterisk,fa fa-at,fa fa-automobile,fa fa-balance-scale,fa fa-ban,fa fa-bank,fa fa-bar-chart,fa fa-bar-chart-o,fa fa-barcode,fa fa-bars,fa fa-battery-0,fa fa-battery-1,fa fa-battery-2,fa fa-battery-3,fa fa-battery-4,fa fa-battery-empty,fa fa-battery-full,fa fa-battery-half,fa fa-battery-quarter,fa fa-battery-three-quarters,fa fa-bed,fa fa-beer,fa fa-bell,fa fa-bell-o,fa fa-bell-slash,fa fa-bell-slash-o,fa fa-bicycle,fa fa-binoculars,fa fa-birthday-cake,fa fa-bluetooth,fa fa-bluetooth-b,fa fa-bolt,fa fa-bomb,fa fa-book,fa fa-bookmark,fa fa-bookmark-o,fa fa-briefcase,fa fa-bug,fa fa-building,fa fa-building-o,fa fa-bullhorn,fa fa-bullseye,fa fa-bus,fa fa-cab,fa fa-calculator,fa fa-calendar,fa fa-calendar-check-o,fa fa-calendar-minus-o,fa fa-calendar-o,fa fa-calendar-plus-o,fa fa-calendar-times-o,fa fa-camera,fa fa-camera-retro,fa fa-car,fa fa-caret-square-o-down,fa fa-caret-square-o-left,fa fa-caret-square-o-right,fa fa-caret-square-o-up,fa fa-cart-arrow-down,fa fa-cart-plus,fa fa-cc,fa fa-certificate,fa fa-check,fa fa-check-circle,fa fa-check-circle-o,fa fa-check-square,fa fa-check-square-o,fa fa-child,fa fa-circle,fa fa-circle-o,fa fa-circle-o-notch,fa fa-circle-thin,fa fa-clock-o,fa fa-clone,fa fa-close,fa fa-cloud,fa fa-cloud-download,fa fa-cloud-upload,fa fa-code,fa fa-code-fork,fa fa-coffee,fa fa-cog,fa fa-cogs,fa fa-comment,fa fa-comment-o,fa fa-commenting,fa fa-commenting-o,fa fa-comments,fa fa-comments-o,fa fa-compass,fa fa-copyright,fa fa-creative-commons,fa fa-credit-card,fa fa-credit-card-alt,fa fa-crop,fa fa-crosshairs,fa fa-cube,fa fa-cubes,fa fa-cutlery,fa fa-dashboard,fa fa-database,fa fa-desktop,fa fa-diamond,fa fa-dot-circle-o,fa fa-download,fa fa-automobile,fa fa-ellipsis-h,fa fa-ellipsis-v,fa fa-envelope,fa fa-envelope-o,fa fa-envelope-square,fa fa-eraser,fa fa-exchange,fa fa-exclamation,fa fa-exclamation-circle,fa fa-exclamation-triangle,fa fa-external-link,fa fa-external-link-square,fa fa-eye,fa fa-eye-slashfa fa-eyedropper,fa fa-fax,fa fa-feed,fa fa-female,fa fa-fighter-jet,fa fa-file-archive-o,fa fa-file-audio-o,fa fa-file-code-o,fa fa-file-excel-o,fa fa-file-image-o,fa fa-file-movie-o,fa fa-file-pdf-o,fa fa-file-photo-o,fa fa-file-picture-o,fa fa-file-powerpoint-o,fa fa-file-sound-o,fa fa-file-video-o,fa fa-file-word-o,fa fa-file-zip-o,fa fa-film,fa fa-filter,fa fa-fire,fa fa-fire-extinguisher,fa fa-flag,fa fa-flag-checkered,fa fa-flag-o,fa fa-flashfa fa-flask,fa fa-folder,fa fa-folder-o,fa fa-folder-open,fa fa-folder-open-o,fa fa-frown-o,fa fa-futbol-o,fa fa-gamepad,fa fa-gavel,fa fa-gear,fa fa-gears,fa fa-gift,fa fa-glass,fa fa-globe,fa fa-graduation-cap,fa fa-group,fa fa-hand-grab-o,fa fa-hand-lizard-o,fa fa-hand-paper-o,fa fa-hand-peace-o,fa fa-hand-pointer-o,fa fa-hand-rock-o,fa fa-hand-scissors-o,fa fa-hand-spock-o,fa fa-hand-stop-o,fa fa-hashtag,fa fa-hdd-o,fa fa-headphones,fa fa-heart,fa fa-heart-o,fa fa-heartbeat,fa fa-history,fa fa-home,fa fa-hotel,fa fa-hourglass,fa fa-hourglass-1,fa fa-hourglass-2,fa fa-hourglass-3,fa fa-hourglass-end,fa fa-hourglass-half,fa fa-hourglass-o,fa fa-hourglass-start,fa fa-i-cursor,fa fa-image,fa fa-inbox,fa fa-industry,fa fa-info,fa fa-info-circle,fa fa-institution,fa fa-key,fa fa-keyboard-o,fa fa-language,fa fa-laptop,fa fa-leaf,fa fa-legal,fa fa-lemon-o,fa fa-level-down,fa fa-level-up,fa fa-life-bouy,fa fa-life-buoy,fa fa-life-ring,fa fa-life-saver,fa fa-lightbulb-o,fa fa-line-chart,fa fa-location-arrow,fa fa-lock,fa fa-magic,fa fa-magnet,fa fa-mail-forward,fa fa-mail-reply,fa fa-mail-reply-all,fa fa-male,fa fa-map,fa fa-map-marker,fa fa-map-o,fa fa-map-pin,fa fa-map-signs,fa fa-meh-o,fa fa-microphone,fa fa-microphone-slash,fa fa-minus,fa fa-minus,fa fa-minus-circle,fa fa-minus-square,fa fa-minus-square-o,fa fa-mobile,fa fa-mobile-phone,fa fa-money,fa fa-moon-o,fa fa-mortar-board,fa fa-motorcycle,fa fa-mouse-pointer,fa fa-music,fa fa-navicon,fa fa-newspaper-o,fa fa-object-group,fa fa-object-ungroup,fa fa-paint-brush,fa fa-paper-plane,fa fa-paper-plane-o,fa fa-paw,fa fa-pencil,fa fa-pencil-square,fa fa-pencil-square-o,fa fa-percent,fa fa-phone,fa fa-phone-square,fa fa-photo,fa fa-picture-o,fa fa-pie-chart,fa fa-plane,fa fa-plug,fa fa-plus,fa fa-plus-circle,fa fa-plus-square,fa fa-plus-square-o,fa fa-power-off,fa fa-print,fa fa-puzzle-piece,fa fa-qrcode,fa fa-question,fa fa-question-circle,fa fa-quote-left,fa fa-quote-right,fa fa-random,fa fa-recycle,fa fa-refresh,fa fa-registered,fa fa-remove,fa fa-reorder,fa fa-reply,fa fa-reply-all,fa fa-retweet,fa fa-road,fa fa-rocket,fa fa-rss,fa fa-rss-square,fa fa-search,fa fa-search-minus,fa fa-search-plus,fa fa-send,fa fa-send-o,fa fa-server,fa fa-share,fa fa-share-alt,fa fa-share-alt-square,fa fa-share-square,fa fa-share-square-o,fa fa-shield,fa fa-ship,fa fa-shopping-bag,fa fa-shopping-basket,fa fa-shopping-cart,fa fa-sign-in,fa fa-sign-out,fa fa-signal,fa fa-sitemap,fa fa-sliders,fa fa-smile-o,fa fa-soccer-ball-o,fa fa-sort,fa fa-sort-alpha-asc,fa fa-sort-alpha-desc,fa fa-sort-amount-asc,fa fa-sort-amount-desc,fa fa-sort-asc,fa fa-sort-desc,fa fa-sort-down,fa fa-sort-numeric-asc,fa fa-sort-numeric-desc,fa fa-sort-up,fa fa-space-shuttle,fa fa-spinner,fa fa-spoon,fa fa-square,fa fa-square-o,fa fa-star,fa fa-star-half,fa fa-star-half-empty,fa fa-sticky-note-o,fa fa-street-view,fa fa-suitcase,fa fa-sun-o,fa fa-support,fa fa-tablet,fa fa-tachometer,fa fa-tag,fa fa-tags,fa fa-tasks,fa fa-taxi,fa fa-television,fa fa-terminal,fa fa-thumb-tack,fa fa-thumbs-down,fa fa-thumbs-o-down,fa fa-thumbs-o-up,fa fa-thumbs-up,fa fa-ticket,fa fa-times,fa fa-times-circle,fa fa-times-circle-o,fa fa-tint,fa fa-toggle-down,fa fa-toggle-left,fa fa-toggle-off,fa fa-toggle-on,fa fa-toggle-right,fa fa-toggle-up,fa fa-trademark,fa fa-trash,fa fa-trash-o,fa fa-tree,fa fa-trophy,fa fa-truck,fa fa-tty,fa fa-tv,fa fa-umbrella,fa fa-university,fa fa-unlock,fa fa-unlock-alt,fa fa-unsorted,fa fa-upload,fa fa-user,fa fa-user-plus,fa fa-user-secret,fa fa-user-times,fa fa-users,fa fa-video-camera,fa fa-volume-down,fa fa-volume-off,fa fa-volume-up,fa fa-warning,fa fa-wheelchair,fa fa-wifi,fa fa-wrench,fa fa-hand-grab-o,fa fa-hand-lizard-o,fa fa-hand-o-down,fa fa-hand-o-left,fa fa-hand-o-right,fa fa-hand-o-up,fa fa-hand-paper-o,fa fa-hand-peace-o,fa fa-hand-pointer-o,fa fa-hand-rock-o,fa fa-hand-scissors-o,fa fa-hand-spock-o,fa fa-hand-stop-o,fa fa-thumbs-down,fa fa-thumbs-o-down,fa fa-thumbs-o-up,fa fa-thumbs-up,fa fa-ambulance,fa fa-automobile,fa fa-bicycle,fa fa-bus,fa fa-cab,fa fa-car,fa fa-fighter-jet,fa fa-motorcycle,fa fa-plane,fa fa-rocket,fa fa-ship,fa fa-space-shuttle,fa fa-subway,fa fa-taxi,fa fa-train,fa fa-truck,fa fa-wheelchair,fa fa-genderless,fa fa-intersex,fa fa-mars,fa fa-mars-double,fa fa-mars-stroke,fa fa-mars-stroke-h,fa fa-mars-stroke-v,fa fa-mercury,fa fa-neuter,fa fa-transgender,fa fa-transgender-alt,fa fa-venus,fa fa-venus-double,fa fa-venus-mars，fa fa-align-center,fa fa-bold,fa fa-columns,fa fa-eraser,fa fa-file-text-o,fa fa-header,fa fa-list,fa fa-outdent,fa fa-repeat,fa fa-scissors,fa fa-table,fa fa-th-large,fa fa-unlink,fa fa-align-justify,fa fa-chain,fa fa-copy,fa fa-file,fa fa-files-o,fa fa-indent,fa fa-list-alt,fa fa-paperclip,fa fa-rotate-left,fa fa-strikethrough,fa fa-text-height,fa fa-text-height,fa fa-align-left,fa fa-chain-broken,fa fa-cut,fa fa-file-o,fa fa-floppy-o,fa fa-italic,fa fa-list-ol,fa fa-paragraph,fa fa-rotate-right,fa fa-subscript,fa fa-text-width,fa fa-underline,fa fa-align-right,fa fa-clipboard,fa fa-dedent,fa fa-file-text,fa fa-font,fa fa-link,fa fa-list-ul,fa fa-paste,fa fa-save,fa fa-superscript,fa fa-th,fa fa-undo,fa fa-angle-double-down,fa fa-angle-down,fa fa-arrow-circle-down,fa fa-arrow-down,fa fa-arrows,fa fa-caret-down,fa fa-caret-square-o-left,fa fa-chevron-circle-down,fa fa-chevron-down,fa fa-exchange,fa fa-hand-o-up,fa fa-long-arrow-up,fa fa-toggle-up,fa fa-angle-double-left,fa fa-angle-left,fa fa-arrow-circle-left,fa fa-arrow-circle-o-up,fa fa-arrow-left,fa fa-caret-left,fa fa-caret-square-o-right,fa fa-chevron-circle-left,fa fa-chevron-left,fa fa-hand-o-down,fa fa-long-arrow-down,fa fa-toggle-down,fa fa-angle-double-right,fa fa-angle-right,fa fa-arrow-circle-o-down,fa fa-arrow-circle-right,fa fa-arrow-right,fa fa-arrows-h,fa fa-caret-right,fa fa-caret-square-o-up,fa fa-chevron-circle-right,fa fa-chevron-right,fa fa-hand-o-left,fa fa-long-arrow-left,fa fa-toggle-left,fa fa-angle-double-up,fa fa-angle-up,fa fa-arrow-circle-o-left,fa fa-arrow-circle-up,fa fa-arrow-up,fa fa-arrows-v,fa fa-caret-square-o-down,fa fa-caret-up,fa fa-chevron-circle-up,fa fa-chevron-up,fa fa-hand-o-right,fa fa-long-arrow-right,fa fa-toggle-right,fa fa-arrows-alt,fa fa-expand,fa fa-pause,fa fa-play-circle,fa fa-step-forward,fa fa-youtube-play,fa fa-backward,fa fa-fast-backward,fa fa-pause-circle,fa fa-play-circle-o,fa fa-stop,fa fa-compress,fa fa-fast-forward,fa fa-pause-circle-o,fa fa-random,fa fa-stop-circle,fa fa-eject,fa fa-forward,fa fa-play,fa fa-step-backward,fa fa-stop-circle-o,fa fa-500px,fa fa-angellist,fa fa-bitbucket,fa fa-bluetooth,fa fa-cc-amex,fa fa-cc-mastercard,fa fa-chrome,fa fa-contao,fa fa-deviantart,fa fa-drupal,fa fa-facebook,fa fa-firefox，fa fa-forumbee，fa fa-gg，fa fa-github，fa fa-google，fa fa-gratipay，fa fa-instagram，fa fa-instagram,fa fa-linkedin,fa fa-meanpath,fa fa-odnoklassniki,fa fa-opera,fa fa-pied-piper,fa fa-pinterest-square,fa fa-rebel,fa fa-renren,fa fa-share-alt,fa fa-skyatlas,fa fa-soundcloud,fa fa-steam,fa fa-tencent-weibo,fa fa-tumblr-square,fa fa-usb,fa fa-vine,fa fa-weixin,fa fa-wordpress,fa fa-y-combinator-square,fa fa-yelp,fa fa-adn,fa fa-apple,fa fa-bitbucket-square,fa fa-bluetooth-b,fa fa-cc-diners-club,fa fa-cc-paypal,fa fa-codepen,fa fa-css3,fa fa-digg,fa fa-edge,fa fa-facebook-f,fa fa-flickr,fa fa-foursquare,fa fa-gg-circle,fa fa-github-alt,fa fa-google-plus,fa fa-hacker-news,fa fa-internet-explorer,fa fa-lastfm,fa fa-linkedin-square,fa fa-medium,fa fa-odnoklassniki-square,fa fa-optin-monster,fa fa-pied-piper-alt,fa fa-product-hunt,fa fa-reddit,fa fa-safari,fa fa-share-alt-square,fa fa-skype,fa fa-spotify,fa fa-steam-square,fa fa-trello,fa fa-twitch,fa fa-viacoin,fa fa-vk,fa fa-whatsapp,fa fa-xing,fa fa-yahoo,fa fa-youtube,fa fa-amazon,fa fa-behance,fa fa-bitcoin,fa fa-btc,fa fa-cc-discover,fa fa-cc-stripe,fa fa-codiepie,fa fa-dashcube,fa fa-dribbble,fa fa-empire,fa fa-facebook-official,fa fa-fonticons,fa fa-ge,fa fa-git,fa fa-github-square,fa fa-google-plus-square,fa fa-houzz,fa fa-ioxhost,fa fa-lastfm-square,fa fa-linux,fa fa-mixcloud,fa fa-opencart,fa fa-pagelines,fa fa-pinterest,fa fa-qq,fa fa-reddit-alien,fa fa-scribd,fa fa-shirtsinbulk,fa fa-slack,fa fa-stack-exchange,fa fa-stack-exchange,fa fa-stumbleupon,fa fa-tripadvisor,fa fa-twitter,fa fa-vimeo,fa fa-wechat,fa fa-wikipedia-w,fa fa-xing-square,fa fa-yc,fa fa-youtube-play,fa fa-android,fa fa-behance-square,fa fa-black-tie,fa fa-buysellads,fa fa-cc-jcb,fa fa-cc-visa,fa fa-connectdevelop,fa fa-delicious,fa fa-dropbox,fa fa-expeditedssl,fa fa-facebook-square,fa fa-fort-awesome,fa fa-get-pocket,fa fa-git-square,fa fa-gittip,fa fa-google-wallet,fa fa-html5,fa fa-joomla,fa fa-leanpub,fa fa-maxcdn,fa fa-modx,fa fa-openid,fa fa-paypal,fa fa-pinterest-p,fa fa-ra,fa fa-reddit-square,fa fa-sellsy,fa fa-simplybuilt,fa fa-slideshare,fa fa-stack-overflow,fa fa-stumbleupon-circle,fa fa-tumblr,fa fa-twitter-square,fa fa-vimeo-square,fa fa-weibo,fa fa-windows,fa fa-y-combinator,fa fa-yc-square,fa fa-youtube-square,fa fa-ambulance,fa fa-heartbeat,fa fa-stethoscope,fa fa-h-square,fa fa-hospital-o,fa fa-user-md,fa fa-heart,fa fa-medkit,fa fa-wheelchair,fa fa-heart-o,fa fa-plus-square";
            return text.Split(',');
        }
    }

}