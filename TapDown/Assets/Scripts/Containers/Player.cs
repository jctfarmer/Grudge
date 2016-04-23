using System;
using UnityEngine;

public class Player  {

    public bool firstPlayer;
    public Character wrestler;
    public int[] currDeck;
    public int currHealth;
    public int[] hand;
    public int handCounter;
    public int deckCounter;
    public bool tapdown;
    public bool winner;
    public int stance;
    public bool turn;
    public bool countering;
    public int turnCounter;
    public bool tapping = false;

    public Player(string type)
    {
        switch (type.ToLower())
        {
            case "hero":
                wrestler = new Hero();
                break;
            case "luchador":
                wrestler = new Luchador();
                break;
            case "technician":
                wrestler = new Technician();
                break;
            case "giant":
                wrestler = new Giant();
                break;
            case "brawler":
                wrestler = new Brawler();
                break;
            default:
                wrestler = new Hero();
                break;
        }
        hand = new int[5];
        currHealth = wrestler.health;
        currDeck = wrestler.getDeck();
        hand = wrestler.hand;
        handCounter = 0;
        tapdown = false;
        stance = 0;
        turn = false;
        turnCounter = wrestler.turnCounter;
        countering = false;

    }

    public void draw()
    {
        if(handCounter < hand.Length)
        {
            hand[handCounter++] = getNextCard();
        }
        else
        {
            discard();
            hand[handCounter++] = getNextCard();
        }
    }

    internal void aiDraw()
    {
        for(int i = 0; i < hand.Length; i++)
        {
            hand[i] = getNextCard();
        }
    }

    public int getNextCard()
    {
        if(deckCounter < 50)
        {
            return currDeck[deckCounter++];
        }
        else
        {
            deckRefresh();
            return currDeck[deckCounter++];
        }

    }

    public void discard()
    {
        if(handCounter >= 2)
        {
            for (int i = 0; i < hand.Length - 1; i++)
            {
                hand[i] = hand[i + 1];

            }
            if(handCounter <=4)
            {
                hand[handCounter] = -1;
            }
            
            handCounter--;
        }
        else if(handCounter == 1)
        {
            hand[0] = -1;
            handCounter--;
        }
    }

    public void discard(int pos)
    {
        if(pos < hand.Length-1)
        {
            hand[pos] = hand[hand.Length - 1];
            if(this.Equals(Overlord.localPlayer))
            {
                Overlord.swap[pos] = 1;
            }
            
            
        }
        hand[hand.Length - 1] = -1;
    }
       
        

    private void deckRefresh()
    {
        currDeck = wrestler.getDeck();
        deckCounter = 0;
        currHealth -= wrestler.refreshDamage;
    }



}
