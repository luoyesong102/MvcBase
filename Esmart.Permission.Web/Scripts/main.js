var main = { submit: null, getData: null, getJsonp: null, getHtml: null, model: null,show:null, downFile:null,ifrmae:null, printError: function () { }, clearError: function () { }, getModel: null, initModel: null, extend: {},token:null,isLoading:false };
//alert 弹出警告信息框 confirm 弹出信息确认框 getData获取数据 modeldiv 弹出层 
//getHtml DIV赋值视图  ifrmae弹出窗用iframe打开 windows.open    gotoPage跳转第几页  printError错误 
//布局方式:开始从layout启动，1--- @RenderBody()+layout --- @RenderSection("TopFunction", false)  @section TopFunction{}++layout -- @{ Html.RenderPartial( "StudentDetail", Model ); } Html.RenderAction( "StudentDetail", Model ) -- +2div ajax 赋值html  3弹出层 4frame
//布局：开始启动layout ,后期content gethtml() ajax异步赋值html(未采用body集成layout)-- 加载dabtal.cshtml(index查询条件和datatablesourse)

//弹出警告信息框 
main.alert = function (message) { 
    bootbox.alert({
        size: 'small',
        message: message
    });
};

//弹出信息确认框
main.confirm = function (message, callback) {
    bootbox.confirm({
        size: 'small',
        className: "modal-confirm",
        message: message,
        callback: callback
    });
};
//$(function() {
    
//})
//jquery的扩展操作也可重写
(function ($) {
    //为带tab的表单附加验证引擎
    $.fn.tpoValidationEngine = function () {
        var returnFlag = true;
        var error_pane = null;
        $(this).find("input,select,textarea").each(function (index, element) {
            $(element).clearError();

        }).bind("jqv.field.result", function (event, field, isError, promptText) {
            if (isError) {
                if (!error_pane) {
                    error_pane=$(field).tabShow();
                }
                if (error_pane.length > 0) {
                    field = error_pane.find(field);
                }
                $(field).error(promptText);
            }
        })


        if (!returnFlag) {
            return returnFlag;
        }
        returnFlag = $(this).validationEngine('validate', {
            validateNonVisibleFields: true,
            showPrompts: false,
            scroll:false
        });

        $(this).validationEngine('detach');
        return returnFlag;
    };

    //禁用表单点击Enter键submit
    $.fn.tpoDisableEnterSubmit = function () {
        return this.each(function () {
            var $this = $(this);
            $this.on('keyup keypress', function (e) {
                var code = e.keyCode || e.which;
                if (code == 13) {
                    e.preventDefault();
                    return false;
                }
            });
        });
    };

    $.fn.error = function (message) {
        var errortemp = " <span class='glyphicon glyphicon-remove form-control-feedback' aria-hidden='true' data-close></span><span  class='sr-only' data-close>(error)</span>";
        $(this).attr("title", "");
        $(this).parent("div").removeClass("has-success").addClass("has-error has-feedback").find("span[data-close]").remove();
        $(errortemp).appendTo($(this).parent("div"));
        var containt = $(this).parent("div").parent("div");
        if (containt.length == 0) {
            containt = $(document.body);
        }
        $(this).popover({ content: "<span style=\"color:red;font-size:12px;\">" + message + "</span>", container: containt, placement: "right", html: true }).popover("show");

        $(this).bind("click", function () {
            $(this).clearError();
        })

    }

    $.fn.clearError = function () {
        
        var type = $(this).attr("type");
        if (type) {
            type = type.toLowerCase();
        }
        if (type == "checkbox" || type == "radio") {
            $(this).parent("div").parent("div").removeClass("has-error has-success has-feedback").find("span[data-close]").remove();
        }
        else {
            $(this).parent("div").removeClass("has-error has-success has-feedback").find("span[data-close]").remove();
        }
        $(this).popover("destroy");
    }

    $.fn.tabShow = function () {
        var error_pane = $(this).closest(".tab-pane");
        var tab = $("a[href='#" + error_pane.attr("id") + "']");
        tab.tab("show");
        return error_pane;
    }
})(jQuery);

