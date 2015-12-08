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
        int numActions = 3;
        Random r;


        public NNBasicStrategy(Net.Net net, double eps = .9, double lambda = .5)
        {
            this.net = net;
            this.eps = eps;
            this.lambda = .5;
            r = new Random();
        }
        //0 for stand, 1 for hit
        public int choosePlayerAction(Hand playerHand, Hand dealerHand, Deck deck)
        {
            int action = 0;
            int playerVal = playerHand.getValue();
            int dealerShown = dealerHand.getDealerShowing();
            var aceVal = playerHand.getAceValue();
            var dubVal = 0.0f;
            if (playerHand.canDouble())
                dubVal = 1.0f;
            var splVal = 0.0f;
            if (playerHand.canSplit())
                splVal = 1.0f;
            state = genInputVec(playerVal, dealerShown, deck, aceVal, dubVal, splVal);
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
                action = getMaxAct(qScores, playerHand); 
            }
            else // choose worst
            {
                action = getExploreAction(qScores, playerHand);
            }

            action = shouldSplit(playerHand, dealerHand, action, playerHand.canSplit());

            return action;
        }

        public float[] getQScores(Hand playerHand, Hand dealerHand, Deck deck)
        {
            int playerVal = playerHand.getValue();
            int dealerShown = dealerHand.getDealerShowing();
            var aceVal = playerHand.getAceValue();
            var dubVal = 0.0f;
            if (playerHand.canDouble())
                dubVal = 1.0f;
            var splVal = 0.0f;
            if (playerHand.canSplit())
                splVal = 1.0f;
            state = genInputVec(playerVal, dealerShown, deck, aceVal, dubVal, splVal);
            float[] qScores = new float[numActions];
            for (int act = 0; act < numActions; act++)
            {
                var input = actionPlusState(act, state);
                var a = net.forward(input);
                qScores[act] = a[0];
            }
            return qScores;
        }


        private int shouldSplit(Hand playerHand, Hand dealerHand, int action, bool canSplit)
        {
            if (!canSplit)
                return action;

            var ret = action;
            var playerVal = playerHand.getValue();
            var dealerShown = dealerHand.getDealerShowing();

            if (playerVal == 12 && playerHand.getAceValue() == 1.0f) //AA
                ret = 3;
            else if (playerVal == 18 && (dealerShown <= 6 || dealerShown == 8 || dealerShown == 9))
                ret = 3;
            else if (playerVal == 16)
                ret = 3;
            else if (playerVal == 14 && dealerShown <= 7)
                ret = 3;
            else if (playerVal == 12 && dealerShown <= 6)
                ret = 3;
            else if (playerVal == 8 && (dealerShown == 5 || dealerShown == 6))
                ret = 3;
            else if (playerVal == 6 && (dealerShown <= 7))
                ret = 3;
            else if (playerVal == 4 && (dealerShown <= 7))
                ret = 3;
            return ret;
        }

        private int getExploreAction(float[] qScores, Hand playerHand)
        {
            float max = -10000000.0f;
            int maxI = -1;
            bool canDub = playerHand.canDouble();
            bool canSpl = playerHand.canSplit();
            for (int i = 0; i < qScores.Length; i++)
            {
                if(i == 2 && !canDub)
                    continue;

                if (qScores[i] > max)
                {
                    max = qScores[i];
                    maxI = i;
                }
            }

            //now get a random worse action
            var a = maxI;
            while(a == maxI || ((!canDub && a == 2) || (!canSpl && a == 3)))
            {
                a = r.Next(0, qScores.Length);
            }

            return a;
        }

        private int getMaxAct(float[] qScores, Hand playerHand)
        {
            float max = -10000000.0f;
            int maxI = -1;
            int secondMax = -1;
            bool canDub = playerHand.canDouble();
            bool canSpl = playerHand.canSplit();
            for (int i = 0; i < qScores.Length; i++)
            {
                if (i == 2 && !canDub)
                    secondMax = maxI;
                if (qScores[i] > max)
                {
                    max = qScores[i];
                    maxI = i;
                }
            }
            //if we picked double and we cant dub
            if (!canDub && maxI == 2)
            {
                //hit with neg reward and pick second best
                runBackwards(-1.0f, 2);
                maxI = secondMax;

            }

            return maxI;
        }

        public void runBackwards(float reward, int actionTaken)
        {

            float[] goal = new float[1];
            goal[0] = reward;
            var x = actionPlusState(actionTaken, state);
            var y = net.forward(x);
            net.backward(goal);
        }

        public void runBackwardsHit(Hand newPlayer, Hand newDealer, Deck deck)
        {
            //get Q value from hitting
            var x = actionPlusState(1, state); 
            var q = net.forward(x);

            //get next Q value
            int playerVal = newPlayer.getValue();
            int dealerShown = newDealer.getDealerShowing();
            var aceVal = newPlayer.getAceValue();

            var newState = genInputVec(playerVal, dealerShown, deck, aceVal, 0.0f, 0.0f);

            float[] qScores = new float[numActions];
            for (int act = 0; act < 2; act++)
            {
                var input = actionPlusState(act, newState);
                var a = net.forward(input);
                qScores[act] = a[0];
            }

            //find max Q
            float max = -1000.0f;

            for (int i = 0; i < 2; i++)
            {
                if (qScores[i] > max)
                    max = qScores[i];
            }

            //discount reward
            var target = lambda * (max - q[0]);
            if(target > 0.0f)
            {
                var i = 1;
            }
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

            if (dealerShowing == 1)
                dealerShowing = 11;

            ret[dealerShowing - 2] = 1.0f;
            return ret;
        }

        private static float[] genInputVec(int playerVal, int dealerShowing, Deck deck, float aceVal, float dubVal, float splVal)
        {
            float[] ret = new float[38];

            var playerVec = playerValVec(playerVal);
            var dealerVec = dealerValVec(dealerShowing);
            var cardCountVec = deck.getDeckPercentages();
            for (int i = 0; i < ret.Length - 2; i++)
            {
                if (i < playerVec.Length)
                {
                    ret[i] = playerVec[i];
                }
                else if(i < playerVec.Length + dealerVec.Length)
                {
                    ret[i] = dealerVec[i - playerVec.Length];
                }
                else
                {
                    ret[i] = cardCountVec[i - (playerVec.Length + dealerVec.Length)];
                }
            }
            ret[36] = aceVal;
            ret[37] = dubVal;
            //ret[29] = splVal;
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
