var pool = null;
API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName === "accentchoice") {
        var list = args[0];
        pool = API.getMenuPool();
        var menu = API.createMenu("Accents - ", 0, 0, 6);
        API.setMenuTitle(menu, "Accents List`");
        API.setMenuBannerRectangle(menu, 100, 255, 0, 0);
        var _loop_1 = function (x) {
            var parsedObj = JSON.parse(list[x]);
            var accentName = parsedObj.accentName;
            var button = API.createMenuItem(accentName, "The " + accentName + " accent.");
            button.Activated.connect(function (menu, item) {
                API.triggerServerEvent("accentChange", accentName);
                menu.Visible = false;
            });
            menu.AddItem(button);
        };
        for (var x = 0; x < list.Count; x++) {
            _loop_1(x);
        }
        API.setMenuBannerRectangle(menu, 100, 0, 0, 255);
        pool.Add(menu);
        menu.Visible = true;
    }
});
API.onUpdate.connect(function () {
    if (pool != null) {
        pool.ProcessMenus();
    }
});
