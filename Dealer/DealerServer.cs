using System.Collections.Generic;
using Cards;
using GTANetworkServer;
using GTANetworkShared;

internal class DealerServer : Script
{
    private const int MAX_CARDS = 52;
    private const int FORMAT_NO = 3;

    private readonly Dictionary<Client, Player> allPlayers = new Dictionary<Client, Player>();
    // Global lists which are used for the checking of which Clients are players/dealers and which aren't. Sorry.

    private readonly Dictionary<Client, Dealer> dealers = new Dictionary<Client, Dealer>();


    public DealerServer()
    {
        API.onResourceStart += dealerStart;
        API.onClientEventTrigger += dealerClientCalled;
        API.onPlayerDisconnected += removeDealer;
    }

    // Javascript calls for when a Menu button has been pressed, which will call one of the following cases.
    private void dealerClientCalled(Client sender, string eventName, params object[] arguments)
    {
        switch (eventName)
        {
            case "dealList":
                Dealer d;
                if (dealers.TryGetValue(sender, out d))
                    API.triggerClientEvent(sender, "listedPlayers", API.toJson(d.playerList.ToArray()));
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
                var toChange = (NetHandle) arguments[1];
                var playerFromHandle = API.getPlayerFromHandle(toChange);
                sendInvite(sender, playerFromHandle);
                break;
            case "invite":
                var player = (NetHandle) arguments[1];
                var p = API.getPlayerFromHandle(player);
                inviteOutCome(sender, p, (bool) arguments[0]);
                break;
            case "peek":
                peekAtCards(sender);
                break;
            case "leave":
                leaveGame(sender);
                break;
            case "fold":
                foldCards(sender);
                break;
            case "peekboard":
                peekAtBoard(sender);
                break;
            case "dealBoard":
                dealCardToBoard(sender);
                break;
            default:
                break;
        }
    }

    private void dealerStart()
    {
        API.consoleOutput("Print Cards is added. Not that.");
        API.consoleOutput("Dealers, now with more menus! Dealer is ready!");
    }

    private void removeDealer(Client player, string reason)
    {
        Dealer leavingPlayer;
        Player p;

        if (dealers.TryGetValue(player, out leavingPlayer))
            foreach (var c in leavingPlayer.playerList)
            {
                leavingPlayer.removePlayer(c);
                allPlayers.Remove(c);
                API.sendNotificationToPlayer(c, "Dealer has left the server, you have been removed from the game.");
            }
        if (allPlayers.TryGetValue(player, out p))
        {
            p.d.removePlayer(player);
            API.sendNotificationToPlayer(p.d.dealerClient,
                player.handle + " has disconnected. Removing from the game.");
        }
    }

    // Calls clientside which enables the JS menu. This is used for drawing cards and giving them to a player. JS needs fixing.
    [Command("/dealcards")]
    public void startDeal(Client sender)
    {
        Dealer d;
        if (dealers.TryGetValue(sender, out d))
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
        Dealer d;
        if (!dealers.TryGetValue(sender, out d))
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
        Dealer dealer;
        if (dealers.TryGetValue(sender, out dealer))
            API.triggerClientEvent(sender, "dealerMenu", dealer.playerList);
        else
            API.sendNotificationToPlayer(sender, "You need to be a dealer to use this menu!");
    }

    // Opens the playerlist, this allows them to either peek or fold. 
    [Command("playermenu")]
    public void startPlayerMenu(Client sender)
    {
        Player p;
        if (allPlayers.TryGetValue(sender, out p))
        {
            API.triggerClientEvent(sender, "playerMenu");
            return;
        }
        API.sendNotificationToPlayer(sender, "You're not in any games.");
    }

    // Adds a player to a Dealer's game. This does the checks to ensure they're already not in another game.
    public void joinGame(Client player, Client dealer)
    {
        Dealer playerDealer;
        Player p;
        if (allPlayers.TryGetValue(player, out p))
        {
            API.sendNotificationToPlayer(player, "Unable to join game : Already in a game.");
            API.sendNotificationToPlayer(dealer, "Player unable to join : Already in game.");
            return;
        }
        if (!dealers.TryGetValue(dealer, out playerDealer))
        {
            API.sendNotificationToPlayer(dealer, "Unable to send invites : You're not a dealer!");
            return;
        }
        {
            var newPlayer = playerDealer.addPlayer(player);
            allPlayers.Add(player, newPlayer);
        }
    }

