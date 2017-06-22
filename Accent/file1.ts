let pool = null;

API.onServerEventTrigger.connect(function(eventName, args) {

    if (eventName == "accentchoice") {

        const list = args[0];
        pool = API.getMenuPool();
        const menu = API.createMenu("Accents - ", 0, 0, 6);
        API.setMenuTitle(menu, "Accents List`");
        API.setMenuBannerRectangle(menu, 100, 255, 0, 0);
        for (let x = 0; x < list.Count; x++) {
            const parsedObj = JSON.parse(list[x]);
            const accentName = parsedObj.accentName;
            const button = API.createMenuItem(accentName, `The ${accentName} accent.`);
            button.Activated.connect(function(menu, item) {
                API.triggerServerEvent("accentChange", accentName);
                menu.Visible = false;

            });

            menu.AddItem(button);

        }

        API.setMenuBannerRectangle(menu, 100, 0, 0, 255);

        pool.Add(menu);

        menu.Visible = true;
    }


});

API.onUpdate.connect(function() {
    if (pool != null) {
        pool.ProcessMenus();
    }
});