using System;
using BlackJack.Net;

namespace BlackJack
{
    class NNBettingStrategy
    {
        private Net.Net net;
        private double eps;
        float[] state;
        private int numActions = 3;
        Random r;

        public NNBettingStrategy(Net.Net bettingNet, double eps)
        {
            net = bettingNet;
            this.eps = eps;
            r = new Random();
        }

        public int chooseBet(Deck deck)
        {
            int action = 0;
            state = deck.getBettingState();
            float[] qScores = new float[numActions];
            for (int act = 0; act < numActions; act++)
            {
                var input = actionPlusState(act, state);
                var a = net.forward(input);
                qScores[act] = a[0];
            }

            //do epsilon greedy action selection
            if (eps > r.NextDouble()) //choose best action
            {
                action = getMaxAct(qScores);
            }
            else // choose worst
            {
                action = getExploreAction(qScores);
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

        private int getExploreAction(float[] qScores)
        {
            float max = -10000000.0f;
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
            while (a == maxI)
            {
                a = r.Next(0, qScores.Length);
            }

            return a;
        }

        private int getMaxAct(float[] qScores)
        {
            float max = -10000000.0f;
            int maxI = -1;
            for (int i = 0; i < qScores.Length; i++)
            {
                if (qScores[i] > max)
                {
                    max = qScores[i];
                    maxI = i;
                }
            }
            return maxI;
        }

    }
}