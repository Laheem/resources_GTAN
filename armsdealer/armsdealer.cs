using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;
using System.IO;

public class ArmsDealer : Script
{
    private readonly static Vector3 ARMS_DEALER_LOCATION = new Vector3(841.0, 1280.0,359.0);
    private readonly static Vector3 MAT_POINT_TEST = new Vector3(794.1194, 1205.838, 339.4802);
    private Rectangle2DColShape ArmsDealerPoint;
    private bool playerHasGotBox;
    private bool matRunStarted;

    public ArmsDealer()
    {
        API.onResourceStart += startArmsDealerModule;
        API.onClientEventTrigger += clientSelect;
        API.onEntityEnterColShape += giveMats;
        Rectangle2DColShape ArmsDealerPoint = API.create2DColShape(ARMS_DEALER_LOCATION.X, ARMS_DEALER_LOCATION.Y, 5.0f, 5.0f);
        bool playerHasGotBox = false;
        bool matRunStarted = false;
    }

    private void giveMats(ColShape boxPoint, NetHandle entity)
    {
        var player = API.getPlayerFromHandle(entity);
        

        if(player == null || !matRunStarted)
        {
            return;
        }
        if (!boxPoint.Equals(ArmsDealerPoint) && !playerHasGotBox)
        {
            API.sendChatMessageToPlayer(player, "You got the mats, good work. Now get back to the dealer.");
            API.sendChatMessageToAll(boxPoint.ToString());
            API.triggerClientEvent(player, "deleteBlip");
            API.deleteColShape(boxPoint);
            playerHasGotBox = true;
        }
        else if(playerHasGotBox)
        {
            API.sendChatMessageToPlayer(player, "Nice work. Here are your mats.");
            playerHasGotBox = false;
            matRunStarted = false;
        }
            
    }


    private void clientSelect(Client sender, string eventName, params object[] arguments)
    {
        startMatRun(sender, arguments[0].ToString());
    }

    private void startArmsDealerModule()
    {
        API.consoleOutput("Ready to run the cartel. Arms dealer module is ready!");
        API.createMarker(1, ARMS_DEALER_LOCATION, new Vector3(), new Vector3(), new Vector3(4,4,4),255, 255, 0, 0);
        Blip armsDealerBlip = API.createBlip(ARMS_DEALER_LOCATION, 0);
        API.setBlipName(armsDealerBlip, "Arms Dealer Job");
        API.setBlipSprite(armsDealerBlip, 110);
        


    }

    [Command("getmats")]
    public void startGetMats(Client sender)
    {

        if (sender.position.DistanceTo(ARMS_DEALER_LOCATION) < 5)
        {
            matRunStarted = true;
            API.triggerClientEvent(sender, "armsDealerTriggered");
        }
        else
            API.sendChatMessageToPlayer(sender, "You're not at the /getmats point.");
    }

    public void startMatRun(Client sender, String matType)
    {
        API.sendChatMessageToPlayer(sender, "Here, get to this spot. There's a drop off there. You'll find your " + matType + " mats there.");
        API.triggerClientEvent(sender, "makeBlip",MAT_POINT_TEST);

        Rectangle2DColShape boxPoint = API.create2DColShape(MAT_POINT_TEST.X, MAT_POINT_TEST.Y, 5.0f, 5.0f);

    }

    [Command("whereamI")]
    public void startWhereAmI(Client sender)
    {
        Vector3 senderPos = API.getEntityPosition(sender);
        API.sendChatMessageToPlayer(sender, " X : " + senderPos.X + " Y :" + senderPos.Y + " Z : " + senderPos.Z);
        string filePath = @"C:\Users\Leuma\Documents\allPoints.txt";
        string textToAdd = "new Vector3(" + senderPos.X + "," + senderPos.Y + "," + senderPos.Z + ")";
        using (StreamWriter fs = File.AppendText(filePath))
        {
            fs.WriteLine(textToAdd);
            fs.Close();
        }



    }


}