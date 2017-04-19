/**
 * ueditor完整配置項
 * 可以在這裡配置整個編輯器的特性
 */
/**************************提示********************************
 * 所有被注釋的配置項均為UEditor預設值。
 * 修改預設配置請首先確保已經完全明確該參數的真實用途。
 * 主要有兩種修改方案，一種是取消此處注釋，然後修改成對應參數；另一種是在產生實體編輯器時傳入對應參數。
 * 當升級編輯器時，可直接使用舊版設定檔替換新版設定檔,不用擔心舊版設定檔中因缺少新功能所需的參數而導致腳本報錯。
 **************************提示********************************/

(function () {

    /**
     * 編輯器資源檔根路徑。它所表示的含義是：以編輯器產生實體頁面為當前路徑，指向編輯器資源檔（即dialog等資料夾）的路徑。
     * 鑒於很多同學在使用編輯器的時候出現的種種路徑問題，此處強烈建議大家使用"相對於網站根目錄的相對路徑"進行配置。
     * "相對於網站根目錄的相對路徑"也就是以斜杠開頭的形如"/myProject/ueditor/"這樣的路徑。
     * 如果網站中有多個不在同一層級的頁面需要產生實體編輯器，且引用了同一UEditor的時候，此處的URL可能不適用於每個頁面的編輯器。
     * 因此，UEditor提供了針對不同頁面的編輯器可單獨配置的根路徑，具體來說，在需要產生實體編輯器的頁面最頂部寫上如下代碼即可。當然，需要令此處的URL等於對應的配置。
     * window.UEDITOR_HOME_URL = "/xxxx/xxxx/";
     */
    window.UEDITOR_HOME_URL = "/Content/ueditor/";
    var URL = window.UEDITOR_HOME_URL || getUEBasePath();

    /**
     * 配置項主體。注意，此處所有涉及到路徑的配置別遺漏URL變數。
     */
    window.UEDITOR_CONFIG = {

        //為編輯器實例添加一個路徑，這個不能被注釋
        UEDITOR_HOME_URL: URL

        // 伺服器統一請求介面路徑
        , serverUrl: URL + "net/controller.ashx"

        //工具列上的所有的功能按鈕和下拉清單，可以在new編輯器的實例時選擇自己需要的從新定義
        , toolbars: [[
            'fullscreen', 'source', '|', 'undo', 'redo', '|',
            'bold', 'italic', 'underline', 'fontborder', 'strikethrough', 'superscript', 'subscript', 'blockquote', 'pasteplain', '|', 'forecolor', 'backcolor', 'insertorderedlist', 'insertunorderedlist', 'selectall', 'cleardoc', '|',
            'rowspacingtop', 'rowspacingbottom', 'lineheight', '|',
            'customstyle', 'paragraph', 'fontfamily', 'fontsize', '|',
            'directionalityltr', 'directionalityrtl', 'indent', '|',
            'justifyleft', 'justifycenter', 'justifyright', 'justifyjustify', '|', 'touppercase', 'tolowercase', '|',
            'link', 'unlink', 'anchor', '|', 'imagenone', 'imageleft', 'imageright', 'imagecenter', '|',
            'simpleupload', 'insertimage', 'emotion', 'scrawl', 'map', 'gmap', 'insertcode', 'pagebreak', 'template', 'background', '|',
            'horizontal', 'date', 'time', 'spechars', 'snapscreen', 'wordimage', '|',
            'inserttable', 'deletetable', 'insertparagraphbeforetable', 'insertrow', 'deleterow', 'insertcol', 'deletecol', 'mergecells', 'mergeright', 'mergedown', 'splittocells', 'splittorows', 'splittocols', 'charts', '|',
            'preview', 'searchreplace', 'drafts'
        ]]
        //當滑鼠放在工具列上時顯示的tooltip提示,留空支援自動多語言配置，否則以配置值為准
        //,labelMap:{
        //    'anchor':'', 'undo':''
        //}

        //語言配置項,預設是zh-cn。有需要的話也可以使用如下這樣的方式來自動多語言切換，當然，前提條件是lang資料夾下存在對應的語言檔：
        //lang值也可以通過自動獲取 (navigator.language||navigator.browserLanguage ||navigator.userLanguage).toLowerCase()
        //,lang:"zh-cn"
        //,langPath:URL +"lang/"

        //主題配置項,預設是default。有需要的話也可以使用如下這樣的方式來自動多主題切換，當然，前提條件是themes資料夾下存在對應的主題檔：
        //現有如下皮膚:default
        //,theme:'default'
        //,themePath:URL +"themes/"

        //,zIndex : 900     //編輯器層級的基數,默認是900

        //針對getAllHtml方法，會在對應的head標籤中增加該編碼設置。
        //,charset:"utf-8"

        //若產生實體編輯器的頁面手動修改的domain，此處需要設置為true
        //,customDomain:false

        //常用配置專案
        //,isShow : true    //預設顯示編輯器

        //,textarea:'editorValue' // 提交表單時，伺服器獲取編輯器提交內容的所用的參數，多實例時可以給容器name屬性，會將name給定的值最為每個實例的鍵值，不用每次產生實體的時候都設置這個值

        //,initialContent:'歡迎使用ueditor!'    //初始化編輯器的內容,也可以通過textarea/script給值，看官網例子

        //,autoClearinitialContent:true //是否自動清除編輯器初始內容，注意：如果focus屬性設置為true,這個也為真，那麼編輯器一上來就會觸發導致初始化的內容看不到了

        //,focus:false //初始化時，是否讓編輯器獲得焦點true或false

        //如果自訂，最好給p標籤如下的行高，要不輸入中文時，會有跳動感
        //,initialStyle:'p{line-height:1em}'//編輯器層級的基數,可以用來改變字體等

        //,iframeCssUrl: URL + '/themes/iframe.css' //給編輯器內部引入一個css檔

        //indentValue
        //首行縮進距離,默認是2em
        //,indentValue:'2em'

        //,initialFrameWidth:1000  //初始化編輯器寬度,默認1000
        //,initialFrameHeight:320  //初始化編輯器高度,默認320

        //,readonly : false //編輯器初始化結束後,編輯區域是否是唯讀的，默認是false

        //,autoClearEmptyNode : true //getContent時，是否刪除空的inlineElement節點（包括嵌套的情況）

        //啟用自動保存
        , enableAutoSave: false
        //自動保存間隔時間， 單位ms
        //,saveInterval: 500

        //,fullscreen : false //是否開啟初始化時即全屏，默認關閉

        //,imagePopup:true      //圖片操作的浮層開關，預設打開

        //,autoSyncData:true //自動同步編輯器要提交的資料
        //,emotionLocalization:false //是否開啟表情當地語系化，默認關閉。若要開啟請確保emotion資料夾下包含官網提供的images表情資料夾

        //粘貼只保留標籤，去除標籤所有屬性
        //,retainOnlyLabelPasted: false

        //,pasteplain:false  //是否默認為純文字粘貼。false為不使用純文字粘貼，true為使用純文字粘貼
        //純文字粘貼模式下的過濾規則
        //'filterTxtRules' : function(){
        //    function transP(node){
        //        node.tagName = 'p';
        //        node.setStyle();
        //    }
        //    return {
        //        //直接刪除及其位元組點內容
        //        '-' : 'script style object iframe embed input select',
        //        'p': {$:{}},
        //        'br':{$:{}},
        //        'div':{'$':{}},
        //        'li':{'$':{}},
        //        'caption':transP,
        //        'th':transP,
        //        'tr':transP,
        //        'h1':transP,'h2':transP,'h3':transP,'h4':transP,'h5':transP,'h6':transP,
        //        'td':function(node){
        //            //沒有內容的td直接刪掉
        //            var txt = !!node.innerText();
        //            if(txt){
        //                node.parentNode.insertAfter(UE.uNode.createText(' &nbsp; &nbsp;'),node);
        //            }
        //            node.parentNode.removeChild(node,node.innerText())
        //        }
        //    }
        //}()

        //,allHtmlEnabled:false //提交到後臺的資料是否包含整個html字串

        //insertorderedlist
        //有序列表的下拉配置,值留空時支援多語言自動識別，若配置值，則以此值為准
        //,'insertorderedlist':{
        //      //自定的樣式
        //        'num':'1,2,3...',
        //        'num1':'1),2),3)...',
        //        'num2':'(1),(2),(3)...',
        //        'cn':'一,二,三....',
        //        'cn1':'一),二),三)....',
        //        'cn2':'(一),(二),(三)....',
        //     //系統自帶
        //     'decimal' : '' ,         //'1,2,3...'
        //     'lower-alpha' : '' ,    // 'a,b,c...'
        //     'lower-roman' : '' ,    //'i,ii,iii...'
        //     'upper-alpha' : '' , lang   //'A,B,C'
        //     'upper-roman' : ''      //'I,II,III...'
        //}

        //insertunorderedlist
        //無序列表的下拉配置，值留空時支援多語言自動識別，若配置值，則以此值為准
        //,insertunorderedlist : { //自定的樣式
        //    'dash' :'— 破折號', //-破折號
        //    'dot':' 。 小圓圈', //系統自帶
        //    'circle' : '',  // '○ 小圓圈'
        //    'disc' : '',    // '● 小圓點'
        //    'square' : ''   //'■ 小方塊'
        //}
        //,listDefaultPaddingLeft : '30'//默認的左邊縮進的基數倍
        //,listiconpath : 'http://bs.baidu.com/listicon/'//自訂標號的路徑
        //,maxListLevel : 3 //限制可以tab的級數, 設置-1為不限制

        //,autoTransWordToList:false  //禁止word中粘貼進來的列表自動變成列表標籤

        //fontfamily
        //字體設置 label留空支援多語言自動切換，若配置，則以配置值為准
        //,'fontfamily':[
        //    { label:'',name:'songti',val:'宋體,SimSun'},
        //    { label:'',name:'kaiti',val:'楷體,楷體_GB2312, SimKai'},
        //    { label:'',name:'yahei',val:'微軟雅黑,Microsoft YaHei'},
        //    { label:'',name:'heiti',val:'黑體, SimHei'},
        //    { label:'',name:'lishu',val:'隸書, SimLi'},
        //    { label:'',name:'andaleMono',val:'andale mono'},
        //    { label:'',name:'arial',val:'arial, helvetica,sans-serif'},
        //    { label:'',name:'arialBlack',val:'arial black,avant garde'},
        //    { label:'',name:'comicSansMs',val:'comic sans ms'},
        //    { label:'',name:'impact',val:'impact,chicago'},
        //    { label:'',name:'timesNewRoman',val:'times new roman'}
        //]

        //fontsize
        //字型大小
        //,'fontsize':[10, 11, 12, 14, 16, 18, 20, 24, 36]

        //paragraph
        //段落格式 值留空時支援多語言自動識別，若配置，則以配置值為准
        //,'paragraph':{'p':'', 'h1':'', 'h2':'', 'h3':'', 'h4':'', 'h5':'', 'h6':''}

        //rowspacingtop
        //段間距 值和顯示的名字相同
        //,'rowspacingtop':['5', '10', '15', '20', '25']

        //rowspacingBottom
        //段間距 值和顯示的名字相同
        //,'rowspacingbottom':['5', '10', '15', '20', '25']

        //lineheight
        //行內間距 值和顯示的名字相同
        //,'lineheight':['1', '1.5','1.75','2', '3', '4', '5']

        //customstyle
        //自訂樣式，不支援國際化，此處配置值即可最後顯示值
        //block的元素是依據設置段落的邏輯設置的，inline的元素依據BIU的邏輯設置
        //儘量使用一些常用的標籤
        //參數說明
        //tag 使用的標籤名字
        //label 顯示的名字也是用來標識不同類型的識別字，注意這個值每個要不同，
        //style 添加的樣式
        //每一個物件就是一個自訂的樣式
        //,'customstyle':[
        //    {tag:'h1', name:'tc', label:'', style:'border-bottom:#ccc 2px solid;padding:0 4px 0 0;text-align:center;margin:0 0 20px 0;'},
        //    {tag:'h1', name:'tl',label:'', style:'border-bottom:#ccc 2px solid;padding:0 4px 0 0;margin:0 0 10px 0;'},
        //    {tag:'span',name:'im', label:'', style:'font-style:italic;font-weight:bold'},
        //    {tag:'span',name:'hi', label:'', style:'font-style:italic;font-weight:bold;color:rgb(51, 153, 204)'}
        //]

        //打開右鍵功能表功能
        //,enableContextMenu: true
        //右鍵功能表的內容，可以參考plugins/contextmenu.js裡邊的預設功能表的例子，label留空支持國際化，否則以此配置為准
        //,contextMenu:[
        //    {
        //        label:'',       //顯示的名稱
        //        cmdName:'selectall',//執行的command命令，當點擊這個右鍵功能表時
        //        //exec可選，有了exec就會在點擊時執行這個function，優先順序高於cmdName
        //        exec:function () {
        //            //this是當前編輯器的實例
        //            //this.ui._dialogs['inserttableDialog'].open();
        //        }
        //    }
        //]

        //快顯功能表
        //,shortcutMenu:["fontfamily", "fontsize", "bold", "italic", "underline", "forecolor", "backcolor", "insertorderedlist", "insertunorderedlist"]

        //elementPathEnabled
        //是否啟用元素路徑，預設是顯示
        //,elementPathEnabled : true

        //wordCount
        //,wordCount:true          //是否開啟字數統計
        //,maximumWords:10000       //允許的最大字元數
        //字數統計提示，{#count}代表當前字數，{#leave}代表還可以輸入多少字元數,留空支援多語言自動切換，否則按此配置顯示
        //,wordCountMsg:''   //當前已輸入 {#count} 個字元，您還可以輸入{#leave} 個字元
        //超出字數限制提示  留空支援多語言自動切換，否則按此配置顯示
        //,wordOverFlowMsg:''    //<span style="color:red;">你輸入的字元個數已經超出最大允許值，伺服器可能會拒絕保存！</span>

        //tab
        //點擊tab鍵時移動的距離,tabSize倍數，tabNode什麼字元做為單位
        //,tabSize:4
        //,tabNode:'&nbsp;'

        //removeFormat
        //清除格式時可以刪除的標籤和屬性
        //removeForamtTags標籤
        //,removeFormatTags:'b,big,code,del,dfn,em,font,i,ins,kbd,q,samp,small,span,strike,strong,sub,sup,tt,u,var'
        //removeFormatAttributes屬性
        //,removeFormatAttributes:'class,style,lang,width,height,align,hspace,valign'

        //undo
        //可以最多回退的次數,默認20
        //,maxUndoCount:20
        //當輸入的字元數超過該值時，保存一次現場
        //,maxInputCount:1

        //autoHeightEnabled
        // 是否自動長高,默認true
        , autoHeightEnabled: false

        //scaleEnabled
        //是否可以拉伸長高,默認true(當開啟時，自動長高失效)
        //,scaleEnabled:false
        //,minFrameWidth:800    //編輯器拖動時最小寬度,默認800
        //,minFrameHeight:220  //編輯器拖動時最小高度,默認220

        //autoFloatEnabled
        //是否保持toolbar的位置不動,默認true
        //,autoFloatEnabled:false
        //浮動時工具列距離流覽器頂部的高度，用於某些具有固定頭部的頁面
        //,topOffset:30
        //編輯器底部距離工具列高度(如果參數大於等於編輯器高度，則設置無效)
        //,toolbarTopOffset:400

        //pageBreakTag
        //分頁識別字,默認是_ueditor_page_break_tag_
        //,pageBreakTag:'_ueditor_page_break_tag_'

        //autotypeset
        //自動排版參數
        //,autotypeset: {
        //    mergeEmptyline: true,           //合併空行
        //    removeClass: true,              //去掉冗餘的class
        //    removeEmptyline: false,         //去掉空行
        //    textAlign:"left",               //段落的排版方式，可以是 left,right,center,justify 去掉這個屬性工作表示不執行排版
        //    imageBlockLine: 'center',       //圖片的浮動方式，獨佔一行劇中,左右浮動，默認: center,left,right,none 去掉這個屬性工作表示不執行排版
        //    pasteFilter: false,             //根據規則過濾沒事粘貼進來的內容
        //    clearFontSize: false,           //去掉所有的內嵌字型大小，使用編輯器默認的字型大小
        //    clearFontFamily: false,         //去掉所有的內嵌字體，使用編輯器預設的字體
        //    removeEmptyNode: false,         // 去掉空節點
        //    //可以去掉的標籤
        //    removeTagNames: {標籤名字:1},
        //    indent: false,                  // 行首縮進
        //    indentValue : '2em',            //行首縮進的大小
        //    bdc2sb: false,
        //    tobdc: false
        //}

        //tableDragable
        //表格是否可以拖拽
        //,tableDragable: true

        //,disabledTableInTable:true  //禁止表格嵌套

        //sourceEditor
        //源碼的查看方式,codemirror 是代碼高亮，textarea是文字方塊,默認是codemirror
        //注意默認codemirror只能在ie8+和非ie中使用
        //,sourceEditor:"codemirror"
        //如果sourceEditor是codemirror，還用配置一下兩個參數
        //codeMirrorJsUrl js載入的路徑，預設是 URL + "third-party/codemirror/codemirror.js"
        //,codeMirrorJsUrl:URL + "third-party/codemirror/codemirror.js"
        //codeMirrorCssUrl css載入的路徑，預設是 URL + "third-party/codemirror/codemirror.css"
        //,codeMirrorCssUrl:URL + "third-party/codemirror/codemirror.css"
        //編輯器初始化完成後是否進入源碼模式，預設為否。
        //,sourceEditorFirst:false

        //iframeUrlMap
        //dialog內容的路徑 ～會被替換成URL,垓屬性一旦打開，將覆蓋所有的dialog的預設路徑
        //,iframeUrlMap:{
        //    'anchor':'~/dialogs/anchor/anchor.html',
        //}

        //webAppKey 百度應用的APIkey，每個站長必須首先去百度官網註冊一個key後方能正常使用app功能，註冊介紹，http://app.baidu.com/static/cms/getapikey.html
        //, webAppKey: ""
    };

    function getUEBasePath(docUrl, confUrl) {

        return getBasePath(docUrl || self.document.URL || self.location.href, confUrl || getConfigFilePath());

    }

    function getConfigFilePath() {

        var configPath = document.getElementsByTagName('script');

        return configPath[configPath.length - 1].src;

    }

    function getBasePath(docUrl, confUrl) {

        var basePath = confUrl;


        if (/^(\/|\\\\)/.test(confUrl)) {

            basePath = /^.+?\w(\/|\\\\)/.exec(docUrl)[0] + confUrl.replace(/^(\/|\\\\)/, '');

        } else if (!/^[a-z]+:/i.test(confUrl)) {

            docUrl = docUrl.split("#")[0].split("?")[0].replace(/[^\\\/]+$/, '');

            basePath = docUrl + "" + confUrl;

        }

        return optimizationPath(basePath);

    }

    function optimizationPath(path) {

        var protocol = /^[a-z]+:\/\//.exec(path)[0],
            tmp = null,
            res = [];

        path = path.replace(protocol, "").split("?")[0].split("#")[0];

        path = path.replace(/\\/g, '/').split(/\//);

        path[path.length - 1] = "";

        while (path.length) {

            if ((tmp = path.shift()) === "..") {
                res.pop();
            } else if (tmp !== ".") {
                res.push(tmp);
            }

        }

        return protocol + res.join("/");

    }

    window.UE = {
        getUEBasePath: getUEBasePath
    };

})();
