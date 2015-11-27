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

        public Deck(int numDecks)
        {
            cardsLeft = new List<int>();
            createCards(numDecks); 
        }

        private void createCards(int numDecks)
        {
            for(int i = 0; i < numDecks; i++)
            {   
                //add cards 1-9 (ace through 9)
                for(int card = 1; card <10; card++)
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
