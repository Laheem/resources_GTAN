using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;

namespace TheftFighter
{
    public class ServerFighter : Script
    {


        private List<Arena> allArenas = new List<Arena>();


        public ServerFighter()
        {
            API.onResourceStart += fighterStart;
            API.onPlayerConnected += setFighterData;
            API.onPlayerDisconnected += checkForFight;
            API.onClientEventTrigger += beginFight;
        }

        private void beginFight(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                case "fightStart":
                    var opponent = (Client) arguments[0];
                    List<Client> fighters = new List<Client>();
                    fighters.Add(sender);
                    fighters.Add(opponent);
                    setFightElements(fighters);

                    break;
                default:
                    break;
            }
        }

        private void checkForFight(Client player, string reason)
        {
            if (API.getEntitySyncedData(player, "inFight"))
            {
                Client target = API.getEntitySyncedData(player, "enemy");
                API.sendNotificationToPlayer(target,"Your opponent has quit the fight.");
                API.setEntitySyncedData(target, "inFight", false);
                API.setEntitySyncedData(target, "enemy", null);
                // TODO : Teleport player away
            }
        }

        private void setFighterData(Client player)
        {
            API.setEntitySyncedData(player, "inFight", false);
            API.setEntitySyncedData(player, "enemy", null);
        }

        private void fighterStart()
        {
            API.consoleOutput("HADOUKEN! Theft Fighter has started");
        }


        public void startFight(Client sender, Client target)
        {
            API.triggerClientEvent(target,"requestFight",sender);
            API.sendNotificationToPlayer(target, API.getPlayerNametag(sender) + " has challenged you to a fight!");
            API.sendNotificationToPlayer(sender,"Invite sent.");
        }


        private void setFightElements(List<Client> fighters)
        {
            Client fighter1 = fighters.ElementAt(0);
            Client fighter2 = fighters.ElementAt(1);
            Arena a = findFreeArena(fighter1);
            if (a == null)
            {
                API.sendNotificationToPlayer(fighter1,"Unable to find a free arena, try later.");
                API.sendNotificationToPlayer(fighter2, "Unable to find a free arena, try later.");
                return;
            }

            movePlayers(a, fighters);


            API.freezePlayer(fighter1,true);
            API.freezePlayer(fighter2,true);

            API.triggerClientEvent(fighter1, "preFight", a.cameraLoc, fighter2);
            API.triggerClientEvent(fighter2, "preFight", a.cameraLoc, fighter1);

            API.freezePlayer(fighter1, false);
            API.freezePlayer(fighter2, false);


            API.setEntitySyncedData(fighter1, "inFight", true);
            API.setEntitySyncedData(fighter1, "enemy", fighter2);
            API.setEntitySyncedData(fighter2, "inFight", true);
            API.setEntitySyncedData(fighter2, "enemy", fighter1);
            API.removeAllPlayerWeapons(fighter1);
            API.removeAllPlayerWeapons(fighter2);


        }

        private Arena findFreeArena(Client sender)
        {
            float dist = float.MaxValue; // v big value
            Arena target = null;
            foreach (Arena a in allArenas)
            {
                if (dist > a.cameraLoc.DistanceTo(API.getEntityPosition(sender)) && a.isFree)
                {
                    target = a;
                }
            }
            return target;
        }

        private void movePlayers(Arena a, List<Client> fighters)
        {
            API.moveEntityPosition(fighters.ElementAt(0),a.spawnPt.ElementAt(0),1);
            API.moveEntityPosition(fighters.ElementAt(1),a.spawnPt.ElementAt(1),1);
            a.addFighters(fighters);
        }
    }
}
