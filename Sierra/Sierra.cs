using System;
using System.Linq;
using ApiAiSDK;
using GTANetworkServer;
using GTANetworkShared;


// Author -  @Cratox, follow up any queries about this to me! 
// IMPORTANT NOTE - This is in development, and is by no means complete. This is not representative of the final project.


namespace Sierra
{
    public class Sierra : Script
    {
        // Access token needed by API.AI to access Sierra. This is liable to change if I refresh!
        private const string ACCESS_TOKEN = "c4019fc02df84319ac70094cfdbc608f";

        private ApiAi apiAi;


        public Sierra()
        {
            API.onChatMessage += checkforSierra;
            API.onResourceStart += sierraStartUp;
            API.onPlayerConnected += checkSierraName;
        }

        // Checks Entity data for the player, defaults to normal if not.
        private void checkSierraName(Client player)
        {
            if (!API.hasEntityData(player, "SierraName"))
                API.setEntityData(player, "SierraName", "Sierra");
            if (!API.hasEntityData(player, "SierraMute"))
                API.setEntityData(player, "SierraMute", false);
        }

        private void sierraStartUp()
        {
            API.consoleOutput("Sierra says hello!");
            API.loadConfig("Newtonsoft.Json.xml");
        }

        // Splits the message, checks for Sierra's current name, then takes everything after the name and passes that to the callSierra method. 
        private void checkforSierra(Client sender, string message, CancelEventArgs cancel)
        {
            var msgArr = message.Split(' ');
            var destArr = new string[msgArr.Length];
            var count = 0;
            string sierraName = API.getEntityData(sender, "SierraName");

            for (var index = 0; index < msgArr.Length; index++)
            {
                var s = msgArr[index];
                if (s.ToLower().Contains(sierraName.ToLower()) && count <= 1 &&
                    !API.getEntityData(sender, "SierraMute") && msgArr.Length > 1)
                {
                    count++;
                    Array.Copy(msgArr, count, destArr, 0, msgArr.Length - count);
                    var finalMessage = string.Join(" ", destArr);
                    if (finalMessage.Trim().Length == 0)
                    {
                        API.sendChatMessageToPlayer(sender,
                            "~b~[" + getSierraName(sender, true) + "]: " + " Sorry, I didn't understand that.");

                        break;
                    }
                    callSierra(sender, finalMessage);
                    API.playSoundFrontEnd(sender, "Put_Away", "Phone_SoundSet_Michael");
                    break;
                }
                count++;
            }
        }

        // Manages the call for Sierra, takes the response back from Google via the action variable.
        public void callSierra(Client sender, string message)
        {
            var config = new AIConfiguration(ACCESS_TOKEN, SupportedLanguage.English);
            apiAi = new ApiAi(config);
            try
            {
                var response = apiAi.TextRequest(message);
                if (response.IsError)
                    throw new AIServiceException("Bad Request");
                var sierraMessage = response.Result.Fulfillment.Speech;
                var action = response.Result.Action;

                // Dependant on the action Google returns, handle something based on that.
                switch (action)
                {
                    case "lock":
                        lockCarInside(sender, sierraMessage);
                        break;
                    case "unlock":
                        unlockCar(sender, sierraMessage);
                        break;
                    case "car-on":
                        changeEngineStatus(sender, sierraMessage, true);
                        break;
                    case "car-off":
                        changeEngineStatus(sender, sierraMessage, false);
                        break;
                    case "name-change":
                        string newName;
                        object val;
                        if (response.Result.Parameters.TryGetValue("given-name", out val))
                        {
                            newName = val.ToString();
                            if (newName == "")
                            {
                                API.sendNotificationToPlayer(sender, "Sorry, you need to put a name in.");
                                return;
                            }
                            changeSierraName(sender, sierraMessage, newName);
                        }
                        break;
                    case "mute-Sierra":
                        sierraMute(sender, true, sierraMessage);
                        break;
                    case "get-time":
                        getSierraTime(sender, sierraMessage);
                        break;
                    default:
                        API.sendChatMessageToPlayer(sender,
                            "~b~[" + getSierraName(sender, true) + "]: " + " Sorry, I didn't understand that.");
                        break;
                }
            }
            // If something goes horribly wrong, hide it from the User and print the message to the console. Alert dev to this if something occurs!
            catch (Exception e)
            {
                API.sendNotificationToPlayer(sender, "Sierra has suffered an error. Sorry!");
                API.consoleOutput(e.StackTrace);
            }
        }

        // Methods below are used as EXAMPLES ONLY, none of these are implemented in other projects so I just made them in here.
        // This can be done in several ways, call or using Namespaces. 

