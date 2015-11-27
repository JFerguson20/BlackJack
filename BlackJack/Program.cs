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
        private static Deck deck;
        static void Main(string[] args)
        {
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
           

            
            /*
            deck.shuffleCards();
            deck = new Deck(1);
            */
        }

        static private float[] testFunc(double a, double b, double c, double d)
        {
            float[] ret = new float[3];


            ret[0] = (float)(Math.Sin(a) * Math.Cos(c + d) + b);
            ret[1] = (float)(Math.Cos(b) *c);
            ret[2] = (float)(Math.Sin(b + d)); 

            return ret;
        }
        // return 1 for player won and -1 for dealer one
        static int playHand()
        {
            bool canSplit = false;
            var playersVal = 0;
            var dealerVal = 0;

            //get initial players and dealers card
            var card = deck.getCard();
            //player first card
            if(card == 1)
            {
                playersVal = 11;
            }
            else
            {
                playersVal = card;
            }
            //get second card
            card = deck.getCard();
            if (card == playersVal)
                canSplit = true;

            if(card == 1)
            {
                if (playersVal >= 11)
                    playersVal += 1;
                else
                    playersVal += 11;
            }
            else
            {
                playersVal += card;
            }

            //get dealer card
            dealerVal = deck.getCard();
            
            //Now make the decisions

            return 1;
        }
    }
}
