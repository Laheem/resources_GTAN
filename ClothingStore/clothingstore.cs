using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

    public class ClothingStore : Script
    {

    public ClothingStore() {
        API.onResourceStart += clothingStoreStart;
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
        API.setPlayerDefaultClothes(sender);
    }
}
