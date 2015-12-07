using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackJack.Net;

namespace BlackJack
{
    class Program
    {
       
        static void Main(string[] args)
        {   
            //var x = simpleBasicStrategy(100000);
            simpleBlackjack(10000000, 50000);
            int i = 0;
        }
        
        

        //test with only hit and stand actions
        static private void simpleBlackjack(int numberOfHands, int testInterval)
        {

            int[] x = {150};
            List<double> percs = new List<double>();
            List<int> runNum = new List<int>();
            //input, playersVal (17), dealersVal(10), playerHasAce(1), doubleFlag(1), spltFlag(1), actions(4)
            var net = new Net.Net(32, x, 1);
            Random r = new Random();
            var eps = .6;
            //showPolicy(net);
            for (int i = 0; i < numberOfHands; i++)
            {
                if(i % testInterval == 0)
                {
                    var perc = NNsimpleBasicStrategy(net, 10000, 1.0);
                    percs.Add(perc);
                    runNum.Add(i);
                    Console.WriteLine(i + ": " +perc);
                    //set learning rate to perc
                    NetCore.learningRate = -1.0f * (float) (perc / 2.0);
                    if (NetCore.learningRate < 0.0f)
                        NetCore.learningRate = .001f;
                    //NetCore.learningRate = .2f;
                    Console.WriteLine(NetCore.learningRate);
                    //showPolicy(net);
                }
                else
                {
                    NNsimpleBasicStrategy(net, 1, eps, true);
                }

                if(i % 100000 == 0)
                    showPolicy(net);

            }
            showPolicy(net);
            //var basic = simpleBasicStrategy(1000000);
            //var random = simpleRandomStrategy(1000000);
            //writeToFile(runNum, percs, basic, random);
        }

        private static void writeToFile(List<int> runNum, List<double> percs, List<double> basic, List<double> random)
        {
            using (System.IO.StreamWriter file =
             new System.IO.StreamWriter("basicStrategyOut.csv"))
            {
                foreach (var run in runNum)
                {
                      file.Write(run + ",");
                }
                file.WriteLine();
                foreach (var run in percs)
                {
                    file.Write(run + ",");
                }
                file.WriteLine();
                foreach (var run in basic)
                {
                    file.Write(run + ",");
                }
                file.WriteLine();
                foreach (var run in random)
                {
                    file.Write(run + ",");
                }
            }


        }

        static private void showPolicy(Net.Net n)
        {
            Console.Write("    ");
            var pol = new NNBasicStrategy(n, 1.0);
            for (int d = 2; d <= 11; d++)
            {
                Console.Write(d + " ");
            }
            Console.WriteLine();
            //Try each possible input and get the output.
            for (int p = 21; p >= 4; p--)
            {
                if(p >= 10)
                    Console.Write(p + "  ");
                else
                    Console.Write(p + "   ");
                for (int d = 2; d <= 11; d++)
                {
                    Hand pH = new Hand();
                    Hand dH = new Hand();
                    pH.addCards(p);
                    dH.addCards(d);
                    var a = pol.choosePlayerAction(pH, dH);
                    Console.Write(a + " ");
                }
                Console.WriteLine();
            }

            //do for soft totals
            Console.WriteLine();
            Console.WriteLine("------ SOFTS ------");

            for (int p = 2; p <= 9; p++)
            {
                Console.Write(p + "   ");
                for (int d = 2; d <= 11; d++)
                {
                    Hand pH = new Hand();
                    Hand dH = new Hand();
                    pH.addCards(11);
                    pH.addCards(p);
                    dH.addCards(d);
                    var a = pol.choosePlayerAction(pH, dH);
                    Console.Write(a + " ");
                }
                Console.WriteLine();
            }

        }

        static private int maxAction(float[] a)
        {
            var ret = 0;
            if (a[0] > a[1])
                ret = 0;
            else
                ret = 1;
            return ret;
        }

        private static void testSimpleBlackjack(Net.Net net)
        {
            
        }

