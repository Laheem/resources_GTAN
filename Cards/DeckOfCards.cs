using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;
using Cards;

class DeckOfCards : Script
{

    Dictionary<Client, List<Card>> playerCards = new Dictionary<Client, List<Card>>();
    Dictionary<Client, List<Card>> drawnCards = new Dictionary<Client, List<Card>>();

    public DeckOfCards()
    {
        API.onResourceStart += onDeckOfCardsStart;
        API.onClientEventTrigger += clientActivityTrigger;

    }

    private void clientActivityTrigger(Client sender, string eventName, params object[] arguments)
    {
        switch (eventName)
        {
            case "startDraw":
                startDrawSelectionMade(sender, arguments[0].ToString());
                break;
            case "revealCards":
                revealCards(sender);
                break;
            case "shuffleCards":
                shuffleCards(sender);
                break;
            default:
                break;
        }
    }

    private void onDeckOfCardsStart()
    {
        API.consoleOutput("Deal 'em. Cards module is ready!");
    }

    [Command("giveCards")]
    public void startGiveCards(Client sender)
    {
        List<Card> valueCheck;
        if (!playerCards.TryGetValue(sender, out valueCheck))
        {
            playerCards.Add(sender, Card.generatePackOfCards());
            drawnCards.Add(sender, new List<Card>());
            API.sendChatMessageToPlayer(sender, "You've been given a pack of cards.");
        }
        else
            API.sendChatMessageToPlayer(sender, "You already have a pack of cards!");
    }

    [Command("Draw")]
    public void startDraw(Client sender)
    {
        List<Card> valCheck;
        if (playerCards.TryGetValue(sender, out valCheck))
        {
            API.triggerClientEvent(sender, "drawACard");
        }
        else API.sendChatMessageToPlayer(sender, "Err... you need cards to draw cards. Sorry about that.");
    }

    public void startDrawSelectionMade(Client sender, string dealType)
    {
        Random listChoice = new Random();
        List<Card> playerCheck;
        List<Card> drawnCardList;

        if (playerCards.TryGetValue(sender, out playerCheck) && drawnCards.TryGetValue(sender, out drawnCardList))
        {
            if (playerCheck.Count == 0)
            {
                API.sendNotificationToPlayer(sender, "Out of cards, reshuffle.");
                return;
            }
        }
        else
            return;

        int cardNo = listChoice.Next(0, playerCheck.Count - 1);
        if (dealType.Equals("private"))
        {
            API.sendChatMessageToPlayer(sender, "~p~[PRIVATE DEALER]: " + playerCheck[cardNo]);

            drawnCardList.Add(playerCheck[cardNo]);
            playerCheck.RemoveAt(cardNo);

            sendMessageInRadius(sender, 20.0f, "~p~[DEALER]: " + sender.name + " has privately drawn a card...");
        }
        else if (dealType.Equals("public"))
        {
            sendMessageInRadius(sender, 20.5f, "~p~[DEALER]: " + sender.name + " has publically drawn the " + playerCheck[cardNo]);
            drawnCardList.Add(playerCheck[cardNo]);
            playerCheck.RemoveAt(cardNo);

        }
    }

    public void revealCards(Client sender)
    {
        List<Card> draws;
        if (drawnCards.TryGetValue(sender, out draws))
        {
            foreach (Card c in draws)
            {
                sendMessageInRadius(sender, 20.0f, sender.name + " has revealed the card - " + c.ToString());
            }
        }
    }

    public void shuffleCards(Client sender)
    {
        List<Card> deck;
        if (playerCards.TryGetValue(sender, out deck))
        {
            playerCards.Remove(sender);
            playerCards.Add(sender, Card.generatePackOfCards());
 
            drawnCards.Remove(sender);
            drawnCards.Add(sender, new List<Card>());

            API.sendNotificationToPlayer(sender, "You have shuffled your cards.");

            List<Client> playerList = API.getPlayersInRadiusOfPlayer(20.0f, sender);
            sendMessageInRadius(sender, 20.0f, sender.name + " has shuffled the cards.");
        }
        else
            API.sendNotificationToPlayer(sender, "No cards in the first place.");
    }



    public void sendMessageInRadius(Client sender, float distance, string msg)
    {
        List<Client> playerList = API.getPlayersInRadiusOfPlayer(distance, sender);
        foreach (Client p in playerList)
        {
            API.sendChatMessageToPlayer(p, msg);
        }
    }
}
