using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkShared;
using GTANetworkServer;

namespace Sierra
{
    public class Atm
    {
        public Vector3 atmLoc { get; private set; }

        public Atm(Vector3 loc)
        {
            this.atmLoc = loc;
        }

        public static List<Atm> getListOfAllAtms()
        {
            List<Atm> atms = new List<Atm>();

            atms.Add((new Atm(new Vector3(-846.6537, -341.509, 37.6685))));
            atms.Add( new Atm(new Vector3(-847.204, -340.4291, 37.6793)));
            atms.Add( new Atm(new Vector3(-1410.736, -98.9279, 51.397)));
            atms.Add( new Atm(new Vector3(-1410.183, -100.6454, 51.3965)));
            atms.Add( new Atm(new Vector3(-2295.853, 357.9348, 173.6014)));
            atms.Add( new Atm(new Vector3(-2295.069, 356.2556, 173.6014)));
            atms.Add( new Atm(new Vector3(-2294.3, 354.6056, 173.6014)));
            atms.Add( new Atm(new Vector3(-282.7141, 6226.43, 30.4965)));
            atms.Add( new Atm(new Vector3(-386.4596, 6046.411, 30.474)));
            atms.Add( new Atm(new Vector3(24.5933, -945.543, 28.333)));
            atms.Add( new Atm(new Vector3(5.686, -919.9551, 28.4809)));
            atms.Add( new Atm(new Vector3(296.1756, -896.2318, 28.2901)));
            atms.Add( new Atm(new Vector3(296.8775, -894.3196, 28.2615)));
            atms.Add( new Atm(new Vector3(-846.6537, -341.509, 37.6685)));
            atms.Add( new Atm(new Vector3(-847.204, -340.4291, 37.6793)));
            atms.Add( new Atm(new Vector3(-1410.736, -98.9279, 51.397)));
            atms.Add( new Atm(new Vector3(-1410.183, -100.6454, 51.3965)));
            atms.Add( new Atm(new Vector3(-2295.853, 357.9348, 173.6014)));
            atms.Add( new Atm(new Vector3(-2295.069, 356.2556, 173.6014)));
            atms.Add( new Atm(new Vector3(-2294.3, 354.6056, 173.6014)));


            return atms;
        }
    }


}
