using System.Collections.Generic;
using GTANetworkShared;

namespace WeaponSafe
{
    // DO NOT REMOVE ANY WEAPONS FROM THIS.
    internal class WeaponName
    {
        public WeaponName()
        {
            wepList = new Dictionary<WeaponHash, string>();
            wepList.Add(WeaponHash.APPistol, "AP Pistol");
            wepList.Add(WeaponHash.Pistol, "Pistol");
            wepList.Add(WeaponHash.Pistol50, "Pistol .50");
            wepList.Add(WeaponHash.HeavyPistol, "Heavy Pistol");
            wepList.Add(WeaponHash.MachinePistol, "Machine Pistol");
            wepList.Add(WeaponHash.VintagePistol, "Vintage Pistol");

            wepList.Add(WeaponHash.SMG, "SMG");
            wepList.Add(WeaponHash.AssaultSMG, "Assault SMG");
            wepList.Add(WeaponHash.MicroSMG, "Micro SMG");
            wepList.Add(WeaponHash.MiniSMG, "Mini SMG");

            wepList.Add(WeaponHash.AdvancedRifle, "Advanced Rifle");
            wepList.Add(WeaponHash.AssaultRifle, "Assault Rifle");
            wepList.Add(WeaponHash.BullpupRifle, "Bullpup Rifle");
            wepList.Add(WeaponHash.CarbineRifle, "Carbine Rifle");
            wepList.Add(WeaponHash.CompactRifle, "Compact Rifle");
            wepList.Add(WeaponHash.MarksmanRifle, "Marksman Rifle");
            wepList.Add(WeaponHash.SniperRifle, "Sniper Rifle");

            wepList.Add(WeaponHash.AssaultShotgun, "Assault Shotgun");
            wepList.Add(WeaponHash.BullpupShotgun, "Bullpup Shotgun");
            wepList.Add(WeaponHash.DoubleBarrelShotgun, "Double Barrel Shotgun");
            wepList.Add(WeaponHash.PumpShotgun, "Pump Shotgun");
            wepList.Add(WeaponHash.HeavyShotgun, "Heavy Shotgun");
            wepList.Add(WeaponHash.SawnoffShotgun, "Sawn-off Shotgun");

            wepList.Add(WeaponHash.Bat, "Bat");
            wepList.Add(WeaponHash.Wrench, "Wrench");
            wepList.Add(WeaponHash.Bottle, "Bottle");
            wepList.Add(WeaponHash.Golfclub, "Golfclub");
            wepList.Add(WeaponHash.Hatchet, "Hatchet");
            wepList.Add(WeaponHash.Hammer, "Hammer");
            wepList.Add(WeaponHash.Knife, "Knife");
        }

        public Dictionary<WeaponHash, string> wepList { get; private set; }
    }
}