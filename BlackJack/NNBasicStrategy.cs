using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack
{
    class NNBasicStrategy
    {
        Net.Net net;
        double eps;
        float[] lastInput;
        float[] a;
        Random r;
        public NNBasicStrategy(Net.Net net, double eps = .9)
        {
            this.net = net;
            this.eps = eps;
            r = new Random();
        }
        //0 for stand, 1 for hit
        public int choosePlayerAction(Hand playerHand, Hand dealerHand)
        {
            int action = 0;
            int playerVal = playerHand.getValue();
            int dealerShown = dealerHand.getDealerShowing();
            var aceVal = playerHand.getAceValue();

            lastInput = genInputVec(playerVal, dealerShown, aceVal);
            a = net.forward(lastInput);
            //eps greedy.
            if(playerVal < 10)
            {
                int k = 0;
            }
            if(eps > r.NextDouble()) //choose best action
            {
                if(a[0] >= 0.5)
                {
                    action = 1;
                }
                else
                {
                    action = 0;
                }
            }
            else // choose worst
            {
                if (a[0] <= 0.5)
                    action = 1;
                else
                    action = 0;
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

        public void runBackwards(float[] goal)
        {
           

            net.forward(lastInput);
            net.backward(goal);
        }

        private static float[] playerValVec(int playerVal)
        {
            //4-21 
            float[] ret = new float[18];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = 0.0f;
            ret[playerVal - 4] = 1.0f;
            return ret;
        }

        private static float[] dealerValVec(int dealerShowing)
        {
            //2-11
            float[] ret = new float[10];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = 0.0f;
            ret[dealerShowing - 2] = 1.0f;
            return ret;
        }

        private static float[] genInputVec(int playerVal, int dealerShowing, float aceVal)
        {
            float[] ret = new float[28];

            var playerVec = playerValVec(playerVal);
            var dealerVec = dealerValVec(dealerShowing);

            for (int i = 0; i < ret.Length - 1; i++)
            {
                if (i < playerVec.Length)
                {
                    ret[i] = playerVec[i];
                }
                else
                {
                    ret[i] = dealerVec[i - playerVec.Length];
                }
            }
            ret[27] = aceVal;
            return ret;
        }
    }
}