        private static double simpleBasicStrategy(int numOfHands)
        {
            var ret = new List<double>();
            int totalHandsPlayed;
            double winLoss = 0.0; //-1 for loss, +1 for win. +1.5 for blackjack. 0 for draw
            var policy = new BasicStrategy();
            //do each hand
            for (totalHandsPlayed = 0; totalHandsPlayed < numOfHands; totalHandsPlayed++)
            {
                Deck deck = new Deck(6);
                deck.shuffleCards();
                Hand playerHand = new Hand();
                Hand dealerHand = new Hand();
                //deal initial cards
                playerHand.addCards(deck.getCard());
                dealerHand.addCards(deck.getCard());
                playerHand.addCards(deck.getCard());
                dealerHand.addCards(deck.getCard());
                playHandBasic(ref deck, playerHand, ref dealerHand, ref policy, ref winLoss);
            }
            var x = winLoss / (numOfHands);
            return x;
        }

        private static double NNsimpleBasicStrategy(Net.Net net, int numOfHands, double eps, bool isTraining = false)
        {
            int totalHandsPlayed;
            double winLoss = 0.0; //-1 for loss, +1 for win. +1.5 for blackjack. 0 for draw

            //input, playersVal (17), dealersVal(10), playerHasAce(1)
            var policy = new NNBasicStrategy(net, eps);

            bool noForwardPass = false; 
            //do each hand
            for (totalHandsPlayed = 0; totalHandsPlayed < numOfHands; totalHandsPlayed++)
            {
                Deck deck = new Deck(6);
                deck.shuffleCards();
                Hand playerHand = new Hand();
                Hand dealerHand = new Hand();
                //deal initial cards
                playerHand.addCards(deck.getCard());
                dealerHand.addCards(deck.getCard());
                playerHand.addCards(deck.getCard());
                dealerHand.addCards(deck.getCard());

                playHand(ref deck, playerHand, ref dealerHand, ref policy, ref winLoss, isTraining);
            }

            var x = winLoss / (1.0 * numOfHands);
            return x;
        }

        private static int playDealer(ref Deck deck, ref Hand dealerHand)
        {
            var policy = new DealerPolicy();
            //play dealer
            var dealerAction = policy.chooseDealerAction(dealerHand);
            while (dealerAction == 1)
            {
                dealerHand.addCards(deck.getCard());
                dealerAction = policy.chooseDealerAction(dealerHand);
            }

            return dealerHand.getValue(); // return value of dealer hand.
        }
        
        private static float playHandBasic(ref Deck deck, Hand playerHand, ref Hand dealerHand, ref BasicStrategy policy, ref double winLoss)
        {
            var reward = 0.0f;
            var mult = 1.0f;
            //check for blackjack.
            if (playerHand.getValue() == 21 && dealerHand.getValue() != 21)
            {
                winLoss += 1.5;
                return 1.5f;
            }
            else if (dealerHand.getValue() == 21) //dealer got blackjack
            {
                winLoss -= 1.0;
                return 0.0f;
            }
            else
            {
                //player decisions
                var actionTaken = policy.choosePlayerAction(playerHand, dealerHand);

                if (actionTaken == 1)//hit
                {
                    //player decisions
                    var action = policy.choosePlayerAction(playerHand, dealerHand);
                    while (action == 1)
                    {
                        playerHand.addCards(deck.getCard());
                        action = policy.choosePlayerAction(playerHand, dealerHand);
                    }
                    //see if we busted
                    if (playerHand.getValue() > 21)
                    {
                        winLoss -= 1.0;
                    }
                    else
                    {
                        actionTaken = 0;
                    }

                }
                else if (actionTaken == 2) //double
                {
                    playerHand.addCards(deck.getCard());
                    if (playerHand.getValue() > 21)
                    {
                        winLoss -= 2.0;
                        reward = -1.0f;
                        mult = 2.0f;
                    }
                    else
                    {
                        mult = 2.0f;
                        actionTaken = 0;
                    }
                }
                else if (actionTaken == 3) //split
                {
                    Hand pH1 = new Hand();
                    Hand pH2 = new Hand();

                    var val = playerHand.getValue() / 2;
                    //split card and get an extra.
                    pH1.addCards(val);
                    pH2.addCards(val);
                    pH1.addCards(deck.getCard());
                    pH2.addCards(deck.getCard());

                    //win loss for the hands
                    reward = playHandBasic(ref deck, pH1, ref dealerHand, ref policy, ref winLoss);
                    reward += playHandBasic(ref deck, pH2, ref dealerHand, ref policy, ref winLoss);

                    winLoss += reward;
                }
                if (actionTaken == 0) //stand
                {
                    //play dealer
                    var dealerVal = playDealer(ref deck, ref dealerHand);
                    if (dealerVal > 21) //dealer busts
                    {
                        winLoss += 1.0 * mult;
                        reward = 1.0f * mult;

                    }
                    else if (dealerVal < playerHand.getValue()) //we beat dealer
                    {
                        winLoss += 1.0f * mult;
                        reward = 1.0f * mult;
                    }
                    else if (dealerVal == playerHand.getValue()) //draw
                    {
                        reward = 0.0f;
                    }
                    else //we lost to dealer
                    {
                        reward = -1.0f * mult;
                        winLoss -= 1.0 * mult;
                    }
                }
            }

            return reward;
        }

