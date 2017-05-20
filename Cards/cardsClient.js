let menuPool = null;

API.onServerEventTrigger.connect(function (eventName, args) {

	if(eventName == "drawACard"){
		
		menuPool = API.getMenuPool(); 
        let menu = API.createMenu("Dealer Choice - ", 0, 0, 6);
		API.setMenuTitle(menu, "Dealer");
        let item1 = API.createMenuItem("Private Draw","Draw a card in private, this will leave the deck!");
		let item2 = API.createMenuItem("Public Draw","Publically draw a card, this will leave the deck!");
		let item3 = API.createMenuItem("Shuffle","Bring back all the cards into the deck.");
		let item4 = API.createMenuItem("Reveal","Reveal all cards that have left the deck.");
		
		item1.Activated.connect(function(menu, item) {
			API.triggerServerEvent("startDraw","private");
			menu.Visible = false;
        });
		
		item2.Activated.connect(function(menu,item){
			API.triggerServerEvent("startDraw","public");
			menu.Visible = false;
		});
		
		item3.Activated.connect(function(menu,item){
			API.triggerServerEvent("shuffleCards");
			menu.Visible = false;
		});
		item4.Activated.connect(function(menu,item){
			API.triggerServerEvent("revealCards");
			menu.Visible = false;
		});
		
		
		menu.AddItem(item1);
        menu.AddItem(item2);
		menu.AddItem(item3);
		menu.AddItem(item4);
        menuPool.Add(menu);
		API.setMenuBannerRectangle(menu, 100, 0, 0, 255);

		
		menu.Visible = true;            
	}

});
API.onUpdate.connect(function() {
    if (menuPool != null) {
        menuPool.ProcessMenus();
    }
});

		