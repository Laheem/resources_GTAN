using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;
using GTANetworkShared;

namespace Cards
{
    public class Card
    {
        private String cardValue;
        private String suit;
        public static readonly string[] SUIT_NAMES = new String[4] { "Clubs", "Hearts", "Spades", "Diamonds" };
        public static readonly string[] CARD_VALUES = new string[13] { "Ace", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King" };

        public Card(String cardValue, String suit)
        {
            this.cardValue = cardValue;
            this.suit = suit;
        }

        public String getCardValue()
        {
            return cardValue;
        }

        public String getSuit()
        {
            return suit;
        }

        static public List<Card> generatePackOfCards()
        {
            List<Card> deck = new List<Card>();
            for (int x = 0; x <= SUIT_NAMES.Length - 1; x++)
            {
                for (int y = 0; y <= CARD_VALUES.Length - 1; y++)
                {
                    deck.Add(new Card(CARD_VALUES[y], SUIT_NAMES[x]));
                }
            }
            return deck;
        }


        public override String ToString()
        {
            return getCardValue() + " of " + getSuit();
        }
    }
}