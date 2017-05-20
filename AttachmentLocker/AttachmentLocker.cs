using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

    public class AttachmentLocker : Script
    {

    public AttachmentLocker()
    {
        API.onResourceStart += attachmentLockerStart;
        API.onClientEventTrigger += gunMenuActivate;
    }

    private void gunMenuActivate(Client sender, string eventName, params object[] arguments)
    {
        WeaponComponent[] allAttach = API.getPlayerWeaponComponents(sender, sender.currentWeapon);
        List<WeaponComponent> curAttach = allAttach.ToList();
        switch (eventName)
        {
            case "removeAttachment":
                API.triggerClientEvent(sender, "removeAttachment", curAttach);
                break;
            case "addAttachment":
                API.triggerClientEvent(sender, "listWeapons", curAttach);
                break;
            default:
                break;
        }
    }

    private void attachmentLockerStart()
    {
        API.consoleOutput("Just how many sights can you add to a gun!? Attachment Locker is ready!");
    }

    [Command("locker")]
    public void startLocker(Client sender)
    {
        API.triggerClientEvent(sender, "menuOption");
    }


    public void storeLocker(Client sender, WeaponComponent part)
    {

    }

    public void takeLocker(Client sender, WeaponComponent part)
    {

    }

    [Command("attach")]
    public void giveAllAttachments(Client sender)
    {
        WeaponHash w = sender.currentWeapon;
        WeaponComponent[] all = API.getAllWeaponComponents(w);
        foreach(WeaponComponent attach in all)
        {
            API.givePlayerWeaponComponent(sender, w, attach);
        }
    }
    
}
