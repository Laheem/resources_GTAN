using System;
using System.Collections.Generic;
using GTANetworkServer;
using GTANetworkShared;

public class WelcomeMsgs : Script
{
	public WelcomeMsgs()
	{
		API.onPlayerConnected += onPlayerConnect;
		API.onPlayerDisconnected += onPlayerDisconnect;
	}

	public void onPlayerConnect(Client player)
	{
		API.sendNotificationToAll("~b~~h~" + player.name + "~h~ ~w~joined.");
    	API.sendChatMessageToAll("~b~~h~" + player.name + "~h~~w~ has joined the server.");
	}

	public void onPlayerDisconnect(Client player, string reason)
	{
		API.sendNotificationToAll("~b~~h~" + player.name + "~h~ ~w~quit.");
    	API.sendChatMessageToAll("~b~~h~" + player.name + "~h~~w~ has quit the server. (" + reason + ")");
	}
}
