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

        [JsonProperty(Order = 7)] public Guid id;

        public WeaponSafe(Client owner, Vector3 loc, string password)
        {
            wepList = new List<WeaponHash>();
            this.loc = loc;
            this.password = password;
            maxSize = MAX_SAFE_SIZE;
            remWeight = MAX_SAFE_SIZE;
            id = new Guid();
        }


        [JsonProperty(Order = 1)]
        public Vector3 loc { get; set; }

        [JsonProperty(Order = 2)]
        private List<WeaponHash> wepList { get; set; }

        [JsonProperty(Order = 3)]
        public int maxSize { get; set; }

        [JsonProperty(Order = 4)]
        public string password { get; set; }

        [JsonProperty(Order = 5)]
        public Client owner { get; }

        [JsonProperty(Order = 6)]
        public int remWeight { get; set; }

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

        public WeaponHash takeWep(int pos)
        {
            WeaponHash wep = wepList.ElementAt(pos);
            wepList.Remove(wepList.ElementAt(pos));
            saveSafe();
            return wep;
        }
    }
}