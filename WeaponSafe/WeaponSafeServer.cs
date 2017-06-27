using System;
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
        private List<WeaponSafe> allSafes = new List<WeaponSafe>();

        public WeaponSafeServer()
        {
            API.onResourceStart += startWeaponSafeResource;
        }

        private void startWeaponSafeResource()
        {
            API.consoleOutput("Weapon safe has started. Let's go.");
            Directory.CreateDirectory(filePath);
            foreach (var file in Directory.GetFiles(filePath,"*.json", SearchOption.TopDirectoryOnly))
            {
                var a = JsonConvert.DeserializeObject<WeaponSafe>(File.ReadAllText(file));
                allSafes.Add(a);
                API.createObject(-1251197000, a.loc, new Vector3());


            }
        }

        [Command("addSafe")]
        public void startAddSafe(Client sender,String password)
        {
            int x;
            if (Int32.TryParse(password, out x) && (x >= 1000 && x <= 9999))
            {
                WeaponSafe safe = new WeaponSafe(sender, sender.position,password);
                API.createObject(-1251197000, safe.loc, new Vector3());
                return;
            }
            API.sendNotificationToPlayer(sender,"Invalid password, should be between 1000 and 9999.");
        }

        [Command("opensafe")]
        public void startOpenSafe(Client sender)
        {
            WeaponSafe nearestSafe = findNearestWeaponSafe(sender);
            if (nearestSafe == null)
            {
                API.sendNotificationToPlayer(sender,"You're not near a safe.");
                return;
            }
            if (sender != nearestSafe.owner)
            {
                API.triggerClientEvent(sender, "openSafe");
            }
        }


        private WeaponSafe findNearestWeaponSafe(Client sender,float bound = 10f)
        {
            WeaponSafe safe = null;
            foreach (WeaponSafe s in allSafes)
            {
                float dist = s.loc.DistanceTo(sender.position);
                if (dist < bound)
                {
                    safe = s;
                }
            }
            return safe;
        }
    }
}