using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

namespace ServerMain
{


    public class AnimationServer : Script
    {
        public AnimationServer()
        {
            API.onResourceStart += resourceStarted;
        }

        private void resourceStarted()
        {
            API.consoleOutput("Freeze frame. Animations are ready to go.");
        }


        [Command("swipeCard")]
        public void startSwipeCard(Client player)
        {
            API.playPlayerAnimation(player, (int) (AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody),
                "mp_fbi_heist", "card_swipe");
        }

        [Command("wave")]
        public void startCheer(Client player)
        {
            API.playPlayerAnimation(player, (int) (AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody),
                "random@car_thief@waving_ig_1", "waving");

        }

        [Command("kneel")]
        public void startKneel(Client player)
        {
            API.playPlayerAnimation(player, (int) (AnimationFlags.Loop), "amb@medic@standing@kneel@base", "base");
            API.playPlayerAnimation(player, (int) (AnimationFlags.Loop), "amb@medic@standing@kneel@enter", "enter");


        }

        [Command("collapse")]
        public void startCollapse(Client player)
        {
            API.playPlayerAnimation(player, (int) (AnimationFlags.Loop),
                "amb@world_human_bum_slumped@male@laying_on_left_side@idle_a", "idle_a");

        }
    }

}
