using System;
using System.Linq;

public class Character {

    public string name;
    public int health;
    public int damageMod;
    public int refreshDamage;
    public int turnCounter;
    public int standRate;
    public int recoverGain;
    public bool counterCounter;
    public int subMod;
    public int[] hand;
    public int[] deck;
    public string cardAtlas;
    public Card[] cards;

    public Character()
    {
        health = 20;
        damageMod = 0;
        refreshDamage = 1;
        turnCounter = 1;
        standRate = 1;
        recoverGain = 1;
        subMod = 0;
        counterCounter = true;
        cardAtlas = "cardHero";
        hand = new int[5] { -1, -1, -1, -1, -1 };
    }

    public int[] getDeck()
    {
        Random rnd = new Random();
        int[] rndDeck = deck;
        for(int i = rndDeck.Length; i > 0; i--)
        {
            int j = rnd.Next(i);
            int k = rndDeck[j];
            rndDeck[j] = deck[i - 1];
            rndDeck[i - 1] = k;
        }
        return rndDeck;
    }

}
