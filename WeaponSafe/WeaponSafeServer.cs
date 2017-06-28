using System;
using System.Collections.Generic;
using System.IO;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json;

namespace WeaponSafe
{
    //TODO : Write an entire naming system for this. Social Club name not great. 
    //TODO : Work out why CEF doesn't work half the time, something to do with DOM?
    //TODO : Tidy up some of the messages.
    //TODO : Fix floaty safe.

    public class WeaponSafeServer : Script
    {
        public const string filePath = "allSafes";

        // Should really only edit MAX_PASS_VAL, you will need to edit the inner JS on the HTML page.
        public const int MIN_PASS_VAL = 0;

        public const int MAX_PASS_VAL = 9999;

        private readonly WeaponName names = new WeaponName();

        private readonly List<WeaponSafe> allSafes = new List<WeaponSafe>();

        public WeaponSafeServer()
        {
            API.onResourceStart += startWeaponSafeResource;
            API.onClientEventTrigger += safeClientHandler;
        }

        private void safeClientHandler(Client sender, string eventName, params object[] arguments)
        {
            switch (eventName)
            {
                // Pulls the weapon and index of it in the list, then gives the player it.
                // the GUID works similar to a PK in databases, just returns the entity that matches it.
                case "takeWeapon":
                    var wepName = (string) arguments[0];
                    var index = (int) arguments[1];
                    var target = JsonConvert.DeserializeObject<Guid>((string) arguments[2]);
                    var wepSafe = findEqualSafe(target);
                    var wep = wepSafe.takeWepFromSafe(index);
                    API.sendChatMessageToPlayer(sender, "~p~[SAFE]:" + " You take a " + wepName + " from the safe.");
                    API.givePlayerWeapon(sender, wep, 9999, true, false);
                    break;
                case "checkPass":
                    // Similar to above, pulls the PK of the safe, then checks the password against that entity.
                    var id = JsonConvert.DeserializeObject<Guid>((string) arguments[0]);
                    var safe = findEqualSafe(id);
                    var attemptStr = (string) arguments[1];
                    if (safe.password.Equals(attemptStr))
                    {
                        API.sendChatMessageToPlayer(sender, "~p~[SAFE]: The safe unlocks.");
                        safe.locked = false;
                        return;
                    }
                    API.sendNotificationToPlayer(sender, "Incorrect password.");
                    break;

                default:
                    break;
            }
        }

        // Adds all the safes that are currently stored in the filepath. 
        private void startWeaponSafeResource()
        {
            API.consoleOutput("Weapon safe has started. Let's go.");
            Directory.CreateDirectory(filePath);
            foreach (var file in Directory.GetFiles(filePath, "*.json", SearchOption.TopDirectoryOnly))
            {
                var a = JsonConvert.DeserializeObject<WeaponSafe>(File.ReadAllText(file));
                allSafes.Add(a);
                API.createObject(-1251197000, a.loc, new Vector3());
            }
        }

        // Adds a safe to the map, and adds it to the list.
        [Command("addSafe")]
        public void startAddSafe(Client sender, string password)
        {
            int x;
            if (int.TryParse(password, out x) && x >= MIN_PASS_VAL && x <= MAX_PASS_VAL)
            {
                var safe = new WeaponSafe(sender.socialClubName, sender.position, password);
                safe.saveSafe();
                API.createObject(-1251197000, safe.loc, new Vector3());
                allSafes.Add(safe);
                return;
            }
            API.sendNotificationToPlayer(sender, "Invalid password, should be between 1000 and 9999.");
        }

        // Opening and closing the safe.

        [Command("opensafe")]
        public void startOpenSafe(Client sender)
        {
            var nearestSafe = findNearestWeaponSafe(sender);
            if (nearestSafe == null)
            {
                API.sendNotificationToPlayer(sender, "You're not near a safe.");
                return;
            }
            if (!nearestSafe.locked)
            {
                API.sendNotificationToPlayer(sender, "The safe is already unlocked.");
                return;
            }

            if (!sender.socialClubName.Equals(nearestSafe.ownerName))
            {
                API.triggerClientEvent(sender, "openSafe", API.toJson(nearestSafe.id));
            }
            else
            {
                nearestSafe.locked = false;
                API.sendChatMessageToPlayer(sender, "~p~[SAFE]: The safe opens.");
            }
        }