//自定义对象的操作实现也可重写
(function ($) {
    var popoverTemp = null, inputvalid = null, errCodes = new Array(), modelurls = new Array(), pageId = 0, token = null, ishowmodel = false;
    function clearError() {
        if (inputvalid != null) {
            for (var i = 0; i < inputvalid.length; i++) {
                inputvalid[i].removeClass("has-error");
            }
        }
        if (popoverTemp != null) {
            popoverTemp.hide();
        }
    }

    ///表单提交
    $.fn.submit = function (callback, error) {
        var form = $(this).closest("form");
        var url = form.attr("action");
        if (this.get(0) == form.get(0)) {
            $(this).get(0).action = getPageUrl(url);
            $(this).get(0).submit();
        }
        else {
            main.submit(url, main.getModel(form), callback, error);
        }
    }


    $.fn.registerSubmit = function (valid, callback) {
        var obj = this;
        $(this).click(function () {
            var form = $(obj).closest("form");
            var url = form.attr("action");
            var data = main.getModel(form);
            if (valid && !valid(data)) {
                return;
            }
            main.submit(url, data, callback);
        });
    }

    function popover(obj, body) {
        var offset = $(obj).offset();
        var left = offset ? offset.left : 0;
        var top = offset ? offset.top : 0;
        var width = $(obj).width();
        var height = $(obj).height();
        if (popoverTemp != null) {
            $(popoverTemp).html(body);
        }
        else {
            var temp = "<div class=\"alert alert-danger\">{0}</div>";
            temp = temp.replace("{0}", body);
            popoverTemp = $(temp).appendTo(document.body);
            $(popoverTemp).width(300);
            $(popoverTemp).height($(obj).height() - 5);
            $(popoverTemp).css({ position: "absolute" });
        }
        $(popoverTemp).css("left", left + width + 30);
        $(popoverTemp).css("top", top - $(popoverTemp).height() / 2 + 7);
        $(popoverTemp).show();
    }
    function inputError(obj, body) {
        var fromgroup = $(obj).closest(".form-group");
        fromgroup.addClass("has-error");
        if (inputvalid == null) {
            inputvalid = new Array();
        }
        inputvalid.push(fromgroup);
        popover(obj, body);
    }
    function getmodelFormData(form) {
        var data = {};
        $(form).find("input").each(function (index, element) {
            var type = $(element).attr("type");
            type = (type ? type : "").toLowerCase();
            var issimple = form.find("input[name='" + $(element).attr("name") + "']").length == 1;
            switch (type) {
                case "checkbox":
                    var checkboxs = data[$(element).attr("name")];
                    if (!checkboxs) {
                        checkboxs = new Array();
                    }
                    if (element.checked) {
                        checkboxs.push(element.value);
                    }
                    if ($(element).attr("isSimple")) {
                        data[$(element).attr("name")] = element.checked;
                    }
                    else {
                        data[$(element).attr("name")] = checkboxs;
                    }
                    break;
                case "radio":
                    if (element.checked) {
                        data[$(element).attr("name")] = $(element).val();
                    }
                    break;
                case "button":
                case "submit":
                    break;
                default:
                    if (issimple) {
                        var datavalue = $(element).attr("data-value");
                        if (datavalue) {
                            data[$(element).attr("name") + "Id"] = datavalue;
                        }
                        data[$(element).attr("name")] = $(element).val();
                    }
                    else {
                        var values = data[$(element).attr("name")];
                        if (!values) {
                            values = new Array();
                        }
                        values.push($(element).val());
                        data[$(element).attr("name")] = values;
                    }
                    break;
            }
        });
        $(form).find("textarea").each(function (index, element) {
            data[$(element).attr("name")] = $(element).val();
        });
        $(form).find("select").each(function (index, element) {
            data[$(element).attr("name")] = $(element).val();
        });
        return data;
    }
    main.printError = function () {
        for (var i = 0; i < errCodes.length; i++) {
            window.console.log(errCodes[i]);
        }
    };
    main.clearError = clearError;

    main.reload = function () {
        if (window.localStorage.href) {
            main.getHtml(window.localStorage.href);
        }
    }
    function clearUrl(url) {
        var arr = new Array();
        for (var i = 0; i < modelurls.length; i++) {
            if (modelurls[i] != url) {
                arr.push(modelurls[i]);
            }
        }
        modelurls = arr;
    }

    function getPageUrl(url) {
        if (url == "") {
            return "";
        }
        url = url.indexOf("?") == -1 ? (url + "?rand=" + Math.random()) : (url + "&rand=" + Math.random());
        var index = url.indexOf("PageId");
        if (index != -1) {
            var pageinfo = url.substr(index, url.length);
            if (pageinfo.indexOf("&") != -1) {
                pageId = pageinfo.split("&")[0].split("=")[1];
            }
            else if (pageinfo.indexOf("#") != -1) {
                pageId = pageinfo.split("#")[0].split("=")[1];
            }
        }
        if (pageId == 0) {
            var wurl = window.location.href;
            index = wurl.indexOf("PageId");
            if (index != -1) {
                var pageinfo = wurl.substr(index, wurl.length);
                if (pageinfo.indexOf("&") != -1) {
                    pageId = pageinfo.split("&")[0].split("=")[1];
                }
                else if (pageinfo.indexOf("#") != -1) {
                    pageId = pageinfo.split("#")[0].split("=")[1];
                }
            }
        }
        if (pageId == 0) {
            pageId = getCookie("PageId") || 0;
        } else {
            setCookie("PageId", pageId);
        }
        if (index == -1) {
            url = url + "&PageId=" + pageId;
        }
        return url;
    }


    function setCookie(name, value) {
        document.cookie = name + "=" + escape(value) + ";path=/";
    }

    //读取cookies 
    function getCookie(name) {
        var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

        if (arr = document.cookie.match(reg))

            return unescape(arr[2]);
        else
            return null;
    }

    main.getUrl = getPageUrl;

    main.downFile = function (url) {
        window.closeWindows = true;
        if (window.dataPage.count == 0) {
            main.alert("无数据导出");
            return;
        }
        var data = getmodelFormData($("#data_search_info"));
        url = getPageUrl(url);
        for (var id in data) {
            if (!data[id]) {
                continue;
            }
            if (data[id] instanceof Array) {
                url = url + "&" + id + "=" + data[id].join(",");
            }
            else {
                url = url + "&" + id + "=" + data[id];
            }
        }
        var displayName = new Array(); var colName = new Array();

        $("#data_body_info").find("table tr th").each(function (element, index) {
         
            if ($(this).attr("data-Excel")) {
                colName.push($(this).attr("data-Excel"));
                displayName.push($(this).html().replace(/(^\s*)|(\s*$)/g, ""));
            }
          
        })
        if (!colName && !displayName) {
            return;
        }
        url = url + "&displayName=" + displayName.join(",");
        url = url + "&colName=" + colName.join(",");
        window.location.href = url;
    }
    main.getModel = getmodelFormData;
    main.model = function (url, config) {
        if (ishowmodel) {
            return;
        }
        ishowmodel = true;
        main.getHtml(url, config.data, function (html) {
            ishowmodel = false;
            var modelcalss = ["bs-example-modal-lg", "modal-lg"];
            if (config.level == 1) {
                modelcalss = ["bs-example-modal-sm", "modal-sm"];
            }
            if (config.level == 2) {
                modelcalss = ["", ""];
            }
            if (config.level == 0) {
                modelcalss = ["bs-example-modal-blg", "modal-blg"];
            }
            if (config.level == 1200) {
                modelcalss = ["bs-example-modal-blg", " modal-1200lg"];
            }
            var style = "";
            if (config.width) {
                if (config.width > $(window).width() - 20) {
                    config.width = $(window).width() - 20;
                }
                style = style + "width:" + config.width + "px;"
            }
            if (config.height) {
                if (config.height > $(window).height() - 10) {
                    config.height = $(window).height() - 10;
                }
                  style = style + "height:" +config.height + "px;"
                }
            if (config.width || config.height) {
                style = "style=\"" + style + "\"";
            }
            var containt = $("#contentRenderBody");
            var isparentContaint = false;
            if (containt.length == 0) {
                if (window.parent && window.parent.document && window.parent.document.body) {
                    containt = $(window.parent.document.body).find("#contentRenderBody");
                    isparentContaint = true;
                }  
            }
            if (containt.length == 0) {
                containt = document.body;
            }
            var temp = "<div class=\"modal fade " + modelcalss[0] + "\"  >    <div class=\"modal-dialog " + modelcalss[1] + "\"  " + style + ">  <div class=\"modal-content\" >  <div class=\"modal-header\">";
            temp += "  <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\" ><i class=\"fa fa-times\"></i></span></button>";
            temp += "  <h4 class=\"modal-title\" >" + (config ? config.title : "管理系统") + "</h4> </div>";
            temp += " <div class=\"modal-body\"  style=\"max-height:" + (config.height ? (config.height - 56) : 600) + "px; overflow-y:auto;\"> " + html + " </div>";
            var isshowFoot = true;
            if (config && config.buttons && config && config.buttons.length == 0) {
                isshowFoot = false;
            } else {
                temp += "     <div class=\"modal-footer\">";
            }

            if (config && config.buttons) {
                var i = 0;
                for (var button in config.buttons) {
                    if (config.buttons[button] === "close") {
                        temp += " <button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">关闭</button>";
                    } else {
                        temp += " <button type=\"button\"";
                        if (config.buttons[button]["class"]) {
                            temp += "  class=\"" + config.buttons[button]["class"] + "\"";
                        }
                        else {
                            temp += "  class=\"btn btn-primary\"";
                    }
                        temp += "  data-modelnum=\"" + i + "\">";
                        i++;
                        temp += config.buttons[button]["value"];
                        temp += "</button>";
                }
            }
            } else {
                temp += " <button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">关闭</button>";
                temp += " <button type=\"button\" class=\"btn btn-primary\" data-modelok=\"ok\"  >确认</button>";
            }
            if (isshowFoot) {
                temp += "  </div>  ";
            }
            temp += "</div>  </div></div>";

            var divModel = $(temp).appendTo(containt);

            divModel.modal({ backdrop: 'static' });

            $(divModel).on('hidden.bs.modal', function (e) {
                $(document.body).css({ "overflow-y": "auto" });
                $(divModel).remove();
            });
            $(divModel).on('shown.bs.modal', function (e) {
                $(document.body).css({ "overflow-y": "hidden" });
            });
            $(divModel).bind("keydown", function (event) {
                if (event.which == 13) {
                    event.stopPropagation();
                  
                }
                return true;
            })
       
            $(divModel).find("[data-modelok]").click(function (event) {
                var data = getmodelFormData($(divModel).find(".modal-body"));
                var form = $(divModel).find("form");
                if (form && !form.tpoValidationEngine()) {
                    $(form).validationEngine('hideAll');
                    if (event && event.preventDefault) {
                        event.preventDefault();
                    }
                    return false;
                }
                var action = form.attr("action");
                if (action) {
                    url = action;
                }
                if (config.defaultClick) {
                     
                    config.defaultClick(data, url, $(divModel));
                    return;
                }
                if (config.defaultValid && !config.defaultValid(data)) {
                    return;
                }
                if (config.initData) {
                    data = config.initData(data);
                }
                if (config.confirmMessage) {
                    main.confirm(config.confirmMessage, function (result) {
                        if (result) {
                            main.submit(url, data, function (data) {
                                $(divModel).modal("hide");
                                if (config.callback) {
                                    config.callback(data);
                                    return;
                                }
                                if (window.dataPage) {
                                    if (config.isDelete) {
                                        dataPage.remove(1);
                                    }
                                    else {
                                        dataPage.search();
                                    }
                                }
                            }, function (code) {
                                if (config.error) {
                                    config.error(code);
                                }
                                else {
                                    if (code.Header.PropertyName) {
                                        var propertyName = $(divModel).find("[name='" + code.Header.PropertyName + "']");
                                        var type = $(propertyName).attr("type");
                                        if (type) {
                                            type = type.toLowerCase();
                                        }
                                        if (type == "checkbox" || type == "radio") {
                                            propertyName = propertyName.parent("div");
                                        }
                                        propertyName.tabShow();
                                        propertyName.error(code.Header.Message);

                                    }
                                    else {
                                        main.extend.subprogress(100, true);
                                        main.extend.alert.find("p").html(code.Header.Message);
                                        main.extend.alert.show();
                                    }
                                }
                            });
                        }

                    })
                }
                else {
                    main.submit(url, data, function (data) {
                        $(divModel).modal("hide");
                        if (config.callback) {
                            config.callback(data);
                            return;
                        }
                        if (window.dataPage) {
                            if (config.isDelete) {
                                dataPage.remove(1);
                            }
                            else {
                                dataPage.search();
                            }
                        }
                    }, function (code) {
                        if (config.error) {
                            config.error(code);
                        }
                        else {
                            if (code.Header.PropertyName) {
                                var propertyName = $(divModel).find("[name='" + code.Header.PropertyName + "']");
                                var type = $(propertyName).attr("type");
                                if (type) {
                                    type = type.toLowerCase();
                                }
                                if (type == "checkbox" || type == "radio") {
                                    propertyName = propertyName.parent("div");
                                }
                                propertyName.tabShow();
                                propertyName.error(code.Header.Message);

                            }
                            else {
                                main.extend.subprogress(100, true);
                                main.extend.alert.find("p").html(code.Header.Message);
                                main.extend.alert.show();
                            }
                        }
                    });
                }

            });
            $(divModel).find("[data-modelnum]").click(function () {
                var data = getmodelFormData($(divModel).find(".modal-body"));
                config.buttons[$(this).attr("data-modelnum") * 1].click(data, $(divModel));
                clearUrl(url);
            });
            var modalcontent = $(divModel).find(".modal-content");
            var modelBody = $(modalcontent).find(".modal-body");
            if (isparentContaint) {
                modalcontent.draggable({ handle: $(divModel).find(".modal-header") });
            } else {
                modalcontent.draggable({ handle: $(divModel).find(".modal-header"), containment: "window" });
            }
          
            modalcontent.resizable({
                    minHeight: 300,
                    minWidth: 500,
                    start: function (event, ui) {
                    main.extend.resizable = true;
            },
                    resize: function (event, ui) {
                    var height = modalcontent.height();
                    modelBody.css({ "height": (height - 110) + "px", "max-height": (height - 110) + "px" });
            },
                    stop: function (event, ui) {
                    main.extend.resizable = false;
            }
            });
        });

    };
    main.ifrmae = function (url, title, callback, isNotUserDefaultClick, width, height, valid) {
        window.closeWindows = true;
        url = getPageUrl(url) + "&showIframeModel=1";
        var modelcalss = ["bs-example-modal-lg", "modal-1200lg"];
        var style = "";
        if (width) { 
            if (width > $(window).width() - 20) {
                width= $(window).width() -20;
            }
            style = style + "width:" + width + "px;";
            
        }
        if (height) {
            if (height > $(window).height() -10) {
                height =$(window).height() -10;
            }
            style = style + "height:" +height + "px;";
          }
        if (width || height) {
            style = "style=\"" + style + "\"";
        }
        var temp = "<div class=\"modal fade " + modelcalss[0] + "\"   >    <div class=\"modal-dialog " + modelcalss[1] + "\" " + style + " >  <div class=\"modal-content\" >  <div class=\"modal-header\">";
        temp += "  <button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\"><span aria-hidden=\"true\">&times;</span></button>";
        temp += "  <h4 class=\"modal-title\" >" + (title ? title : "管理系统") + "</h4> </div>";
        temp += " <div class=\"modal-body \"  style=\"height:" + (height ? (height - 56) : 650) + "px; overflow-y:hidden; margin:0px;padding:0px;\"> ";
        temp += "<iframe src=\"" + url + "\" frameborder=\"0\"   style=\"width:100%;height:" + (height ? (height - 26) : 650) + "px;margin:0px;padding:0px;\"> </iframe>";
        temp += "</div>";
        temp += " <div class=\"modal-footer\">";
        temp += " <button type=\"button\" class=\"btn btn-default\" data-dismiss=\"modal\">关闭</button>";
        if (!isNotUserDefaultClick) {
            temp += " <button type=\"button\" class=\"btn btn-primary\" data-modelok=\"ok\"  >确认</button>";
        }
        temp += "  </div>  ";
        temp += "</div>  </div></div>";
        var containt = window;
        if (window.parent != null) {
            containt= $(window.parent.document).find("#contentRenderBody");
        }
        else {
            containt = $("#contentRenderBody");
        }
        if (containt.length == 0) {
            containt = $(document.body);
        }
        var divModel = $(temp).appendTo(containt);

        var ifrmaecontexts = $(divModel).find("iframe")[0].contentWindow;
        $(divModel).on('shown.bs.modal', function (e) {
            $(document.body).css({ "overflow-y": "hidden" });
        });

        $(divModel).on('hidden.bs.modal', function (e) {
            $(document.body).css({ "overflow-y": "auto" });
            $(divModel).remove();
        })
        $(divModel).find("[data-modelok]").click(function () {
            var form = $(ifrmaecontexts.document).find("form");

            if (form && !form.tpoValidationEngine()) {
                event.preventDefault();
                return false;
            }
            if (valid && !valid()) {
                event.preventDefault();
                return false;
            }
            var action = form.attr("action");
            if (action) {
                url = action;
            }
            url = getPageUrl(url);
            var objform = form.get(0);
            objform.action = url;
            objform.submit();
            $(divModel).modal("hide");
            if (callback) {
                callback();
            }
        });
        divModel.modal({ backdrop: 'static' });
        var modalcontent = $(divModel).find(".modal-content");
        var modelBody = $(modalcontent).find(".modal-body");
        var iframe = $(modelBody).find("iframe");
        modalcontent.draggable({ handle: $(divModel).find(".modal-header"), containment: "window" });
        modalcontent.resizable({
            minHeight: 300,
            minWidth: 500,
            start: function (event, ui) {
                main.extend.resizable = true;
            },
            resize: function (event, ui) {
                var height = modalcontent.height();
                modelBody.height(height - 110);
                iframe.height(height - 75);
            },
            stop: function (event, ui) {
                main.extend.resizable = false;
            }
        });
    }

    var isSubmit = false, stop = false;
    main.submit = function (url, data, callback, error) {
        if (isSubmit) {
            return;
        }
        url = getPageUrl(url);
        clearError();
        main.extend.init();
        main.extend.subprogress(0, false);
        isSubmit = true;
        main.extend.alert.hide();
        $.ajax({
            url: url,
            data: data,
            type: "POST",
            cache: false,
            dataType: 'json',
            success: function (code) {
                isSubmit = false;
                main.extend.subprogress(100, true);
                if (code.Header && code.Header.ReturnCode == 0) {
                    main.extend.subprogress(100, false);
                    if (callback != null) {
                        callback(code.Body);
                    }
                }else if (code.Header && code.Header.ReturnCode == -1) {
                    main.extend.alert.find("p").html(code.Header.Message);
                      main.extend.alert.show();
                      errCodes.push( code.Body);
                } else {
                    errCodes.push(code.Body);
                    if (error) {
                        error(code);
                    } else {
                        if (code.Header.PropertyName) {
                            var propertyName = $("#" + code.Header.PropertyName);
                            var type = $(propertyName).attr("type");
                            if (type) {
                                type = type.toLowerCase();
                            }
                            if (type == "checkbox" || type == "radio") {
                                propertyName = propertyName.parent("div");
                            }
                            propertyName.tabShow();
                            propertyName.error(code.Header.Message);
                           
                        }
                        else {
                            main.extend.alert.find("p").html(code.Header.Message);
                            main.extend.alert.show();
                        }
                    }

                }
            },
            error: function (code) {
                main.extend.subprogress(100, true);
                main.extend.alert.find("p").html("亲，调用服务错误！^_^");
                main.extend.alert.show();
                isSubmit = false;
            }
        });

    };
    main.getData = function (url, data, callback, error) {
        url = getPageUrl(url);
        main.extend.init();
        main.extend.alert.hide();
        $.ajax({
            url: url,
            data: data,
            type: "POST",
            cache: false,
            dataType: 'json',
            success: function (code) {
                if (code.Header && code.Header.ReturnCode == 0) {
                    if (callback != null) {
                        callback(code.Body);
                    }
                }
                else if (code.Header && code.Header.ReturnCode == -1) {
                    main.extend.alert.find("p").html(code.Header.Message);
                    main.extend.alert.show();
                    errCodes.push(code.Body);
                }
                else {

                    if (error) {
                        error(code);

                    } else {
                        main.extend.alert.find("p").html(code.Header.Message);
                        main.extend.alert.show();
                    }
                   
                }
            },
            error: function (code) {
                errCodes.push(code.responseText);
                main.extend.alert.find("p").html("亲调用服务失败^_^");
                main.extend.alert.show();

            }
        });

    };
    main.getJsonp = function (url, data, callbackname) {
        url = url || "";
        if (url.indexOf("?") != -1) {
            url = url + "&CallBack=" + callbackname;
        } else {
            url = url + "?CallBack=" + callbackname;
        }
        stop = false;
        init();
        getprogress(0);
        alert.hide();
        $.ajax({
            url: url,
            data: data,
            type: "GET",
            cache: false,
            dataType: 'jsonp',
            jsonp: "",
            success: function (code) {
                stop = true;
                if (code.Header && code.Header.Success) {
                    setprogress(100);
                    if (callback != null) {
                        callback(code.Body);
                    }
                }
                else {
                    alert.find("p").html(code.Header.Message);
                    alert.show();
                }
            }
        });

                        }
                      
    main.getHtml = function (url, data, callback) {
        url = getPageUrl(url);
        main.extend.init();
        main.extend.alert.hide();
        if (!callback) {
            main.extend.getprogress(1, false, true);
        }
        var methods = "GET";
        if (data) {
            methods = "POST";
        }
        $.ajax({
            url: url,
            type: methods,
            cache: false,
            data: data,
            dataType: 'html',
            success: function (html) {
                ishowmodel = false;
                if (!callback) {//无回调函数
                    main.extend.getprogress(100, false);
                    main.isLoading = false;
                }
                if (html.substr(0, 1) == "{") {
                    var json = JSON.parse(html);
                    main.extend.alert.find("p").html(json.Header.Message);
                    main.extend.alert.show();
                    errCodes.push(json.Body);
                    return;
                }
                if (!callback) {
                    $("#contentRenderBody").html(html);
                    if (main.extend.startPage && window.startPage && main.extend.startPageData) {
                        window.startPage(main.extend.startPageData);
                    }
               
                } else {
                    callback(html);//委托出去
                }
            },
            error: function (code) {
                ishowmodel = false;
                if (!callback) {
                    main.extend.getprogress(100, true);
                    main.isLoading = false;
                }
                main.extend.alert.find("p").html("亲，调用服务失败^_^");
                main.extend.alert.show();
            }
        });
    };
})(jQuery);

