using UnityEngine;

public class tapDownEvent : MonoBehaviour
{
    public static float time;
    public static float percentClicked = 0;

    public static void sweepHand(Player player)
    {
        time = 0f;
        int[] hand = player.hand;
        for (int i = 0; i < hand.Length; i++)
        {
            if (hand[i] != -1)
            {
                if (player.wrestler.cards[hand[i]].escape)
                {
                    Overlord.escapedTap(i);
                    break;
                }
            }

        }
    }

    public static bool sweepAI()
    {

        percentClicked = 15*(Overlord.opponent.currHealth / Overlord.opponent.wrestler.health);
        for (int i = 0; i < percentClicked; i++)
        {

            if (Overlord.opponent.currHealth > 0)
            {
                Card card = Overlord.opponent.wrestler.cards[Overlord.opponent.getNextCard()];
                Debug.Log(card.name + " || " + card.escape.ToString());
                if (card.escape)
                {
                    Overlord.escapedTap();
                    return true;
                }
                
            }
        }

        return false;

    }


    public void tapDownSearch()
    {
        if (Overlord.localPlayer.currHealth > 0)
        {
            percentClicked += Overlord.localPlayer.currHealth / Overlord.localPlayer.wrestler.health;
            if(percentClicked >= 1)
            {
                percentClicked--;
                Card card = Overlord.localPlayer.wrestler.cards[Overlord.localPlayer.getNextCard()];
                if (card.escape)
                {
                    Overlord.escapedTap();
                }
            }
            

        }

    }
}
