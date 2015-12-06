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
        double lambda = .5;
        float[] state;
        float[] a;
        int numActions = 2;
        Random r;


        public NNBasicStrategy(Net.Net net, double eps = .9, double lambda = .5)
        {
            this.net = net;
            this.eps = eps;
            this.lambda = .5;
            r = new Random();
        }
        //0 for stand, 1 for hit
        public int choosePlayerAction(Hand playerHand, Hand dealerHand)
        {
            int action = 0;
            int playerVal = playerHand.getValue();
            int dealerShown = dealerHand.getDealerShowing();
            var aceVal = playerHand.getAceValue();

            state = genInputVec(playerVal, dealerShown, aceVal);
            float[] qScores = new float[numActions];
            for(int act = 0; act < numActions; act++)
            {
                var input = actionPlusState(act, state);
                var a = net.forward(input);
                qScores[act] = a[0];
            }

            //do epsilon greedy action selection
            if(eps > r.NextDouble()) //choose best action
            {
                action = getMaxAct(qScores); 
            }
            else // choose worst
            {
                action = getExploreAction(qScores);
            }

            return action;
        }

        private int getExploreAction(float[] qScores)
        {
            float max = -1000.0f;
            int maxI = -1;

            for (int i = 0; i < qScores.Length; i++)
            {
                if (qScores[i] > max)
                {
                    max = qScores[i];
                    maxI = i;
                }
            }

            //now get a random worse action
            var a = maxI;
            while(a == maxI)
            {
                a = r.Next(0, qScores.Length);
            }

            return a;
        }

        private int getMaxAct(float[] qScores)
        {
            float max = -1000.0f;
            int maxI = -1;

            for(int i = 0; i < qScores.Length; i++)
            {
                if(qScores[i] > max)
                {
                    max = qScores[i];
                    maxI = i;
                }
            }

            return maxI;
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

        public void runBackwards(float reward, int actionTaken)
        {

            float[] goal = new float[1];
            goal[0] = reward;
            var x = actionPlusState(actionTaken, state);
            var y = net.forward(x);
            net.backward(goal);
        }

        public void runBackwardsHit(Hand newPlayer, Hand newDealer)
        {
            //get Q value from hitting
            var x = actionPlusState(1, state); 
            var q = net.forward(x);

            //get next Q value
            int playerVal = newPlayer.getValue();
            int dealerShown = newDealer.getDealerShowing();
            var aceVal = newPlayer.getAceValue();

            var newState = genInputVec(playerVal, dealerShown, aceVal);

            float[] qScores = new float[numActions];
            for (int act = 0; act < numActions; act++)
            {
                var input = actionPlusState(act, newState);
                var a = net.forward(input);
                qScores[act] = a[0];
            }

            //find max Q
            float max = -1000.0f;

            for (int i = 0; i < qScores.Length; i++)
            {
                if (qScores[i] > max)
                    max = qScores[i];
            }

            //discount reward
            var target = lambda * (max - q[0]);
            //back propagate
            float[] goal = new float[1];
            goal[0] = (float)target;
            net.forward(x);
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

        private float[] actionPlusState(int act, float[] state)
        {
            var x = new float[numActions];
            for (int i = 0; i < numActions; i++)
            {
                x[i] = 0.0f;
            }

            x[act] = 1.0f; // set action we want to estimate

            var z = new float[x.Length + state.Length];
            state.CopyTo(z, 0);
            x.CopyTo(z, state.Length);
            return z;
        }
    }
}
