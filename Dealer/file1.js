var pool = null;
API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "dealerMenu") {
        pool = API.getMenuPool();
        var menu = API.createMenu("Dealer Choice - ", 0, 0, 6);
        API.setMenuTitle(menu, "Dealer");
        var item1 = API.createMenuItem("Deal to Player", "NOT WORKING CORRECTLY, USE COMMANDS.");
        var item2 = API.createMenuItem("Invite Player", "NOT WORKING CORRECTLY, USE COMMANDS!");
        var item3 = API.createMenuItem("Shuffle", "Bring back all the cards into the deck.");
        var item4 = API.createMenuItem("Reveal", "Reveal all players hands!");
        var item5 = API.createMenuItem("Deal To Board", "Deal a card to the board.");
        item1.Activated.connect(function (menu, item) {
            API.triggerServerEvent("dealList");
            menu.Visible = false;
        });
        item2.Activated.connect(function (menu, item) {
            API.triggerServerEvent("inviteList");
            menu.Visible = false;
        });
        item3.Activated.connect(function (menu, item) {
            API.triggerServerEvent("shuffle");
            menu.Visible = false;
        });
        item4.Activated.connect(function (menu, item) {
            API.triggerServerEvent("revealCards");
            menu.Visible = false;
        });
        item5.Activated.connect(function (menu, item) {
            API.triggerServerEvent("dealBoard");
            menu.Visible = false;
        });
        menu.AddItem(item1);
        menu.AddItem(item2);
        menu.AddItem(item3);
        menu.AddItem(item4);
        menu.AddItem(item5);
        pool.Add(menu);
        API.setMenuBannerRectangle(menu, 100, 0, 0, 255);
        menu.Visible = true;
    }
    else if (eventName == "listedPlayers") {
        pool = API.getMenuPool();
        var menu1 = API.createMenu("Player List - ", 0, 0, 6);
        API.setMenuTitle(menu1, "Dealer");
        var list = args[0];
        for (var p in list) {
            var item = API.createMenuItem(p, " Choose a player.");
            item.Activated.connect(function (menu1, item) {
                API.triggerServerEvent("dealCard", p);
            });
            menu1.AddItem(item);
        }
        pool.Add(menu1);
        API.setMenuBannerRectangle(menu1, 100, 0, 0, 255);
        menu1.Visible = true;
    }
    else if (eventName == "possiblePlayers") {
        pool = API.getMenuPool();
        var menu2 = API.createMenu("Player List - ", 0, 0, 6);
        API.setMenuTitle(menu2, "Dealer");
        for (var p in API.getAllPlayers()) {
            var item = API.createMenuItem(p.toString(), " Choose a player to invite to the game.");
            item.Activated.connect(function (menu1, item) {
                API.triggerServerEvent("sendInvite", p);
            });
            menu2.AddItem(item);
        }
        pool.Add(menu2);
        API.setMenuBannerRectangle(menu2, 100, 0, 0, 255);
        menu2.Visible = true;
    }
    else if (eventName == "invite") {
        pool = API.getMenuPool();
        var menu = API.createMenu("Invite Alert! - ", 0, 0, 6);
        API.setMenuTitle(menu, "Invite Recieved ");
        var item1 = API.createMenuItem("Accept", "Accept the invite");
        var item2 = API.createMenuItem("Decline", "Decline the invite.");
        item1.Activated.connect(function (menu, item) {
            API.triggerServerEvent("invite", true, args[0]);
            menu.Visible = false;
        });
        item2.Activated.connect(function (menu, item) {
            API.triggerServerEvent("invite", false, args[0]);
            menu.Visible = false;
        });
        pool.Add(menu);
        menu.AddItem(item1);
        menu.AddItem(item2);
        API.setMenuBannerRectangle(menu, 100, 0, 0, 255);
        menu.Visible = true;
    }
    else if (eventName == "playerMenu") {
        pool = API.getMenuPool();
        var menu = API.createMenu("Player Menu - ", 0, 0, 6);
        API.setMenuTitle(menu, "Player");
        var item1 = API.createMenuItem("Peek at Cards", "Have a glance at your cards.");
        var item2 = API.createMenuItem("Leave the game", "Throw away your cards, leave the game.");
        var item3 = API.createMenuItem("Fold", "Know when it's time to give up. Fold your hand.");
        var item4 = API.createMenuItem("Peek at the Board", "Have a look at the board.");
        item1.Activated.connect(function (menu, item) {
            API.triggerServerEvent("peek");
            menu.Visible = false;
        });
        item2.Activated.connect(function (menu, item) {
            API.triggerServerEvent("leave");
            menu.Visible = false;
        });
        item3.Activated.connect(function (menu, item) {
            API.triggerServerEvent("fold");
            menu.Visible = false;
        });
        item4.Activated.connect(function (menu, item) {
            API.triggerServerEvent("peekboard");
            menu.Visible = false;
        });
        pool.Add(menu);
        menu.AddItem(item1);
        menu.AddItem(item2);
        menu.AddItem(item3);
        menu.AddItem(item4);
        API.setMenuBannerRectangle(menu, 100, 0, 0, 255);
        menu.Visible = true;
    }
});
API.onUpdate.connect(function () {
    if (pool != null) {
        pool.ProcessMenus();
    }
});
