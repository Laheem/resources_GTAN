"use strict";
let menuPool = null;
API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "menuOption") {
        menuPool = API.getMenuPool();
        let menu = API.createMenu("Locker - ", 0, 0, 6);
        API.setMenuTitle(menu, "Locker");
        let item1 = API.createMenuItem("Remove an attachment", "Store an attachment in the locker.");
        let item2 = API.createMenuItem("Add an Attachment", "Add an attachment to a weapon you currently have on you.");
        item1.Activated.connect(function (menu, item) {
            API.triggerServerEvent("removeAttachment");
            menu.Visible = false;
        });
        item2.Activated.connect(function (menu, item) {
            API.triggerServerEvent("addAttachment");
            menu.Visible = false;
        });
        menu.AddItem(item1);
        menu.AddItem(item2);
        menuPool.Add(menu);
        API.setMenuBannerRectangle(menu, 100, 0, 0, 255);
        menu.Visible = true;
    }
    else if (eventName == "removeAttachment") {
        menuPool = API.getMenuPool();
        let menu1 = API.createMenu("Locker - ", 0, 0, 6);
        let targettedWep = API.getPlayerCurrentWeapon();
        API.setMenuTitle(menu1, "Locker");
        let menuButtons = [];
        var access = args[0];
        let argsArr = [];
        for (var x in args[0]) {
            argsArr.push(access[x]);
        }
        API.sendChatMessage(argsArr.toString());
        for (let wep of argsArr) {
            var newButton = API.createMenuItem("A", "Choose an attachment to work with.");
            menuButtons.push(newButton);
            newButton.Activated.connect(function (menu1, item) {
                API.removePlayerWeaponComponent(targettedWep, wep);
                menu1.Visible = false;
            });
        }
        let count = menuButtons.length;
        for (let buttons of menuButtons) {
            menu1.AddItem(buttons);
        }
        menuPool.Add(menu1);
        API.setMenuBannerRectangle(menu1, 100, 0, 0, 255);
        menu1.Visible = true;
    }
});
API.onUpdate.connect(function () {
    if (menuPool != null) {
        menuPool.ProcessMenus();
    }
});
