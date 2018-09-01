(function (win, $) {
    var sortName =null, noDataTemp = "<div class=\"bs-example bs-example-standalone\" > <div class=\"alert alert-warning alert-dismissible fade in\"><strong></strong> 没有查到你想要的数据，请重新输入查询条件</div></div>";
    var dataPage = {
        pageindex: 1, sortCol: 0, count: pageGlobal.count, col: "", pagecount: 8, pageSize: 10, tempurl: null, container: "#data_body_info", modelContainer: "#data_body_info", wait: null, waitModel: null, paginContainer: "#data_Pagin_foot", setSearchModel: function (data) { return data; }, rowId: 0,
        first: function () {
            var fun = new dataPageFunction();
            fun.getData({ PageIndex: 1, PageSize: dataPage.pageSize, OrderBy: dataPage.col, SortCol: dataPage.sortCol, Where: dataPage.setSearchModel(main.getModel($("#data_search_info"))) });

        },
        pre: function () {
            var fun = new dataPageFunction();
            dataPage.pageindex = dataPage.pageindex - 1;
            if (dataPage.pageindex <= 1) {
                dataPage.pageindex = 1;
            }
            fun.getData({ PageIndex: dataPage.pageindex, PageSize: dataPage.pageSize, OrderBy: dataPage.col, SortCol: dataPage.sortCol, Where: dataPage.setSearchModel(main.getModel($("#search_par_panel_body_master"))) });

        },
        next: function () {
            var fun = new dataPageFunction();
            dataPage.pageindex = dataPage.pageindex + 1;

            if (dataPage.pageindex > dataPage.count) {
                return;
            }
            fun.getData({ PageIndex: dataPage.pageindex, PageSize: dataPage.pageSize, OrderBy: dataPage.col, SortCol: dataPage.sortCol, Where: dataPage.setSearchModel(main.getModel($("#search_par_panel_body_master"))) });

        },
        end: function () {
            var fun = new dataPageFunction();
            dataPage.pageindex = dataPage.count % dataPage.pageSize == 0 ? dataPage.count / dataPage.pageSize : parseInt((dataPage.count / dataPage.pageSize) + 1);

            fun.getData({ PageIndex: dataPage.pageindex, PageSize: dataPage.pageSize, OrderBy: dataPage.col, SortCol: dataPage.sortCol, Where: dataPage.setSearchModel(main.getModel($("#search_par_panel_body_master"))) });

        },
        sort: function (name, obj) {
            var objName = $(obj).html();
            if ($(obj).hasClass("sorting_asc")) {
                dataPage.sortCol = 1;
            }
            else {
                dataPage.sortCol = 0;
            }
            $(obj).parent("tr").find("th").removeClass("sorting_asc sorting_desc");
            if (dataPage.sortCol == 0) {
                $(obj).removeClass("sorting  sorting_desc").addClass("sorting_asc");
            }
            else {
                $(obj).removeClass("sorting sorting_asc").addClass("sorting_desc");
            }
            sortName = objName;
            var fun = new dataPageFunction();
            dataPage.col = name;
            fun.getData({ PageIndex: dataPage.pageindex, PageSize: dataPage.pageSize, OrderBy: name, SortCol: dataPage.sortCol, Where: dataPage.setSearchModel(main.getModel($("#search_par_panel_body_master"))) });

        },
        search: function (index) {
          
            if (dataPage.pageSize > 200) {
                return;
            }
            if (!parseInt(dataPage.pageindex)) {
                return;
            }
            if (!parseInt(dataPage.pageSize)) {
                return;
            }
            var pageIndexCount = dataPage.count % dataPage.pageSize==0 ? dataPage.count / dataPage.pageSize : dataPage.count / dataPage.pageSize + 1;
            dataPage.pageindex = index || dataPage.pageindex;
            if (dataPage.pageindex > pageIndexCount && dataPage.pageindex > 1) {
                return;
            }
            var fun = new dataPageFunction();
            dataPage.pageSize = parseInt($("#data_page_size").val()) || dataPage.pageSize;
            fun.getData({ PageIndex: dataPage.pageindex, PageSize: dataPage.pageSize, OrderBy: dataPage.col, SortCol: dataPage.sortCol, Where: dataPage.setSearchModel(main.getModel($("#search_par_panel_body_master"))) });

        },
        pagin: function () {
            if (dataPage.count < 0) {
                return "";
            }
            var currentIndexPage = dataPage.count % dataPage.pageSize == 0 ? dataPage.count / dataPage.pageSize : parseInt((dataPage.count / dataPage.pageSize) + 1);
            var lastSize = (dataPage.pageindex == 1 ? dataPage.pageSize : (dataPage.pageindex) * dataPage.pageSize);
            if (lastSize > dataPage.count) {
                lastSize = dataPage.count;
            }
            var pagefirst = dataPage.pageindex % dataPage.pagecount;
            var index = dataPage.pageindex - pagefirst;
            if (pagefirst == 0) {
                index = index - dataPage.pagecount;
            }
            dataPage.pageindex = dataPage.pageindex == 0 ? 1 : dataPage.pageindex;
            index = index == 0 ? 1 : index;
            index = (index >= dataPage.pagecount) ? index + 1 : index;
            var strtemple = "<div class=\"row\">";
            strtemple += " <div class=\"col-lg-3\"  style=\"margin-top:5px;\"><div class=\"pagination\"> 显示 " + (dataPage.pageindex == 1 ? 1 : ((dataPage.pageindex - 1) * dataPage.pageSize + 1)) + " 至 " + lastSize + " 共 " + dataPage.count + " 条</div></div>";
            strtemple += " <div class=\"col-lg-7\">";
            strtemple += "  <nav style=\"float:right\">";
            strtemple += " <ul class=\"pagination\">";
            if (dataPage.pageindex == 1) {
                strtemple += " <li class=\"paginate_button first disabled\"><a href=\"#\">首页</a></li>";
                strtemple += "<li class=\"paginate_button previous disabled\"><a href=\"#\">上一页</a></li>";
            }
            else {
                strtemple += " <li class=\"paginate_button first \"><a href=\"javascript:dataPage.search(1);\">首页</a></li>";
                strtemple += "<li class=\"paginate_button previous \"><a href=\"javascript:dataPage.pre()\">上一页</a></li>";
            }

            var isLastPage = false, isactive = false;
            for (var i = index; i < dataPage.pagecount + index; i++) {
                if ((i == dataPage.pageindex || i == currentIndexPage) && !isactive) {
                    strtemple += "<li class=\"paginate_button active\"><a href=\"javascript:dataPage.search(" + i + ");\">" + i + "</a></li>";
                    isLastPage = i == currentIndexPage;
                    isactive = true;
                }
                else {
                    strtemple += "<li class=\"paginate_button\"><a href=\"javascript:dataPage.search(" + i + ");\">" + i + "</a></li>";
                }

                if (i >= currentIndexPage) {
                    break;
                }
            }

            if (dataPage.pageindex == currentIndexPage || isLastPage || dataPage.count <= dataPage.pageSize) {
                strtemple += "<li class=\"paginate_button  disabled\"><a href=\"#\">下一页 </a></li>";
                strtemple += " <li class=\"paginate_button  disabled\"><a href=\"#\">尾页</a></li>";
            }
            else {
                strtemple += "<li class=\"paginate_button \"><a href=\"javascript:dataPage.next()\">下一页</a></li>";
                strtemple += " <li class=\"paginate_button \"><a href=\"javascript:dataPage.end()\">尾页</a></li>";
            }
            strtemple += " </ul></nav></div>";

            strtemple += "  <div class=\"col-lg-2\">";
            strtemple += "   <form class=\"inline pagination\"><div class=\"form-group\"  style=\"margin-top:20px;\"><div class=\"input-group\"><input type=\"text\"  class=\"form-control\" placeholder=\"请输入页码\" data-gotoPage=\"" + dataPage.pageindex + "\" value=\"" + dataPage.pageindex + "\"  /> <div class=\"input-group-addon\" onclick=\"return dataPage.GotoPage(this)\" style=\"cursor:pointer\">跳转</div></div></div></form></div>";
            return strtemple;
        },
        hide: function () {
            if (dataPage.wait && dataPage.waitModel) {
                $(dataPage.wait).hide();
                $(dataPage.waitModel).hide();
            }
        },
        show: function () {
            dataPage.waitAction();
        },
        Load: function () {
            if (!dataPage.modelContainer) {
                dataPage.modelContainer = $(document);
            }
            var offset = $(dataPage.modelContainer).offset();
            var left = offset ? offset.left : 0;
            var top = offset ? offset.top - 10 : 0;
            var width = $(dataPage.modelContainer).width();
            var height = $(dataPage.modelContainer).height();

            var img = document.createElement("img");
            img.src = "/Content/img/load.gif";
            dataPage.wait = $("<div style=\"z-index:10001; position:absolute;\"></div>").appendTo(dataPage.modelContainer);
            $(dataPage.wait).append(img);
            $(dataPage.wait).hide();
            dataPage.waitModel = $("<div style=\"position:absolute;filter:alpha(opacity=30);-moz-opacity:0.3;opacity:0.3;background-color:silver\"></div>").appendTo(dataPage.modelContainer);
            dataPage.waitModel.hide();
        },
        waitAction: function () {
            if (dataPage.wait && dataPage.waitModel) {
                var offset = $(dataPage.modelContainer).offset();
                var left = offset ? offset.left : 0;
                var top = offset ? offset.top : 0;
                var width = $(dataPage.modelContainer).width();
                var height = $(dataPage.modelContainer).height();
                $(dataPage.wait).css("left", width / 2);
                $(dataPage.wait).css("top", height / 2);
                $(dataPage.waitModel).css("width", width);
                $(dataPage.waitModel).css("height", height + 20);

                $(dataPage.waitModel).css("left", 15);
                $(dataPage.waitModel).css("top", 0);
                $(dataPage.wait).show();
                $(dataPage.waitModel).show();
            }
        },
        checkAll: function (obj) {
            $(obj).closest("table").find("input[type='checkbox']").attr("checked", obj.checked);
        },
        GotoPage: function (obj) {
            if ($(obj).attr("type") == "text") {
                var page = parseInt(obj.value);
                dataPage.pageindex = page;
                dataPage.search();
            } else {
                var value = $(obj).prev().val();
                var page = parseInt(value);
                dataPage.pageindex = page;
                dataPage.search();

            }

            return true;
        },
        GotoSize: function (obj) {
            if ($(obj).attr("type") == "text") {
                var page = parseInt(obj.value);
                dataPage.pageSize = page;
                dataPage.pageindex = 1;
            } else {
                dataPage.pageSize = obj;
                $("#data_page_size").val(obj);
                dataPage.search(1);
            }
            return true;
        },
        reset: function () {
            $("#search_par_panel_body_master").find("input").each(function (index, element) {
                var type = $(element).attr("type").toLowerCase();
                type = (type ? type : "");
                var issimple = form.find("input[name='" + $(element).attr("name") + "']").length == 1;
                switch (type) {
                    case "checkbox":
                    case "radio":
                        element.checked = false;
                        break;
                    case "button":
                    case "submit":
                        break;
                    default:
                        $(element).val("");
                        break;
                }
            });
        },
        remove: function (count) {
            //最后一页删除数据时，判定页索引是否需要回退
            if (dataPage.pageindex > 1 && typeof count === "number") {
                dataPage.count -= count;
                if ((dataPage.count / dataPage.pageSize + 1) <= dataPage.pageindex) {
                    dataPage.pageindex--;
                }
            }
            this.search();
        }
    }


    function dataPageFunction() {
        return this;
    }

    dataPageFunction.prototype = {
        getTemp: function (parameters) {

            main.getHtml(pageGlobal.dataUrl, parameters, function (html) {
                window.sessionStorage.gotoPage = 0;
                $(dataPage.container).html(html);
                if (dataPage.count == 0) {
                    var colspan = $(dataPage.container).find("th").length;
                    $(dataPage.container).find("tbody").html("<tr> <td colspan=\"" + colspan + "\" style=\"text-align:center\"> " + noDataTemp + "</td></tr>");
                    $(dataPage.paginContainer).html("");
                }
                else {
                    $(dataPage.paginContainer).html(dataPage.pagin()).find("input[data-gotoPage]").bind("keydown", function (event) {
                        if (event.which == 13) {
                            dataPage.GotoPage(this);
                            event.stopPropagation();
                            return false;
                        }
                        return true;
                    });
                }
                dataPage.hide();
                dataPage.Load();
                if (sortName) {
                    $(dataPage.container).find("th").each(function (index, element) {
                        if ($(element).html() == sortName) {
                            if (dataPage.sortCol == 0) {
                                $(element).removeClass("sorting sorting_desc").addClass("sorting_asc");
                            }
                            if (dataPage.sortCol == 1) {
                                $(element).removeClass("sorting  sorting_asc").addClass("sorting_desc");
                            }
                        } else {
                            if ($(element).hasClass("sorting_desc") || $(element).hasClass("sorting_asc")) {
                                $(element).removeClass("sorting_asc sorting_desc").addClass("sorting");
                            }
                        }
                    });
                }
               
            })
        },
        getData: function (data) {
            dataPage.show();
            this.getTemp(data);
        }
    }
    win.dataPage = dataPage;
    (function () {
       
        dataPage.Load();
        var data = new dataPageFunction();
        if (pageGlobal.searchUrl) {//不是区域模板不传递查询区域
           
            main.getHtml(pageGlobal.searchUrl, null, function (html) {
               //在模板里定义 var strHtml = "  <div class='panel panel-default'><div class='panel-heading'><a data-toggle='collapse' href='#search_par_panel_body_master'> <h5 class='panel-title'><i class='fa fa-unsorted fa-fw'></i> 查询选项</h5></a></div><div class='panel-body panel-collapse collapse in' id='search_par_panel-body_master'><div class='row'>" + html + "</div></div></div>";
                $("#data_search_info").html(html);
                 data.getData({ PageIndex: 1, PageSize: dataPage.pageSize, OrderBy: dataPage.col, SortCol: dataPage.sortCol, Where: dataPage.setSearchModel(main.getModel($("#search_par_panel_body_master"))) });
            });
        }
        else {
            if (window.sessionStorage.gotoPage == 1) {
                window.sessionStorage.gotoPage = 0;
            } else {
                data.getData({ PageIndex: 1, PageSize: dataPage.pageSize, OrderBy: dataPage.col, SortCol: dataPage.sortCol, Where: dataPage.setSearchModel(main.getModel($("#search_par_panel_body_master"))) });
            }
        }
        $(document.body).unbind("keydown");
        $(document.body).bind("keydown", function (event) {
            if (event.which == 13) {
                 dataPage.search(1);
                return false;
            }
            return true;
        })
    })();
})(window, jQuery);