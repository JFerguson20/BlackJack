using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack
{
    class RandomStrategy
    {
        Random r;
        public RandomStrategy()
        {
            r = new Random();
        }

        //0 for stand, 1 for hit
        public int choosePlayerAction(Hand playerHand, Hand dealerHand)
        {
            var d =r.NextDouble();
            var action = 0;
            if(d > .5)
            {
                action = 1;
            }
            return action;
        }

        public int chooseDealerAction(Hand dealerHand)
        {
            int action = 0;
            var dealerVal = dealerHand.getValue();

            if (dealerVal <= 16) //hit 16 and less
            {
                action = 1;
            }

            if (dealerVal >= 17) //stand 17 and more
            {
                action = 0;
            }

            return action;
        }

    }
}
