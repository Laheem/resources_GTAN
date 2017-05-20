#define AMMO 0x7FFFFFFF


public OnPlayerCommandText(playerid,cmdtext[])
{
	new cmd[30], params[50];
	sscanf(cmdtext, "ss",cmd,params);
	
	if(!strcmp(cmd, "/teleport"))
	{
		new id;
		new Float:x, Float:y, Float:z;
		sscanf(params,"d",id);
		GetPlayerPos(playerid, x, y, z);
		if(SetPlayerPos(id,x,y,z) == 0){
			return 0;
		}
		SetPlayerInterior(id,GetPlayerInterior(id));
		return 1;
	}
	if(!strcmp(cmd, "/give")){
		new weaponId;
		sscanf(params, "d",weaponId);
		if(weaponId > 46){
			return 0;
		}
		GivePlayerWeapon(playerid,weaponId,AMMO);
		return 1;
	}
	
	return 0;
}
		
		