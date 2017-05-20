using Cards;
using GTANetworkServer;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using GTANetworkShared;



class DealerServer : Script
{
    // Global lists which are used for the checking of which Clients are players/dealers and which aren't. Sorry.

    Dictionary<Client, Dealer> dealers = new Dictionary<Client, Dealer>();
    Dictionary<Client, Player> allPlayers = new Dictionary<Client, Player>();
    private const int FORMAT_NO = 3;


    public DealerServer()
    {
        API.onResourceStart += dealerStart;
        API.onClientEventTrigger += dealerClientCalled;
    }

    // Javascript calls for when a Menu button has been pressed, which will call one of the following cases.
    private void dealerClientCalled(Client sender, string eventName, params object[] arguments)
    {
        switch (eventName)
        {
            case "dealList":
                Dealer d;
                if (dealers.TryGetValue(sender, out d))
                {
                    API.triggerClientEvent(sender, "listedPlayers", API.toJson(d.playerList.ToArray()));
                }
                break;
            case "revealCards":
                revealHands(sender);
                break;
            case "shuffle":
                shuffle(sender);
                break;
            case "inviteList":
                API.triggerClientEvent(sender, "possiblePlayers", API.getPlayersInRadiusOfPlayer(30f, sender));
                break;
            case "sendInvite":
                NetHandle toChange = (NetHandle)arguments[1];
                Client playerFromHandle = API.getPlayerFromHandle(toChange);
                sendInvite(sender, playerFromHandle);
                break;
            case "invite":
                NetHandle player = (NetHandle)arguments[1];
                Client p = API.getPlayerFromHandle(player);
                inviteOutCome(sender, p, (bool)arguments[0]);
                break;
            case "peek":
                peekAtCards(sender);
                break;
            case "leave":
                leaveGame(sender);
                break;
            default:
                break;

        }
    }

    private void dealerStart()
    {
        API.consoleOutput("Dealers, now with more menus! Dealer is ready!");
    }


    // Calls clientside which enables the JS menu. This is used for drawing cards and giving them to a player. JS needs fixing.
    [Command("/dealcards")]
    public void startDeal(Client sender)
    {
        if (dealers.TryGetValue(sender, out Dealer d))
        {
            API.triggerClientEvent(sender, "listedPlayers");
            return;
        }
        API.sendNotificationToPlayer(sender, "You're not a dealer!");
    }

    // Magic command used as an alternative to a job system for now, easily changed.
    [Command("becomeDealer")]
    public void becomeADealer(Client sender)
    {
        if (!(dealers.TryGetValue(sender, out Dealer d)))
        {
            dealers.Add(sender, new Dealer(sender));
            return;
        }
        API.sendNotificationToPlayer(sender, "Already a dealer.");

    }

    // Opens the dealer menu, see the TS file for more infomation.
    [Command("dealermenu")]
    public void startDealerMenu(Client sender)
    {
        if (dealers.TryGetValue(sender, out Dealer dealer))
        {
            API.triggerClientEvent(sender, "dealerMenu", dealer.playerList);
        }
        else
        {
            API.sendNotificationToPlayer(sender, "You need to be a dealer to use this menu!");
        }
    }

    // Opens the playerlist, this allows them to either peek or fold. 
    [Command("playermenu")]
    public void startPlayerMenu(Client sender)
    {
        if (allPlayers.TryGetValue(sender, out Player p))
        {
            API.triggerClientEvent(sender, "playerMenu");
            return;
        }
        API.sendNotificationToPlayer(sender, "You're not in any games.");
    }

    // Adds a player to a Dealer's game. This does the checks to ensure they're already not in another game.
    public void joinGame(Client player, Client dealer)
    {
        if (allPlayers.TryGetValue(player, out Player p))
        {
            API.sendNotificationToPlayer(player, "Unable to join game : Already in a game.");
            API.sendNotificationToPlayer(dealer, "Player unable to join : Already in game.");
            return;
        }
        if (!(dealers.TryGetValue(dealer, out Dealer playerDealer)))
        {
            API.sendNotificationToPlayer(dealer, "Unable to send invites : You're not a dealer!");
            return;
        }
        Player newPlayer = playerDealer.addPlayer(player);
        allPlayers.Add(player, newPlayer);
    }

    // Deals a card to a selected player, checks if the dealer has ran out of cards.
    public void dealCard(Client sender, Client player)
    {
        if (!dealers.TryGetValue(sender, out Dealer playerDealer)) return;
        Card drawnCard = playerDealer.dealCard(player);
        if (drawnCard == null)
        {
            API.sendNotificationToPlayer(sender, "Unable to draw cards, reshuffle.");
        }
        else
        {
            sendMessageInRadius(sender, 30f, "~p~[DEALER]: " + sender.name + " has dealt a card to " + player.name);
            API.sendChatMessageToPlayer(player, "~p~[DEALER]: You have been dealt the " + drawnCard);
        }
    }