//元素对象扩展操作及页面加载菜单后的处理
(function ($) {
    var fullShow = true, popoverTemp = null, inputvalid = null, errCodes = new Array(), hisstory = null, clicktemp = null, preobj = null, modelurls = new Array();
    var headerAction = (function (obj) {
        return function () {
            for (var i = 0; i < obj.length; i++) {
                if (fullShow) {
                    $(obj[i]).addClass("collapse-sidebar");
                }
                else {
                    $(obj[i]).removeClass("collapse-sidebar");
                }
            }
        };
    })([$("#auto_layout_header"), $("#autobay_layout_sidebar")]);
    var contentAction = (function (obj) {
        return function () {
            if (fullShow) {
                $(obj).addClass("collapsed-sidebar");
            }
            else {
                $(obj).removeClass("collapsed-sidebar");
            }
        };
    })($("#autobay_layout_content"));
    var init = (function (progress, alert, logheader) {
        return function () {
            progress.width($(document.body).width() - logheader.width() - 20);
            alert.width($(document.body).width() - logheader.width() - 50);
            main.extend.alert = alert;
        };
    })($("#layout_progress_submit"), $("#layout_alert_submit"), $("#autobay_layout_sidebar"));
    main.extend.headerAction = headerAction;
    main.extend.contentAction = contentAction;
    main.extend.init = init;
   
    (function (obj, ul) {
        main.extend.initresize = function () {
            fullShow = !fullShow;
        }
        main.extend.fullshow = function () {
            return fullShow;
        }
        main.extend.initMenu = function () {
            headerAction();
            contentAction();
            fullShow = !fullShow;
            init();
            if (!clicktemp) {
                return;
            }
            if (fullShow) {
                clicktemp.addClass("show").closest("li").addClass("highlight-menu").children("a").first().addClass("expand");
               
            }
            else {
                ul.removeClass("show");
                $(obj).removeClass("highlight-menu").removeClass("expand");
            }
        }
        $(obj).on("click", function () {
            main.extend.initMenu();
            if (fullShow) {
                $("#autobay_layout_sidebar").removeClass("hidden-xs");
        } else {
                $("#autobay_layout_sidebar").addClass("hidden-xs");
            }
            return false;
        });
    })($("#navbar-no-collapse"), $("#autobay_layout_sidebar .side-nav .nav").find(".sub"));
    (function (progress, alert, logheader) {
        var stop = false;
        function subprogress(width, error) {
            if (width == 0) {
                $(progress).hide();
                stop = false;
            }
            else if (error) {
                stop = true;
            }
            if (stop) {
                $(progress).hide();
                //$(progress).find("div").css({ width: "1%" }).html(0 + "%" + "Complete");
                return;
            }
            currentwidth = width;
            currentwidth = currentwidth + 1;
            if (width >= 100) {
                stop = true;
                //$(progress).find("div").css({ width: "95%" }).html(95 + "%" + "Complete");
                setTimeout(function () {
                    $(progress).hide();

                }, 5000);
                return;
            }
            if (currentwidth <= 100 && currentwidth > 0) {
                $(progress).show();
                //$(progress).find("div").css({ width: currentwidth + "%" }).html(currentwidth + "%" + "Complete");
                setTimeout(subprogress, 50, currentwidth);
            }
            else {
                setTimeout(function () {
                    $(progress).hide();
                    //$(progress).find("div").css({ width: "1%" }).html(0 + "%" + "Complete");
                }, 5000);
            }
        }
        main.extend.subprogress = subprogress;
    })($("#layout_progress_submit"), $("#layout_alert_submit"), $("#autobay_layout_sidebar"));
    (function (obj, alert, show) {
        var stop = false, prewidth = 0;
        function getprogress(width, error, start) {
            if (start) {
                stop = false;
            }
            if (width == 0) {
                stop = false;
            }
            if (error) {
                stop = true;
            }
            if (width >= 100) {
              
                stop = true;
            }
            if (stop) {
                show.addClass("hidden");
                return;
            }
            else {
                show.removeClass("hidden");
            }
            currentwidth = width;
            currentwidth = currentwidth + 10;
            if (currentwidth <= 100 && currentwidth > 0) {
                $(obj).css({ "-webkit-transform": "translate3d(" + currentwidth + "%, 0px, 0px)", "transform": "translate3d(" + currentwidth + "%, 0px, 0px)" });
                setTimeout(getprogress, 500, currentwidth);
            }
        }
        main.extend.getprogress = getprogress;

    })($(".pace-progress"), $("#layout_alert_submit"), $(".pace-active"));
    (function (obj) {
        var parent = $(obj).find("a[href='#']");
        var temp = null;
        $(parent).mouseover(function () {
            if (temp != null) {
                $(temp).closest("li").removeClass("hover-li");
            }
            temp = $(this);
            $(temp).closest("li").addClass("hover-li");
        });
        $(document).click(function () {
            $(temp).closest("li").removeClass("hover-li");
        });
        main.gotoPage = function (href, data) {
            href = href.split('?')[0];
            $(obj).find("a").each(function (index, ele) {
                if ($(ele).attr("href") && $(ele).attr("href").indexOf(href) != -1) {
                    main.extend.startPage = true;
                    main.extend.startPageData = data;
                    window.sessionStorage.gotoPage = 1;
                    $(ele).click();
                }
            })
        }
        $(document).ready(function () {
            var href = window.sessionStorage.href;
            if (!href || href=="") {
                href = window.pageHomeUrl;//默认加载页面
            }
            if (href) {//初次加载第一次只有默认页有URL
                $(obj).find("a").each(function (index, ele) {
                    if ($(ele).attr("href") && $(ele).attr("href").indexOf(href) != -1) {
                        var currenthref = $(ele);
                        currenthref.addClass("active");
                        preobj = currenthref;
                        main.getHtml($(ele).attr("href"));//在此处调用getHtml来进行异步请求helf的地址
                        clicktemp = currenthref.closest("ul");
                        clicktemp.addClass("show").closest("li").addClass("highlight-menu").children("a").first().addClass("expand");

                        if (!main.extend.fullShow) {
                            var html = $(this).closest("ul").closest("li").removeClass("hover-li").find("a[href='#']").find("span").html();
                            if (html && html != "") {
                                html = html + "/";
                            }
                            else {
                                html = "";
                            }
                            $("#navbar-no-collapse-currentPage").html(html + $(this).find("span").html());
                        }

                    }
                })
            }
        })
           
        $(obj).find("a").click(function (event) {
            var href = $(this).attr("href");
            if (href != null && href != "#" && href != "") {
                if (main.isLoading) {
                    return false;
                }
                main.isLoading = true;
                main.getHtml(href);
                if (!main.extend.fullshow()) {
                    var html = $(this).closest("ul").closest("li").removeClass("hover-li").find("a[href='#']").find("span").html();
                    if (html && html != "") {
                        html = html + "/";
                    }
                    else {
                        html = "";
                    }
                    $("#navbar-no-collapse-currentPage").html(html + $(this).find("span").html());
                }
                else {
                    $("#navbar-no-collapse-currentPage").html("");
                }
                window.sessionStorage.href = href;
                if (preobj != null) {
                    $(preobj).removeClass("active").removeClass("hover-li");
                    $(preobj).css({ "color": "#FFF" });
                    if (!main.extend.fullshow()) {
                        $(preobj).closest("ul").closest("li").removeClass("highlight-menu").find("a:first-child").removeClass("expand");
                    }
                   
                }
                if (!main.extend.fullshow()) {
                    $(this).closest("ul").closest("li").addClass("highlight-menu").find("a:eq(0)").addClass("expand");
                }
                $(this).addClass("active");
                $(this).css({ "color": "#FE6232" });
                preobj = this;
                hisstory = href;
                event.stopPropagation();
                return false;
            }
            if (fullShow) {
                var thisobj = $(this).next();
                if ($(thisobj).hasClass("show")) {
                    $(thisobj).removeClass("show");
                    $(this).closest("li").removeClass("highlight-menu");
                    $(this).children().first().removeClass("rotate90").addClass("rotate0");
                }
                else {
                    if (clicktemp != null) {
                        $(clicktemp).removeClass("show");
                        $(clicktemp).closest("li").removeClass("highlight-menu");
                        $(clicktemp).prev().removeClass("expand");
                    }
                    $(thisobj).addClass("show");
                    $(this).closest("li").addClass("highlight-menu");
                    $(this).children().first().removeClass("rotate0").addClass("rotate90");
                    $(this).addClass("expand");
                    clicktemp = thisobj;
                }
            }
            event.stopPropagation();
            return false;
        });

    })($("#autobay_layout_sidebar .side-nav .nav"));
    $(function () {
        $(window).on('keydown', function (event) {
            event = event || window.event;
            var code = event.keyCode || event.which;
            if (code == 8 || code == 13) {
                var target = event.target || event.srcElement;
                var tagName = target.tagName;
                switch (tagName.toLowerCase()) {
                    case "input":
                    case "textarea":
                        if ($(target).attr("readonly")) {
                            event.preventDefault();
                            return false;
                        }
                        break;
                    default:
                        event.preventDefault();
                        return false;
                }

            }
            return true;
        });

        $(window).on('click', function(event) {
            event = event || window.event;
            var code = event.keyCode || event.which;
            var target = event.target || event.srcElement;
            var tagName = target.tagName;
            if (!tagName) {
                return true;
            }
            switch (tagName.toLowerCase()) {
            case "button":
                return false;
            case "input":
                if ($(target).attr("type") == "submit") {
                    return false;
                }
                break;
            }

            return true;
        });

        //$("#wrapper").height($(window).height() - 15);
        //$("#contentRenderBody").height($("#wrapper").height() - 40);
        main.extend.initMenu();
        $(".reset-layout").click(function () {
            if (hisstory != null) {
                main.getHtml(hisstory);
            }
            return false;
        });
    });
    $(window).resize(function () {
        if (main.extend.resizable == true) {
            return;
        }
        main.extend.initresize();
        //$("#wrapper").height($(window).height() - 15);
        //$("#contentRenderBody").height($("#wrapper").height() - 40);
        main.extend.initMenu();
    });
})(jQuery);



