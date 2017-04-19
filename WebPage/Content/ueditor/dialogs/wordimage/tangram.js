// Copyright (c) 2009, Baidu Inc. All rights reserved.
// 
// Licensed under the BSD License
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http:// tangram.baidu.com/license.html
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS-IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
/**
* @namespace T Tangram七巧板
* @name T
* @version 1.6.0
*/

/**
 * 聲明baidu包
 * @author: allstar, erik, meizz, berg
 */
var T,
    baidu = T = baidu || { version: "1.5.0" };
baidu.guid = "$BAIDU$";
baidu.$$ = window[baidu.guid] = window[baidu.guid] || { global: {} };

/**
 * 使用flash資源封裝的一些功能
 * @namespace baidu.flash
 */
baidu.flash = baidu.flash || {};

/**
 * 操作dom的方法
 * @namespace baidu.dom 
 */
baidu.dom = baidu.dom || {};


/**
 * 從文檔中獲取指定的DOM元素
 * @name baidu.dom.g
 * @function
 * @grammar baidu.dom.g(id)
 * @param {string|HTMLElement} id 元素的id或DOM元素.
 * @shortcut g,T.G
 * @meta standard
 * @see baidu.dom.q
 *
 * @return {HTMLElement|null} 獲取的元素，查找不到時返回null,如果參數不合法，直接返回參數.
 */
baidu.dom.g = function (id) {
    if (!id) return null;
    if ('string' == typeof id || id instanceof String) {
        return document.getElementById(id);
    } else if (id.nodeName && (id.nodeType == 1 || id.nodeType == 9)) {
        return id;
    }
    return null;
};
baidu.g = baidu.G = baidu.dom.g;


/**
 * 運算元組的方法
 * @namespace baidu.array
 */

baidu.array = baidu.array || {};


/**
 * 遍歷陣列中所有元素
 * @name baidu.array.each
 * @function
 * @grammar baidu.array.each(source, iterator[, thisObject])
 * @param {Array} source 需要遍歷的陣列
 * @param {Function} iterator 對每個陣列元素進行調用的函數，該函數有兩個參數，第一個為陣列元素，第二個為陣列索引值，function (item, index)。
 * @param {Object} [thisObject] 函式呼叫時的this指標，如果沒有此參數，預設是當前遍歷的陣列
 * @remark
 * each方法不支援對Object的遍歷,對Object的遍歷使用baidu.object.each 。
 * @shortcut each
 * @meta standard
 *             
 * @returns {Array} 遍歷的陣列
 */

baidu.each = baidu.array.forEach = baidu.array.each = function (source, iterator, thisObject) {
    var returnValue, item, i, len = source.length;

    if ('function' == typeof iterator) {
        for (i = 0; i < len; i++) {
            item = source[i];
            returnValue = iterator.call(thisObject || source, item, i);

            if (returnValue === false) {
                break;
            }
        }
    }
    return source;
};

/**
 * 對語言層面的封裝，包括類型判斷、模組擴展、繼承基類以及物件自訂事件的支援。
 * @namespace baidu.lang
 */
baidu.lang = baidu.lang || {};


/**
 * 判斷目標參數是否為function或Function實例
 * @name baidu.lang.isFunction
 * @function
 * @grammar baidu.lang.isFunction(source)
 * @param {Any} source 目標參數
 * @version 1.2
 * @see baidu.lang.isString,baidu.lang.isObject,baidu.lang.isNumber,baidu.lang.isArray,baidu.lang.isElement,baidu.lang.isBoolean,baidu.lang.isDate
 * @meta standard
 * @returns {boolean} 類型判斷結果
 */
baidu.lang.isFunction = function (source) {
    return '[object Function]' == Object.prototype.toString.call(source);
};

/**
 * 判斷目標參數是否string類型或String物件
 * @name baidu.lang.isString
 * @function
 * @grammar baidu.lang.isString(source)
 * @param {Any} source 目標參數
 * @shortcut isString
 * @meta standard
 * @see baidu.lang.isObject,baidu.lang.isNumber,baidu.lang.isArray,baidu.lang.isElement,baidu.lang.isBoolean,baidu.lang.isDate
 *             
 * @returns {boolean} 類型判斷結果
 */
baidu.lang.isString = function (source) {
    return '[object String]' == Object.prototype.toString.call(source);
};
baidu.isString = baidu.lang.isString;


/**
 * 判斷流覽器類型和特性的屬性
 * @namespace baidu.browser
 */
baidu.browser = baidu.browser || {};


/**
 * 判斷是否為opera流覽器
 * @property opera opera版本號
 * @grammar baidu.browser.opera
 * @meta standard
 * @see baidu.browser.ie,baidu.browser.firefox,baidu.browser.safari,baidu.browser.chrome
 * @returns {Number} opera版本號
 */

/**
 * opera 從10開始不是用opera後面的字串進行版本的判斷
 * 在Browser identification最後添加Version + 數位進行版本標識
 * opera後面的數字保持在9.80不變
 */
baidu.browser.opera = /opera(\/| )(\d+(\.\d+)?)(.+?(version\/(\d+(\.\d+)?)))?/i.test(navigator.userAgent) ? +(RegExp["\x246"] || RegExp["\x242"]) : undefined;


/**
 * 在目標元素的指定位置插入HTML代碼
 * @name baidu.dom.insertHTML
 * @function
 * @grammar baidu.dom.insertHTML(element, position, html)
 * @param {HTMLElement|string} element 目標元素或目標元素的id
 * @param {string} position 插入html的位置資訊，取值為beforeBegin,afterBegin,beforeEnd,afterEnd
 * @param {string} html 要插入的html
 * @remark
 * 
 * 對於position參數，大小寫不敏感<br>
 * 參數的意思：beforeBegin&lt;span&gt;afterBegin   this is span! beforeEnd&lt;/span&gt; afterEnd <br />
 * 此外，如果使用本函數插入帶有script標籤的HTML字串，script標籤對應的腳本將不會被執行。
 * 
 * @shortcut insertHTML
 * @meta standard
 *             
 * @returns {HTMLElement} 目標元素
 */
