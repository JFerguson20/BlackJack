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
        }
        
        //test with only hit and stand actions
        static private void simpleBlackjack(int numberOfHands, int testInterval)
        {

            int[] x = { 10, 20, 10 };
            //input, playersVal (17), dealersVal(10), playerHasAce(1)
            var net = new Net.Net(28, x, 2);
            Random r = new Random();
            var eps = .9;
            for (int hand = 0; hand < numberOfHands; hand++)
            {
                
                if (hand % testInterval == 0)
                {
                    testSimpleBlackjack(net);
                }
                else
                {
                    var playersVal = 0;
                    var dealersVal = 0;
                    var playerHasAce = false;
                    var deck = dealInitCards(ref playersVal, ref dealersVal, ref playerHasAce);
                    var dealerHidden = deck.getCard();
                    if (dealerHidden == 1 && dealersVal != 11) dealerHidden = 11;

                    var handOver = false;
                    if (playersVal == 21)
                    {
                        //BlackJack!!!
                        handOver = true;
                    }

                    while(!handOver)
                    {
                        var playersTurn = true;
                        var aceVal = 0.0f;
                        if (playerHasAce) aceVal = 1.0f;
                        float[] input = genInputVec(playersVal, dealerHidden, aceVal);
                        float[] goal = { 0.0f, 0.0f };

                        //float[] goal = testFunc(s, s1, s2, s3);                        
                        while (playersTurn)
                        {
                            //action 0 is stand, action 1 is hit.
                            var choosenA = 0;
                            var a = net.forward(input);                        
                            if (eps > r.NextDouble()) //choose best action
                            {
                                if (a[0] > a[1])
                                    choosenA = 0;
                                else
                                    choosenA = 1;
                            }
                            else //choose worst action.
                            {
                                if (a[0] < a[1])
                                    choosenA = 0;
                                else
                                    choosenA = 1;
                            }

                            if (choosenA == 0) //stand
                            {
                                playersTurn = false;
                            }
                            else//hit
                            {                             
                                var newCard = deck.getCard();
                                if (newCard == 1) playerHasAce = true;

                                playersVal += newCard;
                                if (playersVal > 21)
                                {
                                    if(playerHasAce)
                                    {
                                        playersVal -= 10;
                                        playerHasAce = false;
                                    }
                                    else //we busted, goal is a neg for hit action
                                    {
                                        goal[0] = 0.0f;
                                        goal[1] = -1.0f;
                                        playersTurn = false;
                                    }
                                }
                                else //update input for next decision
                                {
                                    aceVal = 0.0f;
                                    if (playerHasAce) aceVal = 1.0f;
                                    input = genInputVec(playersVal, dealersVal, aceVal);                               
                    
                                }
                            }
                        }
                        //now dealers turn.                    
                        if (playersVal > 21) // if we busted, the dealer doesnt have to play
                        {
                            net.forward(input);
                            net.backward(goal);
                        }
                        else
                        {
                            dealersVal += dealerHidden;
                            bool dealersTurn = true;
                            while (dealersTurn)
                            {
                                if (dealersVal < 17)
                                {
                                    var card = deck.getCard();
                                    if (card == 1) card = 11;
                                    dealersVal += card;
                                    if (dealersVal > 21 && card == 11) dealersVal -= 10;
                                }
                                else
                                {
                                    dealersTurn = false;
                                }
                            }

                            //decide who wins
                            if (dealersVal > 21) // dealer busted
                            {
                                goal[0] = 1.0f;
                                goal[1] = 0.0f;
                            }
                            else if (dealersVal > playersVal)
                            {
                                goal[0] = -1.0f;
                                goal[1] = 0.0f;
                            }
                            else if (playersVal > dealersVal)
                            {
                                goal[0] = 1.0f;
                                goal[1] = 0.0f;
                            }

                            net.forward(input);
                            net.backward(goal);
                        }
                    }

                }
            }

        }

        private static void testSimpleBlackjack(Net.Net net)
        {
            
        }

        private static void simpleBasicStrategy()
        {

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
