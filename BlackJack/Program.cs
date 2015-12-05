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
            //simpleBasicStrategy(10000);
            simpleBlackjack(1000000, 1000);
            int i = 0;
        }
        
        

        //test with only hit and stand actions
        static private void simpleBlackjack(int numberOfHands, int testInterval)
        {

            int[] x = {20,20};
            List<double> percs = new List<double>();
            List<int> runNum = new List<int>();
            //input, playersVal (17), dealersVal(10), playerHasAce(1)
            var net = new Net.Net(28, x, 1);
            Random r = new Random();
            var eps = 1.0;
            //showPolicy(net);
            for (int i = 0; i < numberOfHands; i++)
            {
                if(i % testInterval == 0)
                {
                    var perc = NNsimpleBasicStrategy(net, 10000, 1.0);
                    percs.Add(perc);
                    runNum.Add(i);
                    Console.WriteLine(i);
                    //showPolicy(net);
                }
                else
                {
                    NNsimpleBasicStrategy(net, 1, eps, true);
                }
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
            var pol = new NNBasicStrategy(n, 1.0);
            //Try each possible input and get the output.
            for(int p = 21; p >= 4; p--)
            {
                for(int d = 2; d <= 11; d++)
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

        private static List<double> simpleBasicStrategy(int numOfHands)
        {
            var ret = new List<double>();
            int totalHandsPlayed;
            double winLoss = 0.0; //-1 for loss, +1 for win. +1.5 for blackjack. 0 for draw
            var policy = new BasicStrategy();
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

        private static double NNsimpleBasicStrategy(Net.Net net, int numOfHands, double eps, bool isTraining = false)
        {
            int totalHandsPlayed;
            double winLoss = 0.0; //-1 for loss, +1 for win. +1.5 for blackjack. 0 for draw

            //input, playersVal (17), dealersVal(10), playerHasAce(1)
            float[] goal = { 0.0f};
            var policy = new NNBasicStrategy(net, eps);
            int numBlackJacks = 0;
            int numWins = 0;
            int numDraws = 0;
            int numLosses = 0;
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


                if (playerHand.getValue() == 21 && dealerHand.getValue() != 21)
                {
                    //BLACK JACK
                    numBlackJacks++;
                    winLoss += 1.5;
                    noForwardPass = true;
                }
                else if (dealerHand.getValue() == 21)
                {
                    //Dealer BJ
                    numLosses++;
                    winLoss -= 1.0;
                    noForwardPass = true;
                }
                else
                {
                    //player decisions
                    var action = policy.choosePlayerAction(playerHand, dealerHand);
                    while (action == 1)
                    {
                        playerHand.addCards(deck.getCard());

                        if(playerHand.getValue() > 21)
                        {
                            break;
                        }
                        else
                        {
                            goal[0] = 1.0f;
                            
                            if (isTraining)
                            {
                                policy.runBackwards(goal);
                            }
                        }

                        action = policy.choosePlayerAction(playerHand, dealerHand);
                    }

                    //see if we busted
                    if (playerHand.getValue() > 21)
                    {
                        numLosses++;
                        winLoss -= 1.0;
                        goal[0] = 0.0f;
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
                            goal[0] = 0.0f;

                        }
                        else if (dealerHand.getValue() < playerHand.getValue())
                        {

                            winLoss += 1.0f;
                            goal[0] = 0.0f;
                            numWins++;
                            
                        }
                        else if (dealerHand.getValue() == playerHand.getValue())
                        {
                            numDraws++;
                        }
                        else
                        {
                            goal[0] = 1.0f;
                            numLosses++;
                            winLoss -= 1.0;
                        }

                    }
                }

            }
            
            if(isTraining)
            {
                if(numDraws == 0 && numBlackJacks != 1 && !noForwardPass)
                    policy.runBackwards(goal);
            }

            /*
            Console.WriteLine("Wins: " + numWins);
            Console.WriteLine("Losses: " + numLosses);
            Console.WriteLine("Draws: " + numDraws);
            Console.WriteLine("BJ: " + numBlackJacks);
            Console.WriteLine("WinLoss: " + winLoss);
            
            Console.WriteLine("%: " + x);
            */
            var x = winLoss / (1.0 * numOfHands);
            return x;
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