baidu.dom.insertHTML = function (element, position, html) {
    element = baidu.dom.g(element);
    var range, begin;
    if (element.insertAdjacentHTML && !baidu.browser.opera) {
        element.insertAdjacentHTML(position, html);
    } else {
        range = element.ownerDocument.createRange();
        position = position.toUpperCase();
        if (position == 'AFTERBEGIN' || position == 'BEFOREEND') {
            range.selectNodeContents(element);
            range.collapse(position == 'AFTERBEGIN');
        } else {
            begin = position == 'BEFOREBEGIN';
            range[begin ? 'setStartBefore' : 'setEndAfter'](element);
            range.collapse(begin);
        }
        range.insertNode(range.createContextualFragment(html));
    }
    return element;
};

baidu.insertHTML = baidu.dom.insertHTML;

/**
 * 操作flash物件的方法，包括創建flash物件、獲取flash物件以及判斷flash外掛程式的版本號
 * @namespace baidu.swf
 */
baidu.swf = baidu.swf || {};


/**
 * 流覽器支援的flash外掛程式版本
 * @property version 流覽器支援的flash外掛程式版本
 * @grammar baidu.swf.version
 * @return {String} 版本號
 * @meta standard
 */
baidu.swf.version = (function () {
    var n = navigator;
    if (n.plugins && n.mimeTypes.length) {
        var plugin = n.plugins["Shockwave Flash"];
        if (plugin && plugin.description) {
            return plugin.description
                    .replace(/([a-zA-Z]|\s)+/, "")
                    .replace(/(\s)+r/, ".") + ".0";
        }
    } else if (window.ActiveXObject && !window.opera) {
        for (var i = 12; i >= 2; i--) {
            try {
                var c = new ActiveXObject('ShockwaveFlash.ShockwaveFlash.' + i);
                if (c) {
                    var version = c.GetVariable("$version");
                    return version.replace(/WIN/g, '').replace(/,/g, '.');
                }
            } catch (e) { }
        }
    }
})();

/**
 * 操作字串的方法
 * @namespace baidu.string
 */
baidu.string = baidu.string || {};


/**
 * 對目標字串進行html編碼
 * @name baidu.string.encodeHTML
 * @function
 * @grammar baidu.string.encodeHTML(source)
 * @param {string} source 目標字串
 * @remark
 * 編碼字元有5個：&<>"'
 * @shortcut encodeHTML
 * @meta standard
 * @see baidu.string.decodeHTML
 *             
 * @returns {string} html編碼後的字串
 */
