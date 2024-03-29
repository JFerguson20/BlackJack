﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackJack
{
    class BasicStrategy
    {


        public BasicStrategy()
        {
            
        }

        //0 for stand, 1 for hit
        public int choosePlayerAction(Hand playerHand, Hand dealerHand)
        {
            int playerVal = playerHand.getValue();
            int dealerShown = dealerHand.getDealerShowing();
            int action = 1;
            if (playerHand.getAceValue() == 1.0f) //softhand
            {
                if (playerVal <= 17)
                {
                    action = 1;
                }
                else if (playerVal == 18)
                {
                    if(dealerShown <= 8)
                    {
                        action = 0;
                    }
                    else
                    {
                        action = 1;
                    }
                }
                else
                {
                    action = 1;
                }
            }
            else //no soft
            {
                if (playerVal <= 11)
                {
                    action = 1;
                }
                else if (playerVal == 12)
                {
                    if (dealerShown == 4 || dealerShown == 5 || dealerShown == 6)
                    {
                        action = 0; //stand
                    }
                    else
                    {
                        action = 1;
                    }
                }
                else if (playerVal >= 13 && playerVal <= 16)
                {
                    if (dealerShown <= 6)
                    {
                        action = 0;
                    }
                    else
                    {
                        action = 1;
                    }
                }
                else // playerShown > 17
                {
                    action = 0;
                }
            }
            //do the double checks        
            if (playerHand.canDouble())
            {         
                if (playerVal == 11 && dealerShown != 11)
                    action = 2;
                else  if (playerVal == 10 && dealerShown <= 9)
                    action = 2;
                else if (playerVal == 9 && (dealerShown >= 3 && dealerShown <= 6))
                    action = 2;

                //do the soft totals
                if(playerHand.getAceValue() == 1.0f)
                {
                    if (playerVal == 13 && (dealerShown == 5 || dealerShown == 6))
                        action = 2;
                    else if (playerVal == 14 && (dealerShown == 5 || dealerShown == 6))
                        action = 2;
                    else if (playerVal == 15 && (dealerShown == 4 || dealerShown == 5 || dealerShown == 6))
                        action = 2;
                    else if (playerVal == 16 && (dealerShown == 4 || dealerShown == 5 || dealerShown == 6))
                        action = 2;
                    else if (playerVal == 17 && (dealerShown == 3 || dealerShown == 4 || dealerShown == 5 || dealerShown == 6))
                        action = 2;
                    else if (playerVal == 18 && (dealerShown == 3 || dealerShown == 4 || dealerShown == 5 || dealerShown == 6))
                        action = 2;

                }
            }

            //split checks
            if (playerHand.canSplit())
            {
                if (playerVal == 12 && playerHand.getAceValue() == 1.0f) //AA
                    action = 3;
                else if (playerVal == 18 && (dealerShown <= 6 || dealerShown == 8 || dealerShown == 9))
                    action = 3;
                else if (playerVal == 16)
                    action = 3;
                else if (playerVal == 14 && dealerShown <= 7)
                    action = 3;
                else if (playerVal == 12 && (dealerShown == 3 || dealerShown == 4 || dealerShown == 5 || dealerShown == 6))
                    action = 3;
                else if (playerVal == 6 && (dealerShown <= 7))
                    action = 3;
                else if (playerVal == 4 && (dealerShown <= 7))
                    action = 3;
            }
            
            return action;
        } 

        public int chooseDealerAction(Hand dealerHand)
        {
            int action = 0;
            var dealerVal = dealerHand.getValue();

            if(dealerVal <= 16) //hit 16 and less
            {
                action = 1;
            }

            if(dealerVal >= 17) //stand 17 and more
            {
                action = 0;
            }
           
            return action;
        }
    }
}