        private static float playHand(ref Deck deck, Hand playerHand, ref Hand dealerHand, ref NNBasicStrategy policy, ref double winLoss, bool isTraining)
        {
            var reward = 0.0f;
            var mult = 1.0f;
            //check for blackjack.
            if (playerHand.getValue() == 21 && dealerHand.getValue() != 21)
            {
                winLoss += 1.5;
                return 1.5f;
            }
            else if (dealerHand.getValue() == 21) //dealer got blackjack
            {
                winLoss -= 1.0;
                return 0.0f;
            }
            else 
            {
                //player decisions
                var actionTaken = policy.choosePlayerAction(playerHand, dealerHand);

                if (actionTaken == 1)//hit
                {
                    playHit(ref deck, ref playerHand, dealerHand, ref policy, isTraining);

                    if (playerHand.getValue() > 21)
                    {
                        winLoss -= 1.0;
                        reward = -1.0f;
                    }
                    else
                    {
                        actionTaken = 0;
                    }

                }
                else if (actionTaken == 2) //double
                {
                    playerHand.addCards(deck.getCard());
                    if (playerHand.getValue() > 21)
                    {
                        winLoss -= 2.0;
                        reward = -1.0f;
                        mult = 2.0f;
                    }
                    else
                    {
                        mult = 2.0f;
                        actionTaken = 0;
                    }
                }
                else if (actionTaken == 3) //split
                {
                    Hand pH1 = new Hand();
                    Hand pH2 = new Hand();

                    var val = playerHand.getValue() / 2;
                    //split card and get an extra.
                    pH1.addCards(val);
                    pH2.addCards(val);
                    pH1.addCards(deck.getCard());
                    pH2.addCards(deck.getCard());

                    //win loss for the hands
                    reward = playHand(ref deck, pH1, ref dealerHand, ref policy, ref winLoss, isTraining);
                    reward += playHand(ref deck, pH2, ref dealerHand, ref policy, ref winLoss, isTraining);

                    winLoss += reward;
                    policy.runBackwards(reward, actionTaken);
                }
                if(actionTaken == 0) //stand
                {
                    //play dealer
                    var dealerVal = playDealer(ref deck, ref dealerHand);
                    if (dealerVal > 21) //dealer busts
                    {
                        winLoss += 1.0 * mult;
                        reward = 1.0f * mult;

                    }
                    else if (dealerVal < playerHand.getValue()) //we beat dealer
                    {

                        winLoss += 1.0f * mult;
                        reward = 1.0f * mult;
                    }
                    else if (dealerVal == playerHand.getValue()) //draw
                    {
                        reward = 0.0f;
                    }
                    else //we lost to dealer
                    {
                        reward = -1.0f * mult;
                        winLoss -= 1.0 * mult;
                    }
                }

                if (isTraining)
                {
                    if (mult == 2.0f)
                        actionTaken = 2;

                    policy.runBackwards(reward, actionTaken);
                }

            }

            return reward;
        }

        private static void playHit(ref Deck deck, ref Hand playerHand, Hand dealerHand, ref NNBasicStrategy policy, bool isTraining)
        {
            var actionTaken = 1;
            while (actionTaken == 1) // hit
            {
                playerHand.addCards(deck.getCard());

                if (playerHand.getValue() > 21)
                {
                    break;
                }
                else
                {
                    if (isTraining)
                    {
                        policy.runBackwardsHit(playerHand, dealerHand);
                    }
                }

                //need to do delayed reward.
                actionTaken = policy.choosePlayerAction(playerHand, dealerHand);
            }
        }

