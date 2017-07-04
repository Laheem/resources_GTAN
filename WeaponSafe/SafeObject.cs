using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkShared;

namespace WeaponSafe
{
    public class SafeObject
    {

        public NetHandle safeEntity { get; private set; }
        public Vector3 location { get; private set; }

        public SafeObject(NetHandle safeEntity, Vector3 location)
        {
            this.safeEntity = safeEntity;
            this.location = location;
        }
    }
}
