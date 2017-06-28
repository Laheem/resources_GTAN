var cef = null;
var currSafe = null;
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
var pool = null;
API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "showSafeWeapons") {
        var list = args[0];
        pool = API.getMenuPool();
        var menu = API.createMenu("Weapon List - ", 0, 0, 6);
        API.setMenuTitle(menu, "Weapon List");
        API.setMenuBannerRectangle(menu, 100, 255, 0, 0);
        var _loop_1 = function (x) {
            var parsedObj = JSON.parse(list[x]);
            var wepName = parsedObj.weaponName;
            var button = API.createMenuItem(wepName, " " + wepName + ".");
            var wep = wepName;
            button.Activated.connect(function (menu, item) {
                API.triggerServerEvent("takeWeapon", wep, x, args[1]);
                menu.Visible = false;
            });
            menu.AddItem(button);
        };
        for (var x = 0; x < list.Count; x++) {
            _loop_1(x);
        }
        API.setMenuBannerRectangle(menu, 100, 0, 0, 255);
        pool.Add(menu);
        API.sendChatMessage("The menu should be visable!");
        menu.Visible = true;
    }
    else if (eventName === "openSafe") {
        API.setCefDrawState(true);
        API.sendChatMessage("done");
        API.showCursor(true);
        API.setCanOpenChat(false);
        cef = new CefHelper("safe.html");
        cef.show();
        currSafe = args[0];
    }
});
function Password(attemptStr) {
    API.triggerServerEvent("checkPass", currSafe, attemptStr);
    cef.destroy();
    API.setCefDrawState(false);
    API.showCursor(false);
    API.setCanOpenChat(true);
    currSafe = null;
}
API.onUpdate.connect(function () {
    if (pool != null) {
        pool.ProcessMenus();
    }
});
