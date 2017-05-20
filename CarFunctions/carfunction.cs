using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

namespace CarFunctions
{
    public class CarFunction : Script
    {
        public CarFunction()
        {
            API.onResourceStart += startCarFunction;
            API.onClientEventTrigger += callClientFunction;
        }

        private void callClientFunction(Client sender, string eventName, params object[] arguments)
        {
         
            switch (eventName) {
                case "startIndicatorLeft":
                    startIndicateLeft(sender);
                    break;
                case "startIndicatorRight":
                    startIndicateRight(sender);
                    break;
                default:
                    break;
           }

        }

        private void startCarFunction()
        {
            API.consoleOutput("Car functions started! BEEP BEEP.");
        }

        [Command("spawncar")]
        public void startSpawnCar(Client sender)
        {
            VehicleHash bike = API.vehicleNameToModel("Bati2");
            Vector3 angle = new Vector3(0.0, 0.0, 0.0);
            API.createVehicle(bike, sender.position, angle, 0, 0);
            API.sendChatMessageToAll("BIKEEEEE");
        }

        [Command("night")]
        public void setNight(Client sender)
        {
            API.sendChatMessageToAll("Spooky.");
            API.setTime(23, 15);
        }
        
        public void startIndicateLeft(Client sender)
        {
            List<Client> nearbyPlayers = API.getPlayersInRadiusOfPlayer(150.0f, sender);
            if (API.isPlayerInAnyVehicle(sender) && API.getPlayerVehicleSeat(sender) == -1)
            {
                foreach (Client p in nearbyPlayers)
                {
                    API.sendChatMessageToPlayer(p, "~p~ [INDICATOR] : " + sender.name + " " + "is indicating left!");
                }
                API.setVehicleSpecialLightStatus(sender.vehicle, true);
            }
        }

        public void startIndicateRight(Client sender)
        {
            List<Client> nearbyPlayers = API.getPlayersInRadiusOfPlayer(150.0f, sender);
            if (API.isPlayerInAnyVehicle(sender) && API.getPlayerVehicleSeat(sender) == -1)
            {
                foreach (Client p in nearbyPlayers)
                {
                    API.sendChatMessageToPlayer(p, "~p~ [INDICATOR] : " + sender.name + " " + "is indicating right!");
                }
                API.setVehicleSpecialLightStatus(sender.vehicle, true);
            }
        }
    }
}
