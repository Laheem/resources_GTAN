let menuPool = null;
var blipPoint = null;

API.onServerEventTrigger.connect(function (eventName, args) {

	if(eventName == "armsDealerTriggered"){
		
		menuPool = API.getMenuPool(); 
        let menu = API.createMenu("Material Choice - ", 0, 0, 6);
		API.setMenuTitle(menu, "Arms Dealer");
        let item1 = API.createMenuItem("Street Materials", "Low quality, but reliable. They'll do for pistols and melee.");
        let item2 = API.createMenuItem("Standard Materials", "Reliable. Will make most civilian weapons, and machine guns.");
        let item3 = API.createMenuItem("Military Materials", "High quality, will make most rifles.");
		
		item1.Activated.connect(function(menu, item) {
			API.sendChatMessage("Street Command.");
			API.triggerServerEvent("startMatRun","street");
			menu.Visible = false;
        });
		
		item2.Activated.connect(function(menu,item){
			API.sendChatMessage("Standard Command.");
			API.triggerServerEvent("startMatRun","standard");
			menu.Visible = false;
		});
		item3.Activated.connect(function(menu,item){
			API.sendChatMessage("Military Mats Command");
			API.triggerServerEvent("startMatRun","military")
			menu.Visible = false;
		});
		
		
		menu.AddItem(item1);
        menu.AddItem(item2);
        menu.AddItem(item3);
        menuPool.Add(menu);
		
		menu.Visible = true;            
	}

	if(eventName == "makeBlip"){
	    matsPoint = API.createBlip(args[0]);
		API.setBlipSprite(matsPoint,514);
		API.setBlipName(matsPoint,"Material Pickup");
		API.sendChatMessage(args[0].toString());
		API.createMarker(1,args[0], new Vector3(), new Vector3(), new Vector3(4, 4, 4), 255, 0, 0, 100);

	}
	if(eventName == "deleteBlip"){
		API.deleteEntity(matsPoint);
	}
});
API.onUpdate.connect(function() {
    if (menuPool != null) {
        menuPool.ProcessMenus();
    }
});

// TODO : Allow only one /getmats at a time.