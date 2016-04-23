

using System;
using GooglePlayGames.BasicApi.Multiplayer;
using UnityEngine;

public class Overlord  {
    public static bool loggedIn = false;
    public static Player localPlayer;
    public static Player opponent;
    public static byte[] matchData;
    public static Card[] stack = new Card[3];
    public static int[] sendStack = new int[10];
    public static int[] swap = new int[6] { 0, 0, 0, 0, 0, 0};
    public static bool noCounter;
    private static int stackCounter = 0;
    public static bool single = false;
    public static bool addCard = false;
    public static int cardToAdd;
    public static bool submit;
    public static bool clearStack;
    public static bool swapTurns;


    internal static void escapedTap()
    {
        if(localPlayer.tapdown)
        {
            localPlayer.tapping = false;
            localPlayer.tapdown = false;
        }
        else
        {
            opponent.tapping = false;
            opponent.tapdown = false;
        }
        
    }

    internal static void escapedTap(int i)
    {
        if(localPlayer.tapdown)
        {
            localPlayer.discard(i);
        }
        else
        {
            opponent.discard(i);
        }
        
        escapedTap();
    }
    

    public static void addToStack(string name)
    {
        if(name.Equals("rest"))
        {
            
            if(localPlayer.currHealth < localPlayer.wrestler.health)
            {
                localPlayer.currHealth += localPlayer.wrestler.recoverGain;
            }
            
        }
        else if(name.Equals("stand"))
        {
            if(localPlayer.stance > 0)
            {
                localPlayer.stance--;
            }

        }

        Card card = new Card("no counter");
        card.num = -2;
        addToStack(card);


    }

    internal static void addToStack(Card temp)
    {
        stack[stackCounter] = temp;
        for(int i = 0; i<stack.Length; i++)
        {
            if(stack[i]!=null) Debug.Log("stack "+i+" :" + stack[i].name);
        }
        
        sendStack[stackCounter] = stack[stackCounter].num;
        stackCounter++;

        if(temp.pin || temp.submit)
        {
            if (opponent.countering)
            {
                runCard(opponent, localPlayer, temp);
                releaseTurn();
            }
            else if (localPlayer.countering)
            {
                runCard(localPlayer, opponent, temp);
                releaseTurn();
            }
            else if (opponent.turn)
            {
                runCard(opponent, localPlayer, temp);
                submit = temp.submit;
                localPlayer.tapdown = true;
                releaseTurn();
            }
            else if (localPlayer.turn)
            {
                runCard(localPlayer, opponent, temp);
                submit = temp.submit;
                opponent.tapdown = true;
                releaseTurn();
            }
        }
        else if(temp.num == -2)
        {
            if (opponent.countering)
            {
                runCard(opponent, localPlayer, stack[0]);
                releaseTurn();
            }
            else if (localPlayer.countering)
            {
                runCard(localPlayer, opponent, stack[0]);
                releaseTurn();
            }
            else if(opponent.turn)
            {
                releaseTurn();
            }
            else if(localPlayer.turn)
            {
                releaseTurn();
            }
        }
        else
            {
            if (opponent.countering)
            {
                runCard(opponent, localPlayer, temp);
                releaseTurn();
            }
            else if (localPlayer.countering)
            {
                runCard(localPlayer, opponent, temp);
                releaseTurn();
            }
            else if (opponent.turn)
            {
                localPlayer.countering = true;
            }
            else if (localPlayer.turn)
            {
                opponent.countering = true;
            }
        }
    }


    public static void releaseTurn()
    {
        if (single)
        {
            swapTurns = true;
            stackCounter = 0;
            localPlayer.turn = !localPlayer.turn;
            opponent.turn = !opponent.turn;
            localPlayer.countering = false;
            opponent.countering = false;
            localPlayer.turnCounter = localPlayer.wrestler.turnCounter;
            opponent.turnCounter = opponent.wrestler.turnCounter;
            clearStack = true;
        }
    }

    //internal static void endTurn(Player player)
    //{
    //    if(player.Equals(Overlord.localPlayer))
    //    {
    //        Debug.Log(stack[stackCounter - 1].pin);
    //        if(stack[stackCounter-1].num == -2)
    //        {
    //            resolveStack();
    //        }
    //        else
    //        {
    //            opponent.countering = true;
    //        }

            
    //    }
    //    else if(player.Equals(Overlord.opponent))
    //    {
    //        if (stack[stackCounter - 1].num == -2)
    //        {
    //            resolveStack();
    //        }
    //        else
    //        {
    //            localPlayer.countering = true;
    //        }
            
    //    }

        
    //}

    //public static void endCounter(Player player)
    //{
    //    if (player.Equals(Overlord.localPlayer))
    //    {
    //        if (stack[stackCounter - 1].num == -2 || stackCounter == stack.Length)
    //        {
    //            if (stack[stackCounter - 1].num == -2) stackCounter--;
    //            resolveStack();
    //        }

