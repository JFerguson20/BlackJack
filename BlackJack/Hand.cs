using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack
{
    class Hand
    {

        private List<int> cards;
        public Hand()
        {
            cards = new List<int>();
        }

        public void addCards(int card)
        {
            cards.Add(card);
        }

        public List<int> getHand()
        {
            return cards;
        }

        public int getValue()
        {
            //calc value of hand.
            var val = 0;
            foreach(var card in cards)
            {
                val += card;
            }

            //see if we have an ace
            if(val > 21)
            {
                for(int i = 0; i < cards.Count; i++)
                {
                    if(cards[i] == 11)
                    {
                        cards[i] = 1;
                        val = getValue();
                        break;
                    }
                }
            }
            return val;
        }

        public int getDealerShowing()
        {
            int ret = cards[0];
            if (cards[0] == 1)
            {
                ret = cards[1];
            }
                
            return ret;
        }

        public float getAceValue()
        {
            var ret = 0.0f;
            foreach(var card in cards)
            {
                if(card == 11)
                {
                    ret = 1.0f;
                }
            }
            return ret;
        }
    }
}
