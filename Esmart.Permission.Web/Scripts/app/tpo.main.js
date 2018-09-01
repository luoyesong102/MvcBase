var tpo = tpo || { version: "1.0" };

//弹出警告信息框
tpo.alert = function (message) {
    bootbox.alert(message);
};

//弹出信息确认框
tpo.confirm = function (message, callback) {
    bootbox.confirm({
        size: 'small',
        className: "modal-confirm",
        message: message,
        callback: callback
    });
};

(function ($) {

    //为表单附加验证引擎
    $.fn.tpoAttachValidate = function () {
        return this.each(function () {
            var $this = $(this);
            if (!$this || !$this.length || !$this.is("form")) {
                alert("$form参数不正确");
                return;
            }
            $this.validationEngine('attach', {
                promptPosition: "centerRight",
                validationEventTrigger: "blur",
                validateNonVisibleFields: true
            });
        });
    };

    //为带tab的表单附加验证引擎
    $.fn.tpoAttachValidateWithTabs = function () {
        return this.each(function () {
            var $this = $(this);
            if (!$this || !$this.length || !$this.is("form")) {
                alert("$form参数不正确");
                return;
            }

            var tab_panes = [], error_pane = null;

            $this.find('a[data-toggle="tab"]').each(function () {
                tab_panes[$(this).attr("href")] = this;
            }).on('shown.bs.tab', function (e) {
                $this.validationEngine('updatePromptsPosition');
            });

            $this.validationEngine('attach', {
                promptPosition: "centerRight",
                validationEventTrigger: "blur",
                validateNonVisibleFields: true
            });

            $this.bind('jqv.form.validating', function (event) {
                error_pane = null;
            });

            $this.find("input").bind('jqv.field.result', function (event, field, isError, promptText) {
                if (isError) {
                    if (error_pane) return;
                    error_pane = $(this).parents(".tab-pane:first");
                    if (error_pane.length) {
                        $(tab_panes['#' + error_pane.attr("id")]).tab("show");
                        error_pane = true;
                    }
                } else {
                    $(this).prev().remove();
                }
            });
        });
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

    $.fn.tpoPopover = function (options) {

        options = $.extend({}, { container: "#contentRenderBody", html: "" }, options);

        var popover = this.find('a[data-toggle="popover"]'), current;

        if (!popover.length) return;

        if (typeof options.html === "string") {
            options.html = $.trim(options.html);
        }
        else if (typeof options.html != "function") {
            options.html = null;
        }

        if (!options.html) {
            popover.addClass("disabled");
            return;
        }

        options.container = $.trim(options.container);
        if (!options.container) {
            options.container = "body";
        }

        if (typeof options.onShow != "function") {
            options.onShow = function () { };
        }

        $(options.container).click(function (event) {
            event = event || window.event;
            var element = event.target || event.srcElement;
            if (!$(element).attr("data-toggle")) {
                element = $(element).parent("a[data-toggle='popover']");
                if (!element.length) {
                    current = null;
                    popover.popover("destroy");
                }
            }
        });

        popover.each(function () {
            var obj = this;
            $(this).click(function (event) {
                event.preventDefault();
                event.stopPropagation();
                popover.popover("destroy");
                if (current == obj) {
                    current = null;
                    return;
                }
                current = obj;
                var html = options.html;
                if (typeof html === "function") {
                    html = html(obj);
                }
                $(obj).popover({
                    container: options.container,
                    content: html,
                    html: true
                }).popover("show");
                options.onShow(obj);
                $(obj).off("click.popover");
            });
        });
    };

})(jQuery);