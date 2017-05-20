let menuPool = null;
let firstPoint = null;
let currPoint = null;
let currMarker = null;
let firstMarker = null;

API.onServerEventTrigger.connect(function (eventName, args) {

	if(eventName === "chooseTest"){
		
		menuPool = API.getMenuPool(); 
        let menu = API.createMenu("Driving Tests available -  ", 0, 0, 6);
		API.setMenuTitle(menu, "DMV");
        let item1 = API.createMenuItem("Car Test","The usual four door, drive as fast as you can, and keep the car in good shape.");
		let item2 = API.createMenuItem("Biker Test","Drive as fast you can, keep the motorbike in good shape. Try not to fly off.");
		let item3 = API.createMenuItem("Trucker Test","Think cars, but bigger. keep itunder control and get back here in time, don't break the cab.");

		
		item1.Activated.connect(function(menu, item) {
			API.triggerServerEvent("Car","public");
			menu.Visible = false;
        });
		
		item2.Activated.connect(function(menu,item){
			API.triggerServerEvent("Bike","public");
			menu.Visible = false;
		});
		
		item3.Activated.connect(function(menu,item){
			API.triggerServerEvent("Trucker","public");
			menu.Visible = false;
		});
		
		menu.AddItem(item1);
        menu.AddItem(item2);
		menu.AddItem(item3);
        menuPool.Add(menu);
		API.setMenuBannerRectangle(menu, 100, 0, 0, 255);

		
		menu.Visible = true;            
	}

	else if(eventName === "placeFirstPoint"){
		firstPoint = API.createBlip(args[0]);
		API.setBlipSprite(firstPoint,198);
		firstMarker = API.createMarker(1,args[0], new Vector3(), new Vector3(), new Vector3(4, 4, 4), 255, 0, 0, 200);


	}
	else if(eventName === "newCheckPoint"){
	if(firstPoint != null){
		API.deleteEntity(firstPoint);
	}
	if(currPoint != null){
		API.deleteEntity(currPoint);
	}
	if(currMarker != null){
		API.deleteEntity(currMarker);
	}
	if(firstMarker != null){
		API.deleteEntity(firstMarker);
	}
	
	currMarker = API.createMarker(1,args[0], new Vector3(), new Vector3(), new Vector3(4, 4, 4), 255, 0, 0, 200);
	currPoint = API.createBlip(args[0]); 
}

	else if(eventName === "cleanUp"){
		API.deleteEntity(currMarker);
		API.deleteEntity(currPoint);
		API.deleteEntity(firstPoint);
		API.deleteEntity(firstMarker);
	}

	
});
API.onUpdate.connect(function() {
    if (menuPool != null) {
        menuPool.ProcessMenus();
    }	
});