        public void lockCarInside(Client sender, string sierraMessage)
        {
            API.sendChatMessageToPlayer(sender, "~b~[" + getSierraName(sender, true) + "]: " + sierraMessage);
            if (sender.isInVehicle && API.getPlayerVehicleSeat(sender) == -1)
            {
                var playerVeh = sender.vehicle;
                API.setVehicleLocked(playerVeh, true);
                API.sendChatMessageToPlayer(sender,
                    "~b~[" + getSierraName(sender, true) + "]: Done! Your vehicle is now locked. ");
            }
            else
            {
                API.sendChatMessageToPlayer(sender,
                    "~b~[" + getSierraName(sender, true) + "]: Whoops. I wasn't able to find your car.");
            }
        }

        public void unlockCar(Client sender, string sierraMessage)
        {
            API.sendChatMessageToPlayer(sender, "~b~[" + getSierraName(sender, true) + "]: " + sierraMessage);
            var veh = GetClosestVehicle(sender, 100);
            if (veh == new NetHandle())
            {
                API.sendChatMessageToPlayer(sender,
                    "~b~[" + getSierraName(sender, true) +
                    "]: Whoops! I wasn't able to find your car. Try moving closer to it.");
                return;
            }

            API.setVehicleLocked(veh, false);
            API.sendChatMessageToPlayer(sender,
                "~b~[" + getSierraName(sender, true) + "]: Done! Your vehicle is now unlocked. ");
        }

        public void changeEngineStatus(Client sender, string sierraMessage, bool engineFlip)
        {
            API.sendChatMessageToPlayer(sender, "~b~[" + getSierraName(sender, true) + "]: " + sierraMessage);
            var veh = GetClosestVehicle(sender);
            if (veh == new NetHandle())
            {
                API.sendChatMessageToPlayer(sender,
                    "~b~[" + getSierraName(sender, true) +
                    "]: Whoops! I wasn't able to find your car. Try moving closer to it.");
                return;
            }
            API.sendChatMessageToPlayer(sender,
                engineFlip
                    ? "~b~[" + getSierraName(sender, true) + "]: Done! Your vehicle is now on. "
                    : "~b~[" + getSierraName(sender, true) + "]: Done! Your vehicle is now off. ");
            API.setVehicleEngineStatus(veh, engineFlip);
        }

        // Proof of concept commands end.

        // Sierra utilisation commands.

        public void changeSierraName(Client sender, string sierraMessage, string newName)
        {
            API.sendNotificationToPlayer(sender,
                getSierraName(sender, false) + " has changed it's name to " + newName + "!");
            API.setEntityData(sender.handle, "SierraName", newName);
            API.sendChatMessageToPlayer(sender, "~b~[" + getSierraName(sender, true) + "]: " + sierraMessage);
        }

        [Command("unmuteSierra")]
        public void callSierraMute(Client sender)
        {
            sierraMute(sender, false);
        }

        // Avoids Sierra from constantly checking each message for it's name. 
        public void sierraMute(Client sender, bool mute, string sierraMessage = "")
        {
            if (!API.getEntityData(sender, "SierraMute") && mute)
            {
                API.sendChatMessageToPlayer(sender, "~b~[" + getSierraName(sender, true) + "]: " + sierraMessage);
                API.setEntityData(sender, "SierraMute", mute);
            }
            else if (API.getEntityData(sender, "SierraMute") && !mute)
            {
                API.sendChatMessageToPlayer(sender,
                    "~b~[" + getSierraName(sender, true) + "]: " + "Sure, I'm listening in again.");
                API.setEntityData(sender, "SierraMute", false);
            }
            else
            {
                API.sendNotificationToPlayer(sender, getSierraName(sender, false) + "is already listening!");
            }
        }

        public void getSierraTime(Client sender, string sierraMessage)
        {
            API.sendChatMessageToPlayer(sender,
                "~b~[" + getSierraName(sender, true) + "]: " + sierraMessage + " " + API.getTime());
        }

        public string getSierraName(Client sender, bool upper)
        {
            string s = API.getEntityData(sender, "SierraName");
            return upper ? s.ToUpper() : s;
        }


        // Filler method by made by StreetGT - https://forum.gtanet.work/index.php?threads/get-closest-vehicle-to-player-function.2066/ 
        // No actual implementation for personal vehicles currently, but the idea is this can be adapated.
        // Change the API.getAllVehicles to check for the player's entityData eg, then have the range based on whatever you want.
        public NetHandle GetClosestVehicle(Client sender, float distance = 1000.0f)
        {
            var handleReturned = new NetHandle();
            foreach (var veh in API.getAllVehicles())
            {
                var vehPos = API.getEntityPosition(veh);
                var distanceVehicleToPlayer = sender.position.DistanceTo(vehPos);
                if (distanceVehicleToPlayer < distance)
                {
                    distance = distanceVehicleToPlayer;
                    handleReturned = veh;
                }
            }
            return handleReturned;
        }

    }
}