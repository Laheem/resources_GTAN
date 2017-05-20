using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;
using Cards;


public class Dealer
{


    public const int MAX_CARDS = 52;
    public Client dealerClient { get; private set; }
    public List<Card> deckOfCards { get; private set; }
    public List<Card> drawnCards { get; private set; }
    public List<Client> playerList { get; private set; }
    public Dictionary<Client,Player> players { get; private set; } 

    public Dealer(Client dealer)
    {
        this.dealerClient = dealer;
        this.deckOfCards = Card.generatePackOfCards();
        this.drawnCards = new List<Card>();
        this.players = new Dictionary<Client, Player>();
        this.playerList = new List<Client>();
    }

    public Player addPlayer(Client target)
    {
      
        Player newPlayer = new Player(this,target);
        players.Add(target, newPlayer);
        playerList.Add(target);

        return newPlayer;
    }

    public void removePlayer(Client target)
    {
        players.Remove(target);
        playerList.Remove(target);
    }

    public Card dealCard(Client target)
    {
        Random listChoice = new Random();
        if (drawnCards.Count == MAX_CARDS)
        {
            return null;
        }
        else
        {
            int cardNo = listChoice.Next(0, deckOfCards.Count - 1);
            drawnCards.Add(deckOfCards[cardNo]);
            Card chosenCard = deckOfCards.ElementAt(cardNo);
            deckOfCards.RemoveAt(cardNo);

            Player targPlayer;

            if (players.TryGetValue(target, out targPlayer))
            {

                targPlayer.hand.Add(chosenCard);
                return chosenCard;
            }

            return null;
        }

    }

    public void reshuffle()
    {
        Player targPlayer;
        foreach (Client p in playerList)
        {
            players.TryGetValue(p, out targPlayer);
            targPlayer.removeCards();

        }

        drawnCards = new List<Card>();
        deckOfCards = Card.generatePackOfCards();
    }


}
