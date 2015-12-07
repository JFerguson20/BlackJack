using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace BlackJack
{
    class Deck
    {
        List<int> cardsLeft;
        int count;
        public Deck(int numDecks)
        {
            cardsLeft = new List<int>();
            createCards(numDecks);
            count = 0;
        }

        private void createCards(int numDecks)
        {
            for(int i = 0; i < numDecks; i++)
            {   
                //add cards 2-9 (2 through 9)
                for(int card = 2; card <10; card++)
                {
                    //each suit
                    for(int j = 0; j < 4; j++)
                    {
                        cardsLeft.Add(card);
                    }
                }
                //add face cards
                for(int f = 0; f < 4; f++)
                {
                   //each suit
                   for(int k =0; k < 4; k++)
                    {
                        cardsLeft.Add(10);
                    }
                }
                //add aces
                for(int a = 0; a < 4; a++)
                {
                    cardsLeft.Add(11);
                }
            }
        }

        public void shuffleCards()
        {
            int n = cardsLeft.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                int value = cardsLeft[k];
                cardsLeft[k] = cardsLeft[n];
                cardsLeft[n] = value;
            }
        }

        public int getCard()
        {
            var ret = cardsLeft[0];
            cardsLeft.RemoveAt(0);

            //for card counting
            if (ret <= 6)
                count++;

            if (ret >= 10)
            {
                count--;
            }

            return ret;
        }

        public double getTrueCount()
        {
            //get decks left
            var decksLeft = (1.0 * cardsLeft.Count) / (1.0 * 52);
            var trueCount = (count * 1.0) / decksLeft;
            return trueCount;
        }

        //if less then a deck left then deck is done
        public bool isDeckFinished()
        {
            var ret = false;
            if(cardsLeft.Count < 52)
            {
                ret = true;
            }
            return ret;
        }
    }

    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }
}
