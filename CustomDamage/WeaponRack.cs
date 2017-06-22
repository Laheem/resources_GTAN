using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

namespace CustomDamage
{
    class WeaponRack
    {
        public Dictionary<NetHandle, WeaponHash> listOfWeapons { get; private set; }
        public const int MAX_WEPS = 10;
        public Client owner { get; private set; }


        public WeaponRack(Client owner)
        {
            this.owner = owner;
            listOfWeapons = new Dictionary<NetHandle, WeaponHash>();
        }

        public NetHandle addWeapon(Client owner,WeaponHash wep)
        {
            int wepHash = getWeaponHash(wep);
            if (wepHash == -1)
            {
                return new NetHandle();
            }

            listOfWeapons.Add();

        }

        public int getWeaponHash(WeaponHash wep)
        {
            switch (wep)
            {
                case WeaponHash.MicroSMG:
                    return -1056713654;
                case WeaponHash.AssaultSMG:
                    return -473574177;
                case WeaponHash.MG:
                    return -2056364402;
                case WeaponHash.CombatMG:
                    return -739394447;
                case WeaponHash.Gusenberg:
                    return 574348740;
                case WeaponHash.AssaultRifle:
                    return 273925117;
                case WeaponHash.CarbineRifle:
                    return 1026431720;
                case WeaponHash.AdvancedRifle:
                    return -1707584974;
                case WeaponHash.SpecialCarbine:
                    return -1745643757;
                case WeaponHash.BullpupRifle:
                    return -1288559573;
                case WeaponHash.SniperRifle:
                    return 346403307;
                case WeaponHash.HeavySniper:
                    return -746966080;
                case WeaponHash.MarksmanRifle:
                    return -1711248638;
                case WeaponHash.PumpShotgun:
                    return 689760839;
                case WeaponHash.BullpupShotgun:
                    return -1598212834;
                case WeaponHash.AssaultShotgun:
                    return 1255410010;
                case WeaponHash.HeavyShotgun:
                    return -1209868881;
                default:
                    return -1;

            }
        }
    }
}
