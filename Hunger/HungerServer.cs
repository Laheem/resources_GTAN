using System;
using System.Timers;
using GTANetworkServer;

namespace ServerMain
{
    public class HungerServer : Script
    {
        // Suggested to not let EFFECTS_VALUE to divide into MIN_VALUE without remainder, or isolation is not ensured.
        // Values being changed mid thread = bad. :(
        // These are minute values, change this to change the rate of hunger + effects.
        private const double MIN_VALUE = 5;
        private const double EFFECTS_VALUE = 11;

        // The "boundaries" before effects begin to happen.
        private const int LEVEL_ONE_BOUNDARY = 40;
        private const int LEVEL_TWO_BOUNDARY = 20;
        private const int LEVEL_THREE_BOUNDARY = 10;

        private readonly Timer effectsTimer = new Timer(convertMinToMilli(EFFECTS_VALUE));

        private readonly Timer hungerTimer = new Timer(convertMinToMilli(MIN_VALUE));


        public HungerServer()
        {
            API.onResourceStart += hungerStarted;
            API.onPlayerConnected += attachClientHunger;
        }


        private void attachClientHunger(Client player)
        {
            if (!API.hasEntitySyncedData(player, "hungerVal"))
                API.setEntitySyncedData(player, "hungerVal", 100);
            if (!API.hasEntitySyncedData(player, "hungerVisable"))
                API.setEntitySyncedData(player, "hungerVisable", true);
        }

        private void hungerStarted()
        {
            API.consoleOutput("Hunger started successfully! Don't forget your greens.");
            // Remove the following if statement if you don't care about the two timers running at the same time.
            if (EFFECTS_VALUE % MIN_VALUE == 0)
            {
                API.consoleOutput("Isolation is not ensured. This resource will not run.");
                API.stopResource("Hunger");
            }

        hungerTimer.Elapsed += deductHunger;
            hungerTimer.Enabled = true;
            effectsTimer.Elapsed += attemptEffects;
            effectsTimer.Enabled = true;
        }

        // Timer hook for effects, will check for each level. The switch-case is slightly redundant, will be changed later.
        private void attemptEffects(object sender, ElapsedEventArgs e)
        {
            foreach (var c in API.getAllPlayers())
            {
                var level = checkStatusLevel(c);
                switch (level)
                {
                    case EffectLevel.NoEffect:
                        break;
                    case EffectLevel.EffectLevelOne:
                        preformEffects(c, level);
                        break;
                    case EffectLevel.EffectLevelTwo:
                        preformEffects(c, level);
                        break;
                    case EffectLevel.EffectLevelThree:
                        preformEffects(c, level);
                        break;
                    // Something has gone badly wrong, check the console log for infomation. 
                    default:
                        API.consoleOutput("Something went wrong with hunger.");
                        API.consoleOutput(API.getEntitySyncedData(c, "hungerVal") +
                                          " is their hunger value. Resetting to avoid further errors.");
                        API.setEntitySyncedData(c, "hungerVal", 100);
                        break;
                }
            }
        }

        // Commands to Show/Hide the hunger bar.

        [Command("hidehunger")]
        public void startHideHunger(Client sender)
        {
            API.setEntitySyncedData(sender, "hungerVisable", false);
        }

        [Command("showHunger")]
        public void startShowHunger(Client sender)
        {
            API.setEntitySyncedData(sender, "hungerVisable", true);
        }

        // Timer hook for removing 1 hunger per unit of time. 
        private void deductHunger(object sender, ElapsedEventArgs e)
        {
            foreach (var c in API.getAllPlayers())
            {
                if (API.getEntitySyncedData(c, "hungerVal") > 0)
                    API.setEntitySyncedData(c, "hungerVal", API.getEntitySyncedData(c, "hungerVal") - 1);
                int hunger = API.getEntitySyncedData(c, "hungerVal");

                // Checks to see if a user has hit a boundary value, and prints a message informing them their values are getting low.
                switch (hunger)
                {
                    case LEVEL_ONE_BOUNDARY:
                        API.sendChatMessageToPlayer(c, "~p~ You're feeling a bit hungry. ");
                        break;
                    case LEVEL_TWO_BOUNDARY:
                        API.sendChatMessageToPlayer(c, "~p~ You're feeling incredibly hungry. Time to eat?");
                        break;
                    case LEVEL_THREE_BOUNDARY:
                        API.sendChatMessageToPlayer(c,"~p~ You're so hungry it's starting to hurt. You might wanna get moving.");
                        break;
                }
            }
        }

        // Works out the correct boundary level the user is at and assigns the correct Enum.
        public EffectLevel checkStatusLevel(Client sender)
        {
            int hunger = API.getEntitySyncedData(sender, "hungerVal");
            if (hunger > LEVEL_ONE_BOUNDARY)
                return EffectLevel.NoEffect;
            if (hunger > LEVEL_TWO_BOUNDARY && hunger <= LEVEL_ONE_BOUNDARY)
                return EffectLevel.EffectLevelOne;
            if (hunger > LEVEL_THREE_BOUNDARY && hunger <= LEVEL_TWO_BOUNDARY)
                return EffectLevel.EffectLevelTwo;
            if (hunger < LEVEL_THREE_BOUNDARY)
                return EffectLevel.EffectLevelThree;

            return EffectLevel.Error;
        }

        // Pulls a random number, continues with effects if that number hits.
        // Better effects are added later, this is a proof of concept.
        public void preformEffects(Client sender, EffectLevel level)
        {
            var r = new Random();
            var outcome = r.Next(1, 3);
            if (outcome == 1)
                return;

            switch (level)
            {
                case EffectLevel.EffectLevelOne:
                    sendMessageInRadius(sender, 30f,"~p~ " + sender.name + "'s" + " stomach starts to rumble lightly.");
                    break;
                case EffectLevel.EffectLevelTwo:
                    sendMessageInRadius(sender, 30f, "~p~ " + sender.name + "'s" + " stomach starts to rumble loudly.");
                    break;
                case EffectLevel.EffectLevelThree:
                    sendMessageInRadius(sender, 30f,"~p~ " + sender.name + "'s" + " stomach starts to rumble violently.");
                    API.setPlayerHealth(sender, sender.health - 10);    
                    break;
            }
        }


        // Helper methods, as per.

        public void sendMessageInRadius(Client sender, float distance, string msg)
        {
            var playerList = API.getPlayersInRadiusOfPlayer(distance, sender);
            foreach (var p in playerList)
                API.sendChatMessageToPlayer(p, msg);
        }


        public static double convertMinToMilli(double min)
        {
            return min * 60000;
        }

    }
}