baidu.string.encodeHTML = function (source) {
    return String(source)
                .replace(/&/g, '&amp;')
                .replace(/</g, '&lt;')
                .replace(/>/g, '&gt;')
                .replace(/"/g, "&quot;")
                .replace(/'/g, "&#39;");
};

baidu.encodeHTML = baidu.string.encodeHTML;

/**
 * 創建flash物件的html字串
 * @name baidu.swf.createHTML
 * @function
 * @grammar baidu.swf.createHTML(options)
 * 
 * @param {Object} 	options 					創建flash的選項參數
 * @param {string} 	options.id 					要創建的flash的標識
 * @param {string} 	options.url 				flash文件的url
 * @param {String} 	options.errorMessage 		未安裝flash player或flash player版本號過低時的提示
 * @param {string} 	options.ver 				最低需要的flash player版本號
 * @param {string} 	options.width 				flash的寬度
 * @param {string} 	options.height 				flash的高度
 * @param {string} 	options.align 				flash的對齊方式，允許值：middle/left/right/top/bottom
 * @param {string} 	options.base 				設置用於解析swf檔中的所有相對路徑語句的基本目錄或URL
 * @param {string} 	options.bgcolor 			swf檔的背景色
 * @param {string} 	options.salign 				設置縮放的swf檔在由width和height設置定義的區域內的位置。允許值：l/r/t/b/tl/tr/bl/br
 * @param {boolean} options.menu 				是否顯示右鍵功能表，允許值：true/false
 * @param {boolean} options.loop 				播放到最後一幀時是否重新播放，允許值： true/false
 * @param {boolean} options.play 				flash是否在流覽器載入時就開始播放。允許值：true/false
 * @param {string} 	options.quality 			設置flash播放的畫質，允許值：low/medium/high/autolow/autohigh/best
 * @param {string} 	options.scale 				設置flash內容如何縮放來適應設置的寬高。允許值：showall/noborder/exactfit
 * @param {string} 	options.wmode 				設置flash的顯示模式。允許值：window/opaque/transparent
 * @param {string} 	options.allowscriptaccess 	設置flash與頁面的通信許可權。允許值：always/never/sameDomain
 * @param {string} 	options.allownetworking 	設置swf檔中允許使用的網路API。允許值：all/internal/none
 * @param {boolean} options.allowfullscreen 	是否允許flash全屏。允許值：true/false
 * @param {boolean} options.seamlesstabbing 	允許設置執行無縫跳格，從而使用戶能跳出flash應用程式。該參數只能在安裝Flash7及更高版本的Windows中使用。允許值：true/false
 * @param {boolean} options.devicefont 			設置靜態文本物件是否以設備字體呈現。允許值：true/false
 * @param {boolean} options.swliveconnect 		第一次載入flash時流覽器是否應啟動Java。允許值：true/false
 * @param {Object} 	options.vars 				要傳遞給flash的參數，支援JSON或string類型。
 * 
 * @see baidu.swf.create
 * @meta standard
 * @returns {string} flash物件的html字串
 */
baidu.swf.createHTML = function (options) {
    options = options || {};
    var version = baidu.swf.version,
        needVersion = options['ver'] || '6.0.0',
        vUnit1, vUnit2, i, k, len, item, tmpOpt = {},
        encodeHTML = baidu.string.encodeHTML;
    for (k in options) {
        tmpOpt[k] = options[k];
    }
    options = tmpOpt;
    if (version) {
        version = version.split('.');
        needVersion = needVersion.split('.');
        for (i = 0; i < 3; i++) {
            vUnit1 = parseInt(version[i], 10);
            vUnit2 = parseInt(needVersion[i], 10);
            if (vUnit2 < vUnit1) {
                break;
            } else if (vUnit2 > vUnit1) {
                return '';
            }
        }
    } else {
        return '';
    }

    var vars = options['vars'],
        objProperties = ['classid', 'codebase', 'id', 'width', 'height', 'align'];
    options['align'] = options['align'] || 'middle';
    options['classid'] = 'clsid:d27cdb6e-ae6d-11cf-96b8-444553540000';
    options['codebase'] = 'http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,0,0';
    options['movie'] = options['url'] || '';
    delete options['vars'];
    delete options['url'];
    if ('string' == typeof vars) {
        options['flashvars'] = vars;
    } else {
        var fvars = [];
        for (k in vars) {
            item = vars[k];
            fvars.push(k + "=" + encodeURIComponent(item));
        }
        options['flashvars'] = fvars.join('&');
    }
    var str = ['<object '];
    for (i = 0, len = objProperties.length; i < len; i++) {
        item = objProperties[i];
        str.push(' ', item, '="', encodeHTML(options[item]), '"');
    }
    str.push('>');
    var params = {
        'wmode': 1,
        'scale': 1,
        'quality': 1,
        'play': 1,
        'loop': 1,
        'menu': 1,
        'salign': 1,
        'bgcolor': 1,
        'base': 1,
        'allowscriptaccess': 1,
        'allownetworking': 1,
        'allowfullscreen': 1,
        'seamlesstabbing': 1,
        'devicefont': 1,
        'swliveconnect': 1,
        'flashvars': 1,
        'movie': 1
    };

    for (k in options) {
        item = options[k];
        k = k.toLowerCase();
        if (params[k] && (item || item === false || item === 0)) {
            str.push('<param name="' + k + '" value="' + encodeHTML(item) + '" />');
        }
    }
    options['src'] = options['movie'];
    options['name'] = options['id'];
    delete options['id'];
    delete options['movie'];
    delete options['classid'];
    delete options['codebase'];
    options['type'] = 'application/x-shockwave-flash';
    options['pluginspage'] = 'http://www.macromedia.com/go/getflashplayer';
    str.push('<embed');
    var salign;
    for (k in options) {
        item = options[k];
        if (item || item === false || item === 0) {
            if ((new RegExp("^salign\x24", "i")).test(k)) {
                salign = item;
                continue;
            }

            str.push(' ', k, '="', encodeHTML(item), '"');
        }
    }

    if (salign) {
        str.push(' salign="', encodeHTML(salign), '"');
    }
    str.push('></embed></object>');

    return str.join('');
};


/**
 * 在頁面中創建一個flash物件
 * @name baidu.swf.create
 * @function
 * @grammar baidu.swf.create(options[, container])
 * 
 * @param {Object} 	options 					創建flash的選項參數
 * @param {string} 	options.id 					要創建的flash的標識
 * @param {string} 	options.url 				flash文件的url
 * @param {String} 	options.errorMessage 		未安裝flash player或flash player版本號過低時的提示
 * @param {string} 	options.ver 				最低需要的flash player版本號
 * @param {string} 	options.width 				flash的寬度
 * @param {string} 	options.height 				flash的高度
 * @param {string} 	options.align 				flash的對齊方式，允許值：middle/left/right/top/bottom
 * @param {string} 	options.base 				設置用於解析swf檔中的所有相對路徑語句的基本目錄或URL
 * @param {string} 	options.bgcolor 			swf檔的背景色
 * @param {string} 	options.salign 				設置縮放的swf檔在由width和height設置定義的區域內的位置。允許值：l/r/t/b/tl/tr/bl/br
 * @param {boolean} options.menu 				是否顯示右鍵功能表，允許值：true/false
 * @param {boolean} options.loop 				播放到最後一幀時是否重新播放，允許值： true/false
 * @param {boolean} options.play 				flash是否在流覽器載入時就開始播放。允許值：true/false
 * @param {string} 	options.quality 			設置flash播放的畫質，允許值：low/medium/high/autolow/autohigh/best
 * @param {string} 	options.scale 				設置flash內容如何縮放來適應設置的寬高。允許值：showall/noborder/exactfit
 * @param {string} 	options.wmode 				設置flash的顯示模式。允許值：window/opaque/transparent
 * @param {string} 	options.allowscriptaccess 	設置flash與頁面的通信許可權。允許值：always/never/sameDomain
 * @param {string} 	options.allownetworking 	設置swf檔中允許使用的網路API。允許值：all/internal/none
 * @param {boolean} options.allowfullscreen 	是否允許flash全屏。允許值：true/false
 * @param {boolean} options.seamlesstabbing 	允許設置執行無縫跳格，從而使用戶能跳出flash應用程式。該參數只能在安裝Flash7及更高版本的Windows中使用。允許值：true/false
 * @param {boolean} options.devicefont 			設置靜態文本物件是否以設備字體呈現。允許值：true/false
 * @param {boolean} options.swliveconnect 		第一次載入flash時流覽器是否應啟動Java。允許值：true/false
 * @param {Object} 	options.vars 				要傳遞給flash的參數，支援JSON或string類型。
 * 
 * @param {HTMLElement|string} [container] 		flash物件的父容器元素，不傳遞該參數時在當前代碼位置創建flash物件。
 * @meta standard
 * @see baidu.swf.createHTML,baidu.swf.getMovie
 */
baidu.swf.create = function (options, target) {
    options = options || {};
    var html = baidu.swf.createHTML(options)
               || options['errorMessage']
               || '';

    if (target && 'string' == typeof target) {
        target = document.getElementById(target);
    }
    baidu.dom.insertHTML(target || document.body, 'beforeEnd', html);
};
/**
 * 判斷是否為ie流覽器
 * @name baidu.browser.ie
 * @field
 * @grammar baidu.browser.ie
 * @returns {Number} IE版本號
 */
baidu.browser.ie = baidu.ie = /msie (\d+\.\d+)/i.test(navigator.userAgent) ? (document.documentMode || +RegExp['\x241']) : undefined;

/**
 * 移除數組中的項
 * @name baidu.array.remove
 * @function
 * @grammar baidu.array.remove(source, match)
 * @param {Array} source 需要移除項的陣列
 * @param {Any} match 要移除的項
 * @meta standard
 * @see baidu.array.removeAt
 *             
 * @returns {Array} 移除後的陣列
 */
baidu.array.remove = function (source, match) {
    var len = source.length;

    while (len--) {
        if (len in source && source[len] === match) {
            source.splice(len, 1);
        }
    }
    return source;
};

/**
 * 判斷目標參數是否Array物件
 * @name baidu.lang.isArray
 * @function
 * @grammar baidu.lang.isArray(source)
 * @param {Any} source 目標參數
 * @meta standard
 * @see baidu.lang.isString,baidu.lang.isObject,baidu.lang.isNumber,baidu.lang.isElement,baidu.lang.isBoolean,baidu.lang.isDate
 *             
 * @returns {boolean} 類型判斷結果
 */
baidu.lang.isArray = function (source) {
    return '[object Array]' == Object.prototype.toString.call(source);
};



/**
 * 將一個變數轉換成array
 * @name baidu.lang.toArray
 * @function
 * @grammar baidu.lang.toArray(source)
 * @param {mix} source 需要轉換成array的變數
 * @version 1.3
 * @meta standard
 * @returns {array} 轉換後的array
 */
baidu.lang.toArray = function (source) {
    if (source === null || source === undefined)
        return [];
    if (baidu.lang.isArray(source))
        return source;
    if (typeof source.length !== 'number' || typeof source === 'string' || baidu.lang.isFunction(source)) {
        return [source];
    }
    if (source.item) {
        var l = source.length, array = new Array(l);
        while (l--)
            array[l] = source[l];
        return array;
    }

    return [].slice.call(source);
};

/**
 * 獲得flash物件的實例
 * @name baidu.swf.getMovie
 * @function
 * @grammar baidu.swf.getMovie(name)
 * @param {string} name flash對象的名稱
 * @see baidu.swf.create
 * @meta standard
 * @returns {HTMLElement} flash物件的實例
 */
baidu.swf.getMovie = function (name) {
    var movie = document[name], ret;
    return baidu.browser.ie == 9 ?
    	movie && movie.length ?
    		(ret = baidu.array.remove(baidu.lang.toArray(movie), function (item) {
    		    return item.tagName.toLowerCase() != "embed";
    		})).length == 1 ? ret[0] : ret
    		: movie
    	: movie || window[name];
};


baidu.flash._Base = (function () {

    var prefix = 'bd__flash__';

    /**
     * 創建一個隨機的字串
     * @private
     * @return {String}
     */
    function _createString() {
        return prefix + Math.floor(Math.random() * 2147483648).toString(36);
    };

    /**
     * 檢查flash狀態
     * @private
     * @param {Object} target flash對象
     * @return {Boolean}
     */
    function _checkReady(target) {
        if (typeof target !== 'undefined' && typeof target.flashInit !== 'undefined' && target.flashInit()) {
            return true;
        } else {
            return false;
        }
    };

    /**
     * 調用之前進行壓棧的函數
     * @private
     * @param {Array} callQueue 調用佇列
     * @param {Object} target flash對象
     * @return {Null}
     */
    function _callFn(callQueue, target) {
        var result = null;

        callQueue = callQueue.reverse();
        baidu.each(callQueue, function (item) {
            result = target.call(item.fnName, item.params);
            item.callBack(result);
        });
    };

    /**
     * 為傳入的匿名函數創建函數名
     * @private
     * @param {String|Function} fun 傳入的匿名函數或者函數名
     * @return {String}
     */
    function _createFunName(fun) {
        var name = '';

        if (baidu.lang.isFunction(fun)) {
            name = _createString();
            window[name] = function () {
                fun.apply(window, arguments);
            };

            return name;
        } else if (baidu.lang.isString) {
            return fun;
        }
    };

    /**
     * 繪製flash
     * @private
     * @param {Object} options 創建參數
     * @return {Object} 
     */
    function _render(options) {
        if (!options.id) {
            options.id = _createString();
        }

        var container = options.container || '';
        delete (options.container);

        baidu.swf.create(options, container);

        return baidu.swf.getMovie(options.id);
    };

    return function (options, callBack) {
        var me = this,
            autoRender = (typeof options.autoRender !== 'undefined' ? options.autoRender : true),
            createOptions = options.createOptions || {},
            target = null,
            isReady = false,
            callQueue = [],
            timeHandle = null,
            callBack = callBack || [];

        /**
         * 將flash檔繪製到頁面上
         * @public
         * @return {Null}
         */
        me.render = function () {
            target = _render(createOptions);

            if (callBack.length > 0) {
                baidu.each(callBack, function (funName, index) {
                    callBack[index] = _createFunName(options[funName] || new Function());
                });
            }
            me.call('setJSFuncName', [callBack]);
        };

        /**
         * 返回flash狀態
         * @return {Boolean}
         */
        me.isReady = function () {
            return isReady;
        };

        /**
         * 調用flash介面的統一入口
         * @param {String} fnName 調用的函數名
         * @param {Array} params 傳入的參數組成的陣列,若不許要參數，需傳入空陣列
         * @param {Function} [callBack] 非同步調用後將返回值作為參數的調用回呼函數，如無返回值，可以不傳入此參數
         * @return {Null}
        */
        me.call = function (fnName, params, callBack) {
            if (!fnName) return null;
            callBack = callBack || new Function();

            var result = null;

            if (isReady) {
                result = target.call(fnName, params);
                callBack(result);
            } else {
                callQueue.push({
                    fnName: fnName,
                    params: params,
                    callBack: callBack
                });

                (!timeHandle) && (timeHandle = setInterval(_check, 200));
            }
        };

        /**
         * 為傳入的匿名函數創建函數名
         * @public
         * @param {String|Function} fun 傳入的匿名函數或者函數名
         * @return {String}
         */
        me.createFunName = function (fun) {
            return _createFunName(fun);
        };

        /**
         * 檢查flash是否ready， 並進行調用
         * @private
         * @return {Null}
         */
        function _check() {
            if (_checkReady(target)) {
                clearInterval(timeHandle);
                timeHandle = null;
                _call();

                isReady = true;
            }
        };

        /**
         * 調用之前進行壓棧的函數
         * @private
         * @return {Null}
         */
        function _call() {
            _callFn(callQueue, target);
            callQueue = [];
        }

        autoRender && me.render();
    };
})();



/**
 * 創建flash based imageUploader
 * @class
 * @grammar baidu.flash.imageUploader(options)
 * @param {Object} createOptions 創建flash時需要的參數，請參照baidu.swf.create文檔
 * @config {Object} vars 創建imageUploader時所需要的參數
 * @config {Number} vars.gridWidth 每一個預覽圖片所占的寬度，應該為flash寛的整除
 * @config {Number} vars.gridHeight 每一個預覽圖片所占的高度，應該為flash高的整除
 * @config {Number} vars.picWidth 單張預覽圖片的寬度
 * @config {Number} vars.picHeight 單張預覽圖片的高度
 * @config {String} vars.uploadDataFieldName POST請求中圖片資料的key,預設值'picdata'
 * @config {String} vars.picDescFieldName POST請求中圖片描述的key,預設值'picDesc'
 * @config {Number} vars.maxSize 檔的最大體積,單位'MB'
 * @config {Number} vars.compressSize 上傳前如果圖片體積超過該值，會先壓縮
 * @config {Number} vars.maxNum:32 最大上傳多少個檔
 * @config {Number} vars.compressLength 能接受的最大邊長，超過該值會等比壓縮
 * @config {String} vars.url 上傳的url地址
 * @config {Number} vars.mode mode == 0時，是使用捲軸，mode == 1時，拉伸flash, 預設值為0
 * @see baidu.swf.createHTML
 * @param {String} backgroundUrl 背景圖片路徑
 * @param {String} listBacgroundkUrl 佈局控制項背景
 * @param {String} buttonUrl 按鈕圖片不背景
 * @param {String|Function} selectFileCallback 選擇檔的回檔
 * @param {String|Function} exceedFileCallback檔超出限制的最大體積時的回檔
 * @param {String|Function} deleteFileCallback 刪除檔的回檔
 * @param {String|Function} startUploadCallback 開始上傳某個檔時的回檔
 * @param {String|Function} uploadCompleteCallback 某個檔上傳完成的回檔
 * @param {String|Function} uploadErrorCallback 某個檔上傳失敗的回檔
 * @param {String|Function} allCompleteCallback 全部上傳完成時的回檔
 * @param {String|Function} changeFlashHeight 改變Flash的高度，mode==1的時候才有用
 */
baidu.flash.imageUploader = baidu.flash.imageUploader || function (options) {

    var me = this,
        options = options || {},
        _flash = new baidu.flash._Base(options, [
            'selectFileCallback',
            'exceedFileCallback',
            'deleteFileCallback',
            'startUploadCallback',
            'uploadCompleteCallback',
            'uploadErrorCallback',
            'allCompleteCallback',
            'changeFlashHeight'
        ]);
    /**
     * 開始或回復上傳圖片
     * @public
     * @return {Null}
     */
    me.upload = function () {
        _flash.call('upload');
    };

    /**
     * 暫停上傳圖片
     * @public
     * @return {Null}
     */
    me.pause = function () {
        _flash.call('pause');
    };
    me.addCustomizedParams = function (index, obj) {
        _flash.call('addCustomizedParams', [index, obj]);
    }
};

/**
 * 操作原生物件的方法
 * @namespace baidu.object
 */
baidu.object = baidu.object || {};


/**
 * 將源物件的所有屬性拷貝到目標物件中
 * @author erik
 * @name baidu.object.extend
 * @function
 * @grammar baidu.object.extend(target, source)
 * @param {Object} target 目標物件
 * @param {Object} source 源對象
 * @see baidu.array.merge
 * @remark
 * 
1.目標物件中，與源物件key相同的成員將會被覆蓋。<br>
2.源物件的prototype成員不會拷貝。
		
 * @shortcut extend
 * @meta standard
 *             
 * @returns {Object} 目標物件
 */
baidu.extend =
baidu.object.extend = function (target, source) {
    for (var p in source) {
        if (source.hasOwnProperty(p)) {
            target[p] = source[p];
        }
    }

    return target;
};





/**
 * 創建flash based fileUploader
 * @class
 * @grammar baidu.flash.fileUploader(options)
 * @param {Object} options
 * @config {Object} createOptions 創建flash時需要的參數，請參照baidu.swf.create文檔
 * @config {String} createOptions.width
 * @config {String} createOptions.height
 * @config {Number} maxNum 最大可選文件數
 * @config {Function|String} selectFile
 * @config {Function|String} exceedMaxSize
 * @config {Function|String} deleteFile
 * @config {Function|String} uploadStart
 * @config {Function|String} uploadComplete
 * @config {Function|String} uploadError
 * @config {Function|String} uploadProgress
 */
baidu.flash.fileUploader = baidu.flash.fileUploader || function (options) {
    var me = this,
        options = options || {};

    options.createOptions = baidu.extend({
        wmod: 'transparent'
    }, options.createOptions || {});

    var _flash = new baidu.flash._Base(options, [
        'selectFile',
        'exceedMaxSize',
        'deleteFile',
        'uploadStart',
        'uploadComplete',
        'uploadError',
        'uploadProgress'
    ]);

    _flash.call('setMaxNum', options.maxNum ? [options.maxNum] : [1]);

    /**
     * 設置當滑鼠移動到flash上時，是否變成手型
     * @public
     * @param {Boolean} isCursor
     * @return {Null}
     */
    me.setHandCursor = function (isCursor) {
        _flash.call('setHandCursor', [isCursor || false]);
    };

    /**
     * 設置滑鼠相應函數名
     * @param {String|Function} fun
     */
    me.setMSFunName = function (fun) {
        _flash.call('setMSFunName', [_flash.createFunName(fun)]);
    };

    /**
     * 執行上傳操作
     * @param {String} url 上傳的url
     * @param {String} fieldName 上傳的表單字段名
     * @param {Object} postData 鍵值對，上傳的POST數據
     * @param {Number|Array|null|-1} [index]上傳的檔序列
     *                            Int值上傳該文件
     *                            Array一次串列上傳該序列檔
     *                            -1/null上傳所有檔
     * @return {Null}
     */
    me.upload = function (url, fieldName, postData, index) {

        if (typeof url !== 'string' || typeof fieldName !== 'string') return null;
        if (typeof index === 'undefined') index = -1;

        _flash.call('upload', [url, fieldName, postData, index]);
    };

    /**
     * 取消上傳操作
     * @public
     * @param {Number|-1} index
     */
    me.cancel = function (index) {
        if (typeof index === 'undefined') index = -1;
        _flash.call('cancel', [index]);
    };

    /**
     * 刪除檔
     * @public
     * @param {Number|Array} [index] 要刪除的index，不傳則全部刪除
     * @param {Function} callBack
     * */
    me.deleteFile = function (index, callBack) {

        var callBackAll = function (list) {
            callBack && callBack(list);
        };

        if (typeof index === 'undefined') {
            _flash.call('deleteFilesAll', [], callBackAll);
            return;
        };

        if (typeof index === 'Number') index = [index];
        index.sort(function (a, b) {
            return b - a;
        });
        baidu.each(index, function (item) {
            _flash.call('deleteFileBy', item, callBackAll);
        });
    };

    /**
     * 添加檔案類型，支持macType
     * @public
     * @param {Object|Array[Object]} type {description:String, extention:String}
     * @return {Null};
     */
    me.addFileType = function (type) {
        var type = type || [[]];

        if (type instanceof Array) type = [type];
        else type = [[type]];
        _flash.call('addFileTypes', type);
    };

    /**
     * 設置檔案類型，支援macType
     * @public
     * @param {Object|Array[Object]} type {description:String, extention:String}
     * @return {Null};
     */
    me.setFileType = function (type) {
        var type = type || [[]];

        if (type instanceof Array) type = [type];
        else type = [[type]];
        _flash.call('setFileTypes', type);
    };

    /**
     * 設置可選檔的數量限制
     * @public
     * @param {Number} num
     * @return {Null}
     */
    me.setMaxNum = function (num) {
        _flash.call('setMaxNum', [num]);
    };

    /**
     * 設置可選檔大小限制，以兆M為單位
     * @public
     * @param {Number} num,0為無限制
     * @return {Null}
     */
    me.setMaxSize = function (num) {
        _flash.call('setMaxSize', [num]);
    };

    /**
     * @public
     */
    me.getFileAll = function (callBack) {
        _flash.call('getFileAll', [], callBack);
    };

    /**
     * @public
     * @param {Number} index
     * @param {Function} [callBack]
     */
    me.getFileByIndex = function (index, callBack) {
        _flash.call('getFileByIndex', [], callBack);
    };

    /**
     * @public
     * @param {Number} index
     * @param {function} [callBack]
     */
    me.getStatusByIndex = function (index, callBack) {
        _flash.call('getStatusByIndex', [], callBack);
    };
};

/**
 * 使用動態script標籤請求伺服器資源，包括由伺服器端的回檔和流覽器端的回檔
 * @namespace baidu.sio
 */
baidu.sio = baidu.sio || {};

/**
 * 
 * @param {HTMLElement} src script節點
 * @param {String} url script節點的地址
 * @param {String} [charset] 編碼
 */
baidu.sio._createScriptTag = function (scr, url, charset) {
    scr.setAttribute('type', 'text/javascript');
    charset && scr.setAttribute('charset', charset);
    scr.setAttribute('src', url);
    document.getElementsByTagName('head')[0].appendChild(scr);
};

/**
 * 刪除script的屬性，再刪除script標籤，以解決修復記憶體洩漏的問題
 * 
 * @param {HTMLElement} src script節點
 */
baidu.sio._removeScriptTag = function (scr) {
    if (scr.clearAttributes) {
        scr.clearAttributes();
    } else {
        for (var attr in scr) {
            if (scr.hasOwnProperty(attr)) {
                delete scr[attr];
            }
        }
    }
    if (scr && scr.parentNode) {
        scr.parentNode.removeChild(scr);
    }
    scr = null;
};


/**
 * 通過script標籤載入資料，載入完成由流覽器端觸發回檔
 * @name baidu.sio.callByBrowser
 * @function
 * @grammar baidu.sio.callByBrowser(url, opt_callback, opt_options)
 * @param {string} url 載入數據的url
 * @param {Function|string} opt_callback 資料載入結束時調用的函數或函數名
 * @param {Object} opt_options 其他可選項
 * @config {String} [charset] script的字元集
 * @config {Integer} [timeOut] 超時時間，超過這個時間將不再回應本請求，並觸發onfailure函數
 * @config {Function} [onfailure] timeOut設定後才生效，到達超時時間時觸發本函數
 * @remark
 * 1、與callByServer不同，callback參數只支援Function類型，不支援string。
 * 2、如果請求了一個不存在的頁面，callback函數在IE/opera下也會被調用，因此使用者需要在onsuccess函數中判斷資料是否正確載入。
 * @meta standard
 * @see baidu.sio.callByServer
 */
baidu.sio.callByBrowser = function (url, opt_callback, opt_options) {
    var scr = document.createElement("SCRIPT"),
        scriptLoaded = 0,
        options = opt_options || {},
        charset = options['charset'],
        callback = opt_callback || function () { },
        timeOut = options['timeOut'] || 0,
        timer;
    scr.onload = scr.onreadystatechange = function () {
        if (scriptLoaded) {
            return;
        }

        var readyState = scr.readyState;
        if ('undefined' == typeof readyState
            || readyState == "loaded"
            || readyState == "complete") {
            scriptLoaded = 1;
            try {
                callback();
                clearTimeout(timer);
            } finally {
                scr.onload = scr.onreadystatechange = null;
                baidu.sio._removeScriptTag(scr);
            }
        }
    };

    if (timeOut) {
        timer = setTimeout(function () {
            scr.onload = scr.onreadystatechange = null;
            baidu.sio._removeScriptTag(scr);
            options.onfailure && options.onfailure();
        }, timeOut);
    }

    baidu.sio._createScriptTag(scr, url, charset);
};

/**
 * 通過script標籤載入資料，載入完成由伺服器端觸發回檔
 * @name baidu.sio.callByServer
 * @function
 * @grammar baidu.sio.callByServer(url, callback[, opt_options])
 * @param {string} url 載入數據的url.
 * @param {Function|string} callback 伺服器端調用的函數或函數名。如果沒有指定本參數，將在URL中尋找options['queryField']做為callback的方法名.
 * @param {Object} opt_options 載入資料時的選項.
 * @config {string} [charset] script的字元集
 * @config {string} [queryField] 伺服器端callback請求欄位名，默認為callback
 * @config {Integer} [timeOut] 超時時間(單位：ms)，超過這個時間將不再回應本請求，並觸發onfailure函數
 * @config {Function} [onfailure] timeOut設定後才生效，到達超時時間時觸發本函數
 * @remark
 * 如果url中已經包含key為“options['queryField']”的query項，將會被替換成callback中參數傳遞或自動生成的函數名。
 * @meta standard
 * @see baidu.sio.callByBrowser
 */
baidu.sio.callByServer = /**@function*/function (url, callback, opt_options) {
    var scr = document.createElement('SCRIPT'),
        prefix = 'bd__cbs__',
        callbackName,
        callbackImpl,
        options = opt_options || {},
        charset = options['charset'],
        queryField = options['queryField'] || 'callback',
        timeOut = options['timeOut'] || 0,
        timer,
        reg = new RegExp('(\\?|&)' + queryField + '=([^&]*)'),
        matches;

    if (baidu.lang.isFunction(callback)) {
        callbackName = prefix + Math.floor(Math.random() * 2147483648).toString(36);
        window[callbackName] = getCallBack(0);
    } else if (baidu.lang.isString(callback)) {
        callbackName = callback;
    } else {
        if (matches = reg.exec(url)) {
            callbackName = matches[2];
        }
    }

    if (timeOut) {
        timer = setTimeout(getCallBack(1), timeOut);
    }
    url = url.replace(reg, '\x241' + queryField + '=' + callbackName);

    if (url.search(reg) < 0) {
        url += (url.indexOf('?') < 0 ? '?' : '&') + queryField + '=' + callbackName;
    }
    baidu.sio._createScriptTag(scr, url, charset);

    /*
     * 返回一個函數，用於立即（掛在window上）或者超時（掛在setTimeout中）時執行
     */
    function getCallBack(onTimeOut) {
        /*global callbackName, callback, scr, options;*/
        return function () {
            try {
                if (onTimeOut) {
                    options.onfailure && options.onfailure();
                } else {
                    callback.apply(window, arguments);
                    clearTimeout(timer);
                }
                window[callbackName] = null;
                delete window[callbackName];
            } catch (exception) {
            } finally {
                baidu.sio._removeScriptTag(scr);
            }
        }
    }
};

/**
 * 通過請求一個圖片的方式令伺服器存儲一條日誌
 * @function
 * @grammar baidu.sio.log(url)
 * @param {string} url 要發送的地址.
 * @author: int08h,leeight
 */
baidu.sio.log = function (url) {
    var img = new Image(),
        key = 'tangram_sio_log_' + Math.floor(Math.random() *
              2147483648).toString(36);
    window[key] = img;

    img.onload = img.onerror = img.onabort = function () {
        img.onload = img.onerror = img.onabort = null;

        window[key] = null;
        img = null;
    };
    img.src = url;
};



/*
 * Tangram
 * Copyright 2009 Baidu Inc. All rights reserved.
 * 
 * path: baidu/json.js
 * author: erik
 * version: 1.1.0
 * date: 2009/12/02
 */


/**
 * 操作json物件的方法
 * @namespace baidu.json
 */
baidu.json = baidu.json || {};
/*
 * Tangram
 * Copyright 2009 Baidu Inc. All rights reserved.
 * 
 * path: baidu/json/parse.js
 * author: erik, berg
 * version: 1.2
 * date: 2009/11/23
 */



/**
 * 將字串解析成json物件。注：不會自動祛除空格
 * @name baidu.json.parse
 * @function
 * @grammar baidu.json.parse(data)
 * @param {string} source 需要解析的字串
 * @remark
 * 該方法的實現與ecma-262第五版中規定的JSON.parse不同，暫時只支持傳入一個參數。後續會進行功能豐富。
 * @meta standard
 * @see baidu.json.stringify,baidu.json.decode
 *             
 * @returns {JSON} 解析結果json對象
 */
baidu.json.parse = function (data) {
    //2010/12/09：更新至不使用原生parse，不檢測用戶輸入是否正確
    return (new Function("return (" + data + ")"))();
};
/*
 * Tangram
 * Copyright 2009 Baidu Inc. All rights reserved.
 * 
 * path: baidu/json/decode.js
 * author: erik, cat
 * version: 1.3.4
 * date: 2010/12/23
 */



/**
 * 將字串解析成json物件，為過時介面，今後會被baidu.json.parse代替
 * @name baidu.json.decode
 * @function
 * @grammar baidu.json.decode(source)
 * @param {string} source 需要解析的字串
 * @meta out
 * @see baidu.json.encode,baidu.json.parse
 *             
 * @returns {JSON} 解析結果json對象
 */
baidu.json.decode = baidu.json.parse;
/*
 * Tangram
 * Copyright 2009 Baidu Inc. All rights reserved.
 * 
 * path: baidu/json/stringify.js
 * author: erik
 * version: 1.1.0
 * date: 2010/01/11
 */



/**
 * 將json物件序列化
 * @name baidu.json.stringify
 * @function
 * @grammar baidu.json.stringify(value)
 * @param {JSON} value 需要序列化的json物件
 * @remark
 * 該方法的實現與ecma-262第五版中規定的JSON.stringify不同，暫時只支持傳入一個參數。後續會進行功能豐富。
 * @meta standard
 * @see baidu.json.parse,baidu.json.encode
 *             
 * @returns {string} 序列化後的字串
 */
baidu.json.stringify = (function () {
    /**
     * 字串處理時需要轉義的字元表
     * @private
     */
    var escapeMap = {
        "\b": '\\b',
        "\t": '\\t',
        "\n": '\\n',
        "\f": '\\f',
        "\r": '\\r',
        '"': '\\"',
        "\\": '\\\\'
    };

    /**
     * 字串序列化
     * @private
     */
    function encodeString(source) {
        if (/["\\\x00-\x1f]/.test(source)) {
            source = source.replace(
                /["\\\x00-\x1f]/g,
                function (match) {
                    var c = escapeMap[match];
                    if (c) {
                        return c;
                    }
                    c = match.charCodeAt();
                    return "\\u00"
                            + Math.floor(c / 16).toString(16)
                            + (c % 16).toString(16);
                });
        }
        return '"' + source + '"';
    }

    /**
     * 陣列序列化
     * @private
     */
    function encodeArray(source) {
        var result = ["["],
            l = source.length,
            preComma, i, item;

        for (i = 0; i < l; i++) {
            item = source[i];

            switch (typeof item) {
                case "undefined":
                case "function":
                case "unknown":
                    break;
                default:
                    if (preComma) {
                        result.push(',');
                    }
                    result.push(baidu.json.stringify(item));
                    preComma = 1;
            }
        }
        result.push("]");
        return result.join("");
    }

    /**
     * 處理日期序列化時的補零
     * @private
     */
    function pad(source) {
        return source < 10 ? '0' + source : source;
    }

    /**
     * 日期序列化
     * @private
     */
    function encodeDate(source) {
        return '"' + source.getFullYear() + "-"
                + pad(source.getMonth() + 1) + "-"
                + pad(source.getDate()) + "T"
                + pad(source.getHours()) + ":"
                + pad(source.getMinutes()) + ":"
                + pad(source.getSeconds()) + '"';
    }

    return function (value) {
        switch (typeof value) {
            case 'undefined':
                return 'undefined';

            case 'number':
                return isFinite(value) ? String(value) : "null";

            case 'string':
                return encodeString(value);

            case 'boolean':
                return String(value);

            default:
                if (value === null) {
                    return 'null';
                } else if (value instanceof Array) {
                    return encodeArray(value);
                } else if (value instanceof Date) {
                    return encodeDate(value);
                } else {
                    var result = ['{'],
                        encode = baidu.json.stringify,
                        preComma,
                        item;

                    for (var key in value) {
                        if (Object.prototype.hasOwnProperty.call(value, key)) {
                            item = value[key];
                            switch (typeof item) {
                                case 'undefined':
                                case 'unknown':
                                case 'function':
                                    break;
                                default:
                                    if (preComma) {
                                        result.push(',');
                                    }
                                    preComma = 1;
                                    result.push(encode(key) + ':' + encode(item));
                            }
                        }
                    }
                    result.push('}');
                    return result.join('');
                }
        }
    };
})();
/*
 * Tangram
 * Copyright 2009 Baidu Inc. All rights reserved.
 * 
 * path: baidu/json/encode.js
 * author: erik, cat
 * version: 1.3.4
 * date: 2010/12/23
 */



/**
 * 將json物件序列化，為過時介面，今後會被baidu.json.stringify代替
 * @name baidu.json.encode
 * @function
 * @grammar baidu.json.encode(value)
 * @param {JSON} value 需要序列化的json物件
 * @meta out
 * @see baidu.json.decode,baidu.json.stringify
 *             
 * @returns {string} 序列化後的字串
 */
baidu.json.encode = baidu.json.stringify;