        private static List<double> simpleRandomStrategy(int numOfHands)
        {
            var ret = new List<double>();
            int totalHandsPlayed;
            double winLoss = 0.0; //-1 for loss, +1 for win. +1.5 for blackjack. 0 for draw
            var policy = new RandomStrategy();
            int numBlackJacks = 0;
            int numWins = 0;
            int numDraws = 0;
            int numLosses = 0;
            //do each hand
            for (totalHandsPlayed = 0; totalHandsPlayed < numOfHands; totalHandsPlayed++)
            {
                Deck deck = new Deck(6);
                deck.shuffleCards();
                Hand playerHand = new Hand();
                Hand dealerHand = new Hand();
                //deal initial cards
                playerHand.addCards(deck.getCard());
                dealerHand.addCards(deck.getCard());
                playerHand.addCards(deck.getCard());
                dealerHand.addCards(deck.getCard());


                if (playerHand.getValue() == 21 && dealerHand.getValue() != 21)
                {
                    //BLACK JACK
                    numBlackJacks++;
                    winLoss += 1.5;
                }
                else if (dealerHand.getValue() == 21)
                {
                    //Dealer BJ
                    numLosses++;
                    winLoss -= 1.0;
                }
                else
                {
                    //player decisions
                    var action = policy.choosePlayerAction(playerHand, dealerHand);
                    while (action == 1)
                    {
                        playerHand.addCards(deck.getCard());
                        action = policy.choosePlayerAction(playerHand, dealerHand);
                    }

                    //see if we busted
                    if (playerHand.getValue() > 21)
                    {
                        numLosses++;
                        winLoss -= 1.0;
                    }
                    else
                    {
                        //play dealer
                        var dealerAction = policy.chooseDealerAction(dealerHand);
                        while (dealerAction == 1)
                        {
                            dealerHand.addCards(deck.getCard());
                            dealerAction = policy.chooseDealerAction(dealerHand);
                        }

                        if (dealerHand.getValue() > 21) //dealer busts
                        {
                            numWins++;
                            winLoss += 1.0;
                        }
                        else if (dealerHand.getValue() < playerHand.getValue())
                        {
                            numWins++;
                            winLoss += 1.0f;
                        }
                        else if (dealerHand.getValue() == playerHand.getValue())
                        {
                            numDraws++;
                        }
                        else
                        {
                            numLosses++;
                            winLoss -= 1.0;
                        }

                    }
                }

                if(totalHandsPlayed % 1000 == 0)
                {
                    var x = winLoss / (1000.0);
                    ret.Add(x);
                    numWins = 0;
                    numLosses = 0;
                    numDraws = 0;
                    numBlackJacks = 0;
                    winLoss = 0.0;
                }

            }
            Console.WriteLine("Wins: " + numWins);
            Console.WriteLine("Losses: " + numLosses);
            Console.WriteLine("Draws: " + numDraws);
            Console.WriteLine("BJ: " + numBlackJacks);
            Console.WriteLine("WinLoss: " + winLoss);

            return ret;
        }

        private static float[] playerValVec(int playerVal)
        {
            //4-21 
            float[] ret = new float[17];
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

            for(int i = 0; i < ret.Length; i++)
            {
                if( i < playerVec.Length)
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



            /*
        static private float[] testFunc(double a, double b, double c, double d)
        {
            float[] ret = new float[3];


            ret[0] = (float)(Math.Sin(a) * Math.Cos(c + d) + b);
            ret[1] = (float)(Math.Cos(b) *c);
            ret[2] = (float)(Math.Sin(b + d)); 

            return ret;
        }
        */
        // return 1 for player won and -1 for dealer one
        static Deck dealInitCards(ref int playersVal, ref int dealerShown, ref bool playerHasAce)
        {
            var deck = new Deck(1);
            playerHasAce = false;
            deck.shuffleCards();
            playersVal = deck.getCard();
            //get initial players and dealers card
            var card = deck.getCard();
            //player first card
            if (card == 1)
            {
                playersVal = 11;
                playerHasAce = true;
            }
            else
            {
                playersVal = card;
            }
            //get second card
            card = deck.getCard();

            if (card == 1)
            {
                playerHasAce = true;
                if (playersVal >= 11)
                    playersVal += 1;
                else
                    playersVal += 11;
            }
            else
            {
                playersVal += card;
            }

            //get dealer showing card
            dealerShown = deck.getCard();
            if (dealerShown == 1) dealerShown = 11;
            return deck;
        }
    }
}
