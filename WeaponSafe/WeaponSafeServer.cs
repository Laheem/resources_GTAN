using System.Collections.Generic;
using System.IO;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json;

namespace WeaponSafe
{
    public class WeaponSafeServer : Script
    {
        public const string filePath = "allSafes";
        private readonly List<WeaponSafe> allSafes = new List<WeaponSafe>();

        public WeaponSafeServer()
        {
            API.onResourceStart += startWeaponSafeResource;
        }

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

        [Command("addSafe")]
        public void startAddSafe(Client sender, string password)
        {
            int x;
            if (int.TryParse(password, out x) && x >= 1000 && x <= 9999)
            {
                var safe = new WeaponSafe(sender, sender.position, password);
                API.createObject(-1251197000, safe.loc, new Vector3());
                return;
            }
            API.sendNotificationToPlayer(sender, "Invalid password, should be between 1000 and 9999.");
        }

        [Command("opensafe")]
        public void startOpenSafe(Client sender)
        {
            var nearestSafe = findNearestWeaponSafe(sender);
            if (nearestSafe == null)
            {
                API.sendNotificationToPlayer(sender, "You're not near a safe.");
                return;
            }
            if (sender != nearestSafe.owner)
                API.triggerClientEvent(sender, "openSafe");
            else
            {
                nearestSafe.locked = false;
            }
        }

        [Command("closeSafe")]
        public void startCloseSafe(Client sender)
        {
            var nearestSafe = findNearestWeaponSafe(sender);
            if (nearestSafe == null)
            {
                API.sendNotificationToPlayer(sender,"You're not near a safe.");
                return;
            }

            if (sender != nearestSafe.owner)
            {
                API.sendNotificationToPlayer(sender,"You're not the safe owner.");
                return;
            }
            API.sendNotificationToPlayer(sender,"You lock the safe, using the password.");
            nearestSafe.locked = true;
        }

        [Command("TakeWeapon")]
        public void takeWep(Client sender)
        {
            var nearestSafe = findNearestWeaponSafe(sender);
            if (nearestSafe == null)
            {
                API.sendNotificationToPlayer(sender,"You're not near a safe.");
                return;
            }

            if (nearestSafe.locked)
            {
                API.sendChatMessageToPlayer(sender,"~p~ [SAFE]: Try as you might, you're not going to get too far with a locked safe. It needs unlocking.");
                return;
            }

            // TODO : Add implementation for Str conversion of wep names.
            API.triggerClientEvent(sender,"showSafeWeapons",null);
        }


        private WeaponSafe findNearestWeaponSafe(Client sender, float bound = 10f)
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
    }
}