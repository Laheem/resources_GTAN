using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;
using Cards;

    public class Player
    {
    public Dealer d { get; private set; }
    public Client player { get; private set; }
    public List<Card> hand { get; private set; }

    public Player(Dealer d, Client player)
    {
        this.d = d;
        this.player = player;
        hand = new List<Card>();


    }

        public void removeCards()
        {
            this.hand = new List<Card>();
        }
    }