    // Shuffles the dealers deck, and returns all hands back to the dealer.
    public void shuffle(Client sender)
    {
        if (dealers.TryGetValue(sender, out Dealer dealer))
        {
            dealer.reshuffle();
            API.sendNotificationToPlayer(sender, "Cards reshuffled.");
            sendMessageInRadius(sender, 30f, "~p~ [DEALER] : " + sender.name + " has reshuffled the deck.");

        }
    }

    // Reveals all hands in the game.
    public void revealHands(Client sender)
    {
        Player currPlayer;
        if (!dealers.TryGetValue(sender, out Dealer dealer)) return;
        {
            foreach (Client p in dealer.playerList)
            {
                if (!dealer.players.TryGetValue(p, out currPlayer)) continue;
                sendMessageInRadius(sender, 30f, "~p~[DEALER]: Player " + p.name + " has revealed the following cards : ");
                List<Card> list = new List<Card>();
                foreach (Card c in currPlayer.hand)
                {
                    list.Add(c);
                    if (list.Count == FORMAT_NO)
                    {
                        sendMessageInRadius(sender, 30f, String.Join(", ", list));
                        list = new List<Card>();
                    }
                }
                sendMessageInRadius(sender, 30f, String.Join(", ", list));
                list = new List<Card>();
            }
        }
    }

    // Player implmentations, all of these are using JS calls to trigger, not commands. 

    public void peekAtCards(Client sender)
    {
        Player peekingPlayer;
        if (!allPlayers.TryGetValue(sender, out peekingPlayer)) return;
        sendMessageInRadius(sender, 30f, " ~p~ [DEALER]: " + sender.name + " has peeked at his cards... ");
        API.sendChatMessageToPlayer(sender, "~p~[PLAYER]: You hold the following cards : ");

        // Formats the cards in a more readable way, just change the Format_No dependant on how many cards you want on a line.
        List<Card> test = new List<Card>();

        foreach (Card c in peekingPlayer.hand)
        {
            test.Add(c);
            if (test.Count != FORMAT_NO) continue;
            API.sendChatMessageToPlayer(sender, String.Join(", ", test));
            test = new List<Card>();
        }
        API.sendChatMessageToPlayer(sender, String.Join(", ", test));
    }

    // Removes the Player from the game. This is called in the Player Menu.
    public void leaveGame(Client sender)
    {
        if (!allPlayers.TryGetValue(sender, out Player playerToRemove)) return;
        foreach (Client p in playerToRemove.d.playerList)
        {
            API.sendNotificationToPlayer(p, sender.name + " has left the game.");
        }

        playerToRemove.d.removePlayer(sender);
        allPlayers.Remove(sender);
    }

    // Sends an invite to a target player, opens the JS menu for a notification like menu. 
    public void sendInvite(Client sender, Client target)
    {
        API.triggerClientEvent(target, "invite", sender);
    }

    public void inviteOutCome(Client target, Client dealer, bool outcome)
    {
        if (outcome)
        {
            joinGame(target, dealer);
        }
        else
        {
            API.sendNotificationToPlayer(target, "You have rejected the invite.");
            API.sendNotificationToPlayer(dealer, "User has rejected your invite.");
        }

        API.sendChatMessageToAll(outcome.ToString());
    }


    //Temp Commands - 


    [Command("addplayer", GreedyArg = true)]
    public void startAddPlayer(Client sender, Client target)
    {

        if (dealers.TryGetValue(sender, out Dealer d) && !allPlayers.TryGetValue(target, out Player p))
        {
            sendInvite(sender, target);
            return;
        }
        API.sendChatMessageToPlayer(sender, "Unable to add the player. You may not be a dealer or they may be in a game.");

    }

    [Command("dealtoplayer", GreedyArg = true)]
    public void startDealToPlayer(Client sender, Client target)
    {

        if (dealers.TryGetValue(sender, out Dealer d) && (target.exists && d.playerList.Contains(target)))
        {
            dealCard(sender, target);
            return;
        }
        API.sendChatMessageToPlayer(sender, "Unable to deal a card. You may not be a dealer or they are not in your game.");
    }


    // Utility Commands.
    public void sendMessageInRadius(Client sender, float distance, string msg)
    {
        List<Client> playerList = API.getPlayersInRadiusOfPlayer(distance, sender);
        foreach (Client p in playerList)
        {
            API.sendChatMessageToPlayer(p, msg);
        }
    }


}


