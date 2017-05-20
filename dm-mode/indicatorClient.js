API.onKeyDown.connect(function(sender,left){
	if(left.KeyCode === Keys.O && API.isPlayerInAnyVehicle(sender) && !API.isChatOpen()){
		if(API.getPlayerVehicleSeat(sender) == -1){
			API.triggerServerEvent("startIndicatorLeft",sender);
		}
	});
API.onKeyDown.connect(function(sender,right){
	if(right.KeyCode == Keys.P && API.isPlayerInAnyVehicle(sender) && !API.isChatOpen()){
		if(API.getPlayerVehicleSeat(sender) == -1){
			API.triggerServerEvent("startIndicatorRight",sender);
		}
	});
	
		
		
DM IS WHEN 