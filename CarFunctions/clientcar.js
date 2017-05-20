API.onKeyDown.connect(function(Player,args){
	test = API.getLocalPlayer();
	if(args.KeyCode === Keys.O && !API.isChatOpen()){
		API.triggerServerEvent("startIndicatorLeft",Player);
		}
});

API.onKeyDown.connect(function(Player,args){
	test = API.getLocalPlayer();
	if(args.KeyCode == Keys.P && !API.isChatOpen()){
			API.triggerServerEvent("startIndicatorRight",Player);
		}
});