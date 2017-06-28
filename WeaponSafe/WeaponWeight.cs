using System.Collections.Generic;
using GTANetworkShared;

namespace WeaponSafe
{
    // Purpose of the class is to give each weapon a "weight" in the safe. If you want to modify this, change the constant values.
    // Add weapons that I may have missed here and in WeaponName.cs
    public class WeaponWeight
    {
        private const int REGULAR_PISTOL_WEIGHT = 50;
        private const int REGULAR_SMG_WEIGHT = 150;
        private const int REGULAR_RIFLE_WEIGHT = 500;
        private const int REGULAR_SHOTGUN_WEIGHT = 400;
        private const int REGULAR_MELEE_WEIGHT = 20;

        public WeaponWeight()
        {
            weightMap = new Dictionary<WeaponHash, int>();
            weightMap.Add(WeaponHash.APPistol, REGULAR_PISTOL_WEIGHT);
            weightMap.Add(WeaponHash.Pistol, REGULAR_PISTOL_WEIGHT);
            weightMap.Add(WeaponHash.Pistol50, REGULAR_PISTOL_WEIGHT);
            weightMap.Add(WeaponHash.HeavyPistol, REGULAR_PISTOL_WEIGHT);
            weightMap.Add(WeaponHash.MachinePistol, 75);
            weightMap.Add(WeaponHash.VintagePistol, REGULAR_PISTOL_WEIGHT);

            weightMap.Add(WeaponHash.SMG, REGULAR_SMG_WEIGHT);
            weightMap.Add(WeaponHash.AssaultSMG, REGULAR_SMG_WEIGHT);
            weightMap.Add(WeaponHash.MicroSMG, REGULAR_SMG_WEIGHT - 25);
            weightMap.Add(WeaponHash.MiniSMG, REGULAR_SMG_WEIGHT - 20);

            weightMap.Add(WeaponHash.AdvancedRifle, REGULAR_RIFLE_WEIGHT);
            weightMap.Add(WeaponHash.AssaultRifle, REGULAR_RIFLE_WEIGHT);
            weightMap.Add(WeaponHash.BullpupRifle, REGULAR_RIFLE_WEIGHT);
            weightMap.Add(WeaponHash.CarbineRifle, REGULAR_RIFLE_WEIGHT);
            weightMap.Add(WeaponHash.CompactRifle, REGULAR_RIFLE_WEIGHT - 50);
            weightMap.Add(WeaponHash.MarksmanRifle, REGULAR_RIFLE_WEIGHT);
            weightMap.Add(WeaponHash.SniperRifle, REGULAR_RIFLE_WEIGHT);

            weightMap.Add(WeaponHash.AssaultShotgun, REGULAR_SHOTGUN_WEIGHT + 100);
            weightMap.Add(WeaponHash.BullpupShotgun, REGULAR_SHOTGUN_WEIGHT);
            weightMap.Add(WeaponHash.DoubleBarrelShotgun, REGULAR_SHOTGUN_WEIGHT);
            weightMap.Add(WeaponHash.PumpShotgun, REGULAR_SHOTGUN_WEIGHT);
            weightMap.Add(WeaponHash.HeavyShotgun, REGULAR_SHOTGUN_WEIGHT);
            weightMap.Add(WeaponHash.SawnoffShotgun, REGULAR_SHOTGUN_WEIGHT);

            weightMap.Add(WeaponHash.Bat, REGULAR_MELEE_WEIGHT);
            weightMap.Add(WeaponHash.Wrench, REGULAR_MELEE_WEIGHT);
            weightMap.Add(WeaponHash.Bottle, REGULAR_MELEE_WEIGHT);
            weightMap.Add(WeaponHash.Golfclub, REGULAR_MELEE_WEIGHT);
            weightMap.Add(WeaponHash.Hatchet, REGULAR_MELEE_WEIGHT);
            weightMap.Add(WeaponHash.Hammer, REGULAR_MELEE_WEIGHT);
            weightMap.Add(WeaponHash.Knife, REGULAR_MELEE_WEIGHT);
        }

        public Dictionary<WeaponHash, int> weightMap { get; private set; }
    }
}