        [Command("closeSafe")]
        public void startCloseSafe(Client sender)
        {
            var nearestSafe = findNearestWeaponSafe(sender);
            if (nearestSafe == null)
            {
                API.sendNotificationToPlayer(sender, "You're not near a safe.");
                return;
            }
            if (nearestSafe.locked)
            {
                API.sendNotificationToPlayer(sender, "This safe is already locked tight.");
                return;
            }
            if (!sender.socialClubName.Equals(nearestSafe.ownerName))
            {
                API.sendNotificationToPlayer(sender, "You're not the safe owner.");
                return;
            }
            API.sendChatMessageToPlayer(sender, "~p~[SAFE]: The safe locks.");
            nearestSafe.locked = true;
        }

        // Adding and Taking Weapons functionality.

        [Command("TakeWeapon")]
        public void takeWep(Client sender)
        {
            var nearestSafe = findNearestWeaponSafe(sender);
            if (nearestSafe == null)
            {
                API.sendNotificationToPlayer(sender, "You're not near a safe.");
                return;
            }

            if (nearestSafe.locked)
            {
                API.sendChatMessageToPlayer(sender,
                    "~p~[SAFE]: Try as you might, you're not going to get too far with a locked safe. It needs unlocking.");
                return;
            }

            // Converts the weapon hashes to names, to allow for the Javascript to display the actual names.
            var wepNames = convertWepToStr(nearestSafe.wepList);

            // Convert the weapon names into JSON, along with the GUID of the safe you're trying to use and send it off to Javascript.
            API.triggerClientEvent(sender, "showSafeWeapons", returnallWeaponNames(wepNames),
                API.toJson(nearestSafe.id));
        }


        [Command("addWeapon")]
        public void addWeapon(Client sender)
        {
            var nearestSafe = findNearestWeaponSafe(sender);
            if (nearestSafe == null)
            {
                API.sendNotificationToPlayer(sender, "You're not near a safe.");
                return;
            }
            if (nearestSafe.locked)
            {
                API.sendChatMessageToPlayer(sender,
                    "~p~[SAFE]: Try as you might, you're not going to get too far with a locked safe. It needs unlocking.");
                return;
            }

            var status = nearestSafe.addWeaponToVault(sender, sender.currentWeapon);
            if (status == SafeStatus.Success)
            {
                API.removePlayerWeapon(sender, sender.currentWeapon);
                API.sendChatMessageToPlayer(sender,
                    "~p~[SAFE]: You add the weapon inside of the safe, tucking it away.");
                return;
            }
            if (status == SafeStatus.WeightTooHigh)
            {
                API.sendNotificationToPlayer(sender, "There's not enough space for that weapon.");
                return;
            }

            API.sendNotificationToPlayer(sender, "You can't add that weapon.");
        }


        // Helper Functions.

        private WeaponSafe findNearestWeaponSafe(Client sender, float bound = 5)
        {
            WeaponSafe safe = null;
            foreach (var s in allSafes)
            {
                var dist = s.loc.DistanceTo(sender.position);
                if (dist < bound)
                    safe = s;
            }
            return safe;
        }

        public List<string> convertWepToStr(List<WeaponHash> wepList)
        {
            var wepNames = new List<string>();
            foreach (var w in wepList)
            {
                string s;
                names.wepList.TryGetValue(w, out s);
                wepNames.Add(s);
            }
            return wepNames;
        }

        public List<string> returnallWeaponNames(List<string> listofWeapons)
        {
            var allWeaponNames = new List<string>();
            foreach (var w in listofWeapons)
            {
                var jsonParse = new Dictionary<string, object>();
                jsonParse["weaponName"] = w;
                allWeaponNames.Add(API.toJson(jsonParse));
            }

            return allWeaponNames;
        }

        public WeaponSafe findEqualSafe(Guid id)
        {
            foreach (var s in allSafes)
                if (s.id == id)
                    return s;
            return null;
        }
    }
}