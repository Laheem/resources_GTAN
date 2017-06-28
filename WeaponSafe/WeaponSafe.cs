using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GTANetworkServer;
using GTANetworkShared;
using Newtonsoft.Json;

namespace WeaponSafe
{
    public class WeaponSafe
    {
        private const int MAX_SAFE_SIZE = 1000;
        private readonly WeaponWeight weights = new WeaponWeight();

        public WeaponSafe(string ownerName, Vector3 loc, string password)
        {
            wepList = new List<WeaponHash>();
            this.loc = loc;
            this.password = password;
            maxSize = MAX_SAFE_SIZE;
            remWeight = MAX_SAFE_SIZE;
            id = Guid.NewGuid();
            locked = true;
            this.ownerName = ownerName;
        }

        // Indicates all the properties that are to be serialized by JSON.

        [JsonProperty(Order = 1)]
        public Vector3 loc { get; set; }

        [JsonProperty(Order = 2)]
        public List<WeaponHash> wepList { get; set; }

        [JsonProperty(Order = 3)]
        public int maxSize { get; set; }

        [JsonProperty(Order = 4)]
        public string password { get; set; }

        [JsonProperty(Order = 5)]
        public string ownerName { get; set; }

        [JsonProperty(Order = 6)]
        public int remWeight { get; set; }

        [JsonProperty(Order = 7)]
        public Guid id { get; private set; }

        [JsonProperty(Order = 8)]
        public bool locked { get; set; }


        // Adds a weapon to the vault, checks for correct size and weapons.
        public SafeStatus addWeaponToVault(Client sender, WeaponHash wep)
        {
            int x;
            if (weights.weightMap.TryGetValue(wep, out x))
                if (x > remWeight)
                {
                    return SafeStatus.WeightTooHigh;
                }
                else
                {
                    remWeight = remWeight - x;
                    wepList.Add(wep);
                    saveSafe();
                    return SafeStatus.Success;
                }
            return SafeStatus.NonPermittedWeapon;
        }

        public void saveSafe()
        {
            Directory.CreateDirectory(WeaponSafeServer.filePath);
            File.WriteAllText(WeaponSafeServer.filePath + "\\" + id + ".json",
                JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        // Takes a weapon from the safe's list, and removes it and gives back the weight to the safe.
        // Returns the weapon needed to give to the player.
        public WeaponHash takeWepFromSafe(int pos)
        {
            int x;
            var wep = wepList.ElementAt(pos);
            weights.weightMap.TryGetValue(wep, out x);
            remWeight = remWeight + x;
            wepList.Remove(wepList.ElementAt(pos));
            saveSafe();
            return wep;
        }
    }
}