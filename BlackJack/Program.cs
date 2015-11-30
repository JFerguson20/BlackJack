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
        {   /*
            int[] x = {10, 20, 10};
            var net = new Net.Net(4, x, 3);
            Random r = new Random();
            
            for(int i = 0; i < 10000000; i++)
            {
                var s = r.NextDouble();
                var s1 = r.NextDouble();
                var s2 = r.NextDouble();
                var s3 = r.NextDouble();
                float[] input = { (float)s, (float) s1, (float) s2, (float) s3};
                float[] goal = testFunc(s,s1, s2, s3);
                net.forward(input);
                net.backward(goal);
                if(i % 10000 == 0)
                {
                    Console.WriteLine(i);
                }
            }

            for(int j = 0; j < 20; j ++)
            {
                var t1 = r.NextDouble();
                var t2 = r.NextDouble();
                var t3 = r.NextDouble();
                var t4 = r.NextDouble();
                float[] input1 = { (float)t1, (float)t2, (float) t3, (float) t4};
                float[] goal1 = testFunc(t1, t2, t3, t4);
                var a = net.forward(input1);
                
                Console.WriteLine(a[0] + " " + a[1] + " " + a[2] + " : " + goal1[0] + " " + goal1[1] + " " + goal1[2]);
            }
            */


            /*
            deck.shuffleCards();
            deck = new Deck(1);
            */
            //simpleBasicStrategy(10000);
            simpleBlackjack(1000000, 10000);
            int i = 0;
        }
        
        //test with only hit and stand actions
        static private void simpleBlackjack(int numberOfHands, int testInterval)
        {

            int[] x = { 100 };
            //input, playersVal (17), dealersVal(10), playerHasAce(1)
            var net = new Net.Net(28, x, 2);
            Random r = new Random();
            var eps = .9; ;
            for(int i = 0; i < numberOfHands; i++)
            {
                if(i % testInterval == 0)
                {
                    var perc = NNsimpleBasicStrategy(net, 1000, 1.0);
                    Console.WriteLine(i + " : "+ perc);
                }
                else
                {
                    NNsimpleBasicStrategy(net, 1, eps, true);
                }
            }

        }

        private static void testSimpleBlackjack(Net.Net net)
        {
            
        }

        private static void simpleBasicStrategy(int numOfHands)
        {
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

            }
            Console.WriteLine("Wins: " + numWins);
            Console.WriteLine("Losses: " + numLosses);
            Console.WriteLine("Draws: " + numDraws);
            Console.WriteLine("BJ: " + numBlackJacks);
            Console.WriteLine("WinLoss: " + winLoss);
            var x = winLoss / (1.0 * numOfHands);
            Console.WriteLine("%: " + x);
        }

        private static double NNsimpleBasicStrategy(Net.Net net, int numOfHands, double eps, bool isTraining = false)
        {
            int totalHandsPlayed;
            double winLoss = 0.0; //-1 for loss, +1 for win. +1.5 for blackjack. 0 for draw

            //input, playersVal (17), dealersVal(10), playerHasAce(1)
            float[] goal = { 0.0f, 0.0f };
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

                        action = policy.choosePlayerAction(playerHand, dealerHand);
                    }

                    //see if we busted
                    if (playerHand.getValue() > 21)
                    {
                        numLosses++;
                        winLoss -= 1.0;
                        goal[0] = 0.0f;
                        goal[1] = -1.0f;
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
                            goal[0] = 1.0f;
                            goal[1] = 0.0f;
                        }
                        else if (dealerHand.getValue() < playerHand.getValue())
                        {
                            goal[0] = 1.0f;
                            goal[1] = 0.0f;

                            numWins++;
                            winLoss += 1.0f;
                        }
                        else if (dealerHand.getValue() == playerHand.getValue())
                        {
                            numDraws++;
                        }
                        else
                        {
                            goal[0] = -1.0f;
                            goal[1] = 0.0f;
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
