using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

public class chatcommands : Script
{
    // Generic test start up method, in use to ensure the thing actually loaded...
    public void testStartUp()
    {
        API.onResourceStart += startChatCommands;
    }

    public void startChatCommands()
    {
        API.consoleOutput("Chat commands are booted!");
    }

    // Command /attempt takes a String of an attempted action, and will randomly generate a outcome, based on a random seed of the system clock.
    [Command("attempt", GreedyArg = true)]
    public void startAttempt(Client sender, string attemptedAction)
    {
        // Pulls the players near, to allow for the message to be se    nt to players within the correct radius.
        List<Client> playersNear = API.getPlayersInRadiusOfPlayer(150.5f, sender);
        int outcome = new Random().Next(0, 2);

        string textOutcome = "and failed";
        if (outcome == 0)
        {
            textOutcome = "and succeeded";
        }
        foreach (Client p in playersNear)
        {
            API.sendChatMessageToPlayer(p, "~p~[ATTEMPT]  " + sender.handle + " has attempted to " + attemptedAction + " " + textOutcome);
        }
    }
    // The /roll command takes a string, based on the number of dices allowed. This is currently set to two. This can be changed via the UPPER_DICE_LIMIT const. See below.

    [Command("roll", GreedyArg = true)]
    public void startRoll(Client sender, String diceNo)
    {
        const int UPPER_DICE_LIMIT = 2;
        int numOfDie;
        int diceRoll;
        // Generate a random BEFORE the actual loop, same values are extremely likely when done inside of the loop, due to the seed being Sys time.
        Random roll = new Random();
        // Pulls the players near, to allow for the message to be sent to players within the correct radius.
        List<Client> playersNear = API.getPlayersInRadiusOfPlayer(150.5f, sender);
        if (Int32.TryParse(diceNo, out numOfDie))
        {
            int[] diceArr = new int[numOfDie];
            if(numOfDie > UPPER_DICE_LIMIT || numOfDie < 1)
            {
                API.sendChatMessageToPlayer(sender, "SYNTAX : /roll 1/2");
                return;
            }
            for(int x = 0; x <= numOfDie-1; x++)
            {
                diceRoll = roll.Next(1,7);
                API.sendChatMessageToAll(diceRoll.ToString());
                diceArr[x] = diceRoll;
            }
            foreach(Client p in playersNear)
            {
                API.sendChatMessageToPlayer(p, "~p~ "  + sender.handle + " "  + "has rolled " + numOfDie.ToString() + " dice and got " + String.Join(" and ", diceArr));
            }
        }
        else
            API.sendChatMessageToPlayer(sender, "SYNTAX : /roll 1/2");
            return;
        }

    // The /rand command, is used for generating a random number. This is not 0 indexed, like the one on SA:MP. 
    [Command("rand",GreedyArg = true)]
    public void startRand(Client sender, String upperBoundary)
    {
        const int MAX_LIMIT = 100;
        int upperlimit;
        List<Client> playersNear = API.getPlayersInRadiusOfPlayer(150.5f, sender);
        if(Int32.TryParse(upperBoundary,out upperlimit))
        {
            if (upperlimit <= MAX_LIMIT && upperlimit > 0)
            {
                int outcome = new Random().Next(0, upperlimit + 1);
                foreach(Client p in playersNear)
                {
                    API.sendChatMessageToPlayer(p, "~p~ (( " + sender.handle   + " randomised number " + outcome.ToString() + " out of " + upperlimit.ToString() + " )) ");
                }
            }
            else
                API.sendChatMessageToPlayer(sender, "SYNTAX : /rand 1-100");

        }
    }
    }