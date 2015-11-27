using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack
{
    class Program
    {
        private static Deck deck;
        static void Main(string[] args)
        {

            deck = new Deck(1);
            deck.shuffleCards();

        }

        // return 1 for player won and -1 for dealer one
        static int playHand()
        {
            bool canSplit = false;
            var playersVal = 0;
            var dealerVal = 0;

            //get initial players and dealers card
            var card = deck.getCard();
            //player first card
            if(card == 1)
            {
                playersVal = 11;
            }
            else
            {
                playersVal = card;
            }
            //get second card
            card = deck.getCard();
            if (card == playersVal)
                canSplit = true;

            if(card == 1)
            {
                if (playersVal >= 11)
                    playersVal += 1;
                else
                    playersVal += 11;
            }
            else
            {
                playersVal += card;
            }

            //get dealer card
            dealerVal = deck.getCard();
            
            //Now make the decisions

            return 1;
        }
    }
}
