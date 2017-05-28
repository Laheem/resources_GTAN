using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

    public class ClothingStore : Script
    {

    public ClothingStore() {
        API.onResourceStart += clothingStoreStart;
        API.onPlayerConnected += lastknownSkin;
    }

    private void lastknownSkin(Client player)
    {
        if (API.hasEntityData(player, "lastSkin"))
        {
            PedHash skin = API.getEntityData(player.handle, "lastskin");
            API.setPlayerSkin(player, skin);
        }
        else
        {
            API.setEntityData(player, "lastSkin", PedHash.Franklin);
            API.setPlayerSkin(player,API.getEntityData(player.handle,"lastSkin"));
        }
    }

    private void clothingStoreStart()
    {
        API.consoleOutput("Ready to look good. Clothing Store started!");
    }

    [Command("changeskin",GreedyArg = true)]
    public void startChangeClothes(Client sender, String potentialSkin)
    {
        potentialSkin = potentialSkin.ToLower();
        PedHash newSkin = API.pedNameToModel(potentialSkin);
        if(newSkin == 0)
        {
            API.sendChatMessageToPlayer(sender, "Invalid skin name, check the GTA:N Wiki for a list of names.");
            return; 
        }
        API.setPlayerSkin(sender, newSkin);
        API.setEntityData(sender, "lastSkin", newSkin);

    }
}
