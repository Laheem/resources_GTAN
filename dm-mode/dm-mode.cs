using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GTANetworkServer;
using GTANetworkShared;
using System.Threading;
using System;

public class DeathMatch : Script
{
    private int x;
    private int ammoNum;
    public DeathMatch()
    {
        API.onClientEventTrigger += OnClientEvent;
        API.onResourceStart += startDm;
    }


    public void OnClientEvent(Client player,string eventName, params object[] args)
    {
        if(eventName == "setHealth")
        {
            setHealth(player, player, args[0].ToString());
        }
        if(eventName == "setArmour")
        {
            setArmour(player, player, args[0].ToString());
        }
        if(eventName == "startDeathMatch")
        {
            startDeathMatch(player);
        }
    }

    public void startDm()
    {
        API.consoleOutput("DM HAS STARTED!");
    }

    [Command("deathmatch")]
    public void startDeathMatch(Client sender)
    {
        API.setPlayerHealth(sender, -1);
    }
    [Command("sethealth", GreedyArg = true)]
    public void setHealth(Client sender, Client target, string amount)
    {
        if (target.exists && Int32.TryParse(amount, out x))
        {
            API.setPlayerHealth(target, x);
            API.sendChatMessageToPlayer(sender, "Health was changed.");
        }
    }
    [Command("setarmour", GreedyArg = true)]
    public void setArmour(Client sender, Client target, string amount)
    {
        if (target.exists && Int32.TryParse(amount, out x))
        {
            API.setPlayerArmor(target, x);
            API.sendChatMessageToPlayer(sender, "Armour was changed.");
        }
    }
    [Command("giveweapon", GreedyArg = true)]
    public void giveWep(Client sender, Client target, string weapon, string ammo)
    {
        WeaponHash givenWep = API.weaponNameToModel(weapon);
        if (target.exists && Int32.TryParse(ammo, out ammoNum)) {
            API.givePlayerWeapon(target, givenWep, ammoNum, false, true);
        }
    }

}

