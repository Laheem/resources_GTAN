var CefHelper = (function () {
    function CefHelper(resourcePath) {
        this.path = resourcePath;
        this.open = false;
    }
    CefHelper.prototype.show = function () {
        if (this.open === false) {
            this.open = true;
            var resolution = API.getScreenResolution();
            this.browser = API.createCefBrowser(resolution.Width, resolution.Height, true);
            API.waitUntilCefBrowserInit(this.browser);
            API.setCefBrowserPosition(this.browser, 0, 0);
            API.loadPageCefBrowser(this.browser, this.path);
        }
    };
    CefHelper.prototype.destroy = function () {
        this.open = false;
        API.destroyCefBrowser(this.browser);
        API.showCursor(false);
    };
    CefHelper.prototype.eval = function (string) {
        this.browser.eval(string);
    };
    return CefHelper;
}());
var cef = null;
API.onServerEventTrigger.connect(function (eventName, args) {
    API.setCefDrawState(true);
    API.sendChatMessage("done");
    API.showCursor(true);
    API.setCanOpenChat(false);
    cef = new CefHelper("test.html");
    cef.show();
});
API.onResourceStop.connect(function () {
    cef.destroy();
});
function ReturnDataValues(data) {
    cef.destroy();
    API.setCefDrawState(false);
    API.showCursor(false);
    API.setCanOpenChat(true);
    API.sendNotification("Data received. " + data);
}
;
