using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

namespace TheftFighter
{
    class Arena
    {
        public List<Vector3> spawnPt { get; private set; }
        public Vector3 cameraLoc { get; private set; }
        public bool isFree { get; private set; }
        public List<Client> fighters { get; private set; }


        public Arena(List<Vector3> spawnPts, Vector3 cameraLoc)
        {
            this.spawnPt = spawnPts;
            this.cameraLoc = cameraLoc;
            isFree = true;
            fighters = new List<Client>();
        }

        public void addFighters(List<Client> fightersList)
        {
            fighters = fightersList;
            isFree = false;
        }

        public void removeFighters()
        {
            fighters = new List<Client>();
            isFree = true;
        }
    }
}