    // Deals a card to a selected player, checks if the dealer has ran out of cards.
    public void dealCard(Client sender, Client player)
    {
        Dealer playerDealer;
        if (dealers.TryGetValue(sender, out playerDealer))
        {
            var drawnCard = playerDealer.dealCard(player);
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
    }

    // Shuffles the dealers deck, and returns all hands back to the dealer.
    public void shuffle(Client sender)
    {
        Dealer dealer;
        if (dealers.TryGetValue(sender, out dealer))
        {
            dealer.reshuffle();
            API.sendNotificationToPlayer(sender, "Cards reshuffled.");
            sendMessageInRadius(sender, 30f, "~p~[DEALER] : " + sender.name + " has reshuffled the deck.");
        }
    }

    // Reveals all hands in the game.
    public void revealHands(Client sender)
    {
        Dealer dealer;
        Player currPlayer;
        if (dealers.TryGetValue(sender, out dealer) && dealer.playerList.Count > 1)
        {
            foreach (var p in dealer.playerList)
                if (dealer.players.TryGetValue(p, out currPlayer))
                {
                    sendMessageInRadius(sender, 30f,
                        "~p~[DEALER]: Player " + p.name + " has revealed the following cards : ");
                    printCards(sender, currPlayer.hand, FORMAT_NO);
                }
        }
        else
        {
            API.sendNotificationToPlayer(sender,"You have no players, there's no hands to reveal.");
        }
    }

    // Player implmentations, all of these are using JS calls to trigger, not commands. 

    public void peekAtCards(Client sender)
    {
        Player peekingPlayer;
        if (allPlayers.TryGetValue(sender, out peekingPlayer))
        {
            if (peekingPlayer.hand.Count == 0)
            {
                API.sendNotificationToPlayer(sender, "You don't have any cards!");
                return;
            }
            sendMessageInRadius(sender, 30f, " ~p~[DEALER]: " + sender.name + " has peeked at his cards... ");

            API.sendChatMessageToPlayer(sender, "~p~[PLAYER]: You hold the following cards : ");
            printCards(sender, peekingPlayer.hand, FORMAT_NO);
            // Formats the cards in a more readable way, just change the Format_No dependant on how many cards you want on a line.
        }
    }

    public void dealCardToBoard(Client sender)
    {
        Dealer d;
        dealers.TryGetValue(sender, out d);
        var card = d.dealToBoard();
        if (card == null)
        {
            API.sendNotificationToPlayer(sender, "Out of cards, reshuffle.");
            return;
        }
        API.sendChatMessageToPlayer(sender,"~p~[DEALER]: You dealt the " + card + " to the board.");
        foreach (var c in d.playerList)
            API.sendChatMessageToPlayer(c,
                "~p~[DEALER]: The dealer has dealt the " + card + " to the board.");
    }
      

    // Removes the Player from the game. This is called in the Player Menu.
    public void leaveGame(Client sender)
    {
        Player playerToRemove;

        if (allPlayers.TryGetValue(sender, out playerToRemove))
        {
            foreach (var p in playerToRemove.d.playerList)
                API.sendNotificationToPlayer(p, sender.name + " has left the game.");

            playerToRemove.d.removePlayer(sender);
            allPlayers.Remove(sender);
        }
    }

    public void peekAtBoard(Client sender)
    {
        Player p;
        allPlayers.TryGetValue(sender, out p);
        if (p.d.board.Count == 0)
        {
            API.sendNotificationToPlayer(sender, "The board is empty!");
            return;
        }
        API.sendChatMessageToPlayer(sender, "~p~[DEALER]: The board contains : ");
        printCards(sender,p.d.board,FORMAT_NO);

    }


    public void foldCards(Client sender)
    {
        Player p;

        if (allPlayers.TryGetValue(sender, out p))
        {
            if (p.hand.Count == 0)
            {
                API.sendNotificationToPlayer(sender,"You don't have any cards to fold. Sorry!");
            }
            p.removeCards();
            foreach (var c in p.d.playerList)
                API.sendNotificationToPlayer(c, p.player.name + " has folded their cards.");
        }
    }


    // Sends an invite to a target player, opens the JS menu for a notification like menu. 
    public void sendInvite(Client sender, Client target)
    {
        Dealer d;
        Player p;
        if (!dealers.TryGetValue(sender, out d))
        {
            API.sendNotificationToPlayer(sender, "Unable to send invite. You're not a dealer.");
            return;
        }
        if (allPlayers.TryGetValue(target, out p))
        {
            API.sendNotificationToPlayer(sender, "Player is already in a game.");
            return;
        }

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
        Dealer d;

        if (dealers.TryGetValue(sender, out d) && target.exists)
        {
            sendInvite(sender, target);
            return;
        }
        API.sendChatMessageToPlayer(sender,
            "Unable to add the player. You may not be a dealer or they may be in a game.");
    }

    [Command("dealtoplayer", GreedyArg = true)]
    public void startDealToPlayer(Client sender, Client target)
    {
        Dealer d;

        if (dealers.TryGetValue(sender, out d) && target.exists && d.playerList.Contains(target))
        {
            dealCard(sender, target);
            return;
        }
        API.sendChatMessageToPlayer(sender,
            "Unable to deal a card. You may not be a dealer or they are not in your game.");
    }


    // Utility Commands.
    public void sendMessageInRadius(Client sender, float distance, string msg)
    {
        var playerList = API.getPlayersInRadiusOfPlayer(distance, sender);
        foreach (var p in playerList)
            API.sendChatMessageToPlayer(p, msg);
    }

    public void printCards(Client recipient, List<Card> allCards, int formatNo)
    {
        var tempStore = new List<Card>();
        if (allCards.Count == 0 || formatNo == 0)
            return;
        foreach (var c in allCards)
        {
            tempStore.Add(c);
            if (tempStore.Count != formatNo) continue;
            sendMessageInRadius(recipient, 30f, string.Join(", ", tempStore));
            tempStore = new List<Card>();
        }
        if (tempStore.Count > 0)
            sendMessageInRadius(recipient, 30f, string.Join(", ", tempStore));
    }
}