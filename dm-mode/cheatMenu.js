let menuPool = null;

API.onKeyDown.connect(function(Player, args) {
    if (args.KeyCode == Keys.E && !API.isChatOpen())
    {
        menuPool = API.getMenuPool(); 
        let menu = API.createMenu("Cheat Menu", 0, 0, 6);
        let item1 = API.createMenuItem("Boost Armour to 100", "");
        let item2 = API.createMenuItem("Boost Health", "");
        let item3 = API.createMenuItem("Suicide", "");

        item1.Activated.connect(function(menu, item) {
            API.sendChatMessage("Set Armour Command.");
			API.triggerServerEvent("setArmour",100);
        });
		item2.Activated.connect(function(menu,item){
			API.sendChatMessage("Set Health Command.");
			API.triggerServerEvent("setHealth",100);
		});
		item3.Activated.connect(function(menu,item){
			API.sendChatMessage("Suicide Command");
			API.triggerServerEvent("startDeathMatch")
		});
        
        menu.AddItem(item1);
        menu.AddItem(item2);
        menu.AddItem(item3);
        menuPool.Add(menu);

        menu.Visible = true;            
    }
});

API.onUpdate.connect(function() {
    if (menuPool != null) {
        menuPool.ProcessMenus();
    }
});