    //    }
    //    else if (player.Equals(Overlord.opponent))
    //    {
    //        Debug.Log(stackCounter);
    //        if (stack[stackCounter-1].num == -2 || stackCounter == stack.Length)
    //        {
    //            if (stack[stackCounter-1].num == -2) stackCounter--;
    //            resolveStack();
    //        }
            
    //    }
    //}

    //public static void resolveStack()
    //{
    //    Overlord.clearStack = true;
    //    if (stackCounter > 0)
    //    {
    //        int parity = stackCounter - 1 % 2;
    //        if (localPlayer.turn)
    //        {
    //            if (parity == 0)
    //            {
    //                runStack(localPlayer, opponent);
    //            }
    //            else if (parity == 1)
    //            {
    //                runStack(opponent, localPlayer);
    //            }

    //            localPlayer.turnCounter--;
    //            if (localPlayer.turnCounter < 1)
    //            {
    //                releaseTurn();
    //            }
    //            else
    //            {
    //                localPlayer.countering = false;
    //                opponent.countering = false;
    //            }

    //        }
    //        else if (opponent.turn)
    //        {
    //            if (parity == 1)
    //            {
    //                runStack(localPlayer, opponent);
    //            }
    //            else if (parity == 0)
    //            {
    //                runStack(opponent, localPlayer);
    //            }
    //            opponent.turnCounter--;
    //            if (opponent.turnCounter < 1)
    //            {
    //                releaseTurn();
    //            }
    //            else
    //            {
    //                localPlayer.countering = false;
    //                opponent.countering = false;
    //            }
    //        }

    //    }

    //    if (localPlayer.turn)
    //    {
    //        localPlayer.turnCounter--;
    //        if (localPlayer.turnCounter < 1)
    //        {
    //            releaseTurn();
    //        }
    //        else
    //        {
    //            localPlayer.countering = false;
    //            opponent.countering = false;
    //        }
    //    }
    //    else if (opponent.turn)
    //    {
    //        opponent.turnCounter--;
    //        if (opponent.turnCounter < 1)
    //        {
    //            releaseTurn();
    //        }
    //        else
    //        {
    //            localPlayer.countering = false;
    //            opponent.countering = false;
    //        }
    //    }


        
    //}


  

    private static void runCard(Player winner, Player loser, Card temp)
    {
        int damage = 0;
       
        Debug.Log(temp.name);
        loser.currHealth -= temp.attack+winner.wrestler.damageMod;
        if (temp.draw)
        {
            for (int j = 0; j < temp.numDraw; j++)
            {
                winner.draw();
            }

        }
        if (temp.oppDisc) loser.discard();
        if (temp.oppSkip) winner.turnCounter++;
        if (temp.downOpp && loser.stance < loser.wrestler.standRate) loser.stance=loser.wrestler.standRate;
        
    }

    

    internal static void AI()
    {
        if(!opponent.tapdown)
        {
            if (opponent.countering)
            {
                bool countered = false;
                for (int i = 0; i < 5; i++)
                {
                    Debug.Log(i);
                    Debug.Log(opponent.hand[i]);
                    if(opponent.hand[i] != -1)
                    {
                        if (opponent.wrestler.cards[opponent.hand[i]].counter && !opponent.wrestler.cards[opponent.hand[i]].toDown)
                        {
                            cardToAdd = opponent.hand[i];
                            opponent.hand[i] = -1;
                            addToStack(opponent.wrestler.cards[cardToAdd]);
                            countered = true;
                            break;
                        }
                    }
                    
                }
                if (!countered)
                {
                    Card card = new Card("no counter");
                    card.num = -2;
                    addToStack(card);
                }
                if (countered)
                {
                    addCard = true;
                    Overlord.swapTurns = true;
                    opponent.countering = false;
                    
                }
                
            }
            else if(opponent.turn)
            {
                bool attacked = false;
                for (int i = 0; i < 5; i++)
                {
                    Debug.Log(i);
                    Debug.Log(opponent.hand[i]);
                    if (opponent.hand[i] != -1)
                    {
                        if (opponent.wrestler.cards[opponent.hand[i]].atk && !opponent.wrestler.cards[opponent.hand[i]].toDown)
                        {
                            cardToAdd = opponent.hand[i];
                            opponent.hand[i] = -1;
                            addToStack(opponent.wrestler.cards[cardToAdd]);
                            attacked = true;
                            break;
                        }
                    }

                }
                if (!attacked)
                {
                    Card card = new Card("no counter");
                    card.num = -2;
                    addToStack(card);
                }
                if (attacked)
                {
                    addCard = true;
                    Overlord.swapTurns = true;
                    opponent.countering = false;
                    localPlayer.countering = true;

                }
            }
        }
        
    }
}
