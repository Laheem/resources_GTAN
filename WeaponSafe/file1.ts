var cef = null;
var currSafe = null;

class CefHelper {
    constructor(resourcePath) {
        this.path = resourcePath;
        this.open = false;
    }

    show() {
        if (this.open === false) {
            this.open = true;
            const resolution = API.getScreenResolution();
            this.browser = API.createCefBrowser(resolution.Width, resolution.Height, true);
            API.waitUntilCefBrowserInit(this.browser);
            API.setCefBrowserPosition(this.browser, 0, 0);
            API.loadPageCefBrowser(this.browser, this.path);
        }
    }

    destroy() {
        this.open = false;
        API.destroyCefBrowser(this.browser);
        API.showCursor(false);
    }

    eval(string) {
        this.browser.eval(string);
    }
}

let pool = null;

API.onServerEventTrigger.connect(function(eventName, args) {

    if (eventName === "showSafeWeapons") {
        const list = args[0];
        pool = API.getMenuPool();
        const menu = API.createMenu("Weapon List - ", 0, 0, 6);
        API.setMenuTitle(menu, "Weapon List");
        API.setMenuBannerRectangle(menu, 100, 255, 0, 0);
        for (let x = 0; x < list.Count; x++) {
            const parsedObj = JSON.parse(list[x]);
            const wepName = parsedObj.weaponName;
            const button = API.createMenuItem(wepName, ` ${wepName}.`);
            const wep = wepName;
            button.Activated.connect(function(menu, item) {
                API.triggerServerEvent("takeWeapon", wep, x, args[1]);
                menu.Visible = false;

            });
            menu.AddItem(button);
        }

        API.setMenuBannerRectangle(menu, 100, 0, 0, 255);
        pool.Add(menu);
        API.sendChatMessage("The menu should be visable!");
        menu.Visible = true;
    } else if (eventName === "openSafe") {
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

API.onUpdate.connect(function() {
    if (pool != null) {
        pool.ProcessMenus();
    }
});