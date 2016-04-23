using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetIcons : MonoBehaviour {
    public Sprite heroIcon;
    public Sprite luchaIcon;
    public Sprite techIcon;
    public Sprite brawlIcon;
    public Sprite giantIcon;
    public Sprite comp1;
    public Sprite comp2;
    public Sprite p1Icon;
    public Sprite p2Icon;
    Image[] images;

    // Use this for initialization
    void Start () {
        Player p1;
        Player p2;
        images = gameObject.GetComponentsInChildren<Image>();
        if(Overlord.localPlayer.firstPlayer)
        {
            p1 = Overlord.localPlayer;
            p2 = Overlord.opponent;
        }
        else
        {
            p2 = Overlord.localPlayer;
            p1 = Overlord.opponent;
        }
        
        switch (p1.wrestler.name.ToLower())
        {
            case "hero":
                p1Icon = heroIcon;
                break;
            case "luchador":
                p1Icon = luchaIcon;
                break;
            case "technician":
                p1Icon = techIcon;
                break;
            case "giant":
                p1Icon = giantIcon;
                break;
            case "brawler":
                p1Icon = brawlIcon;
                break;
            default:
                p1Icon = heroIcon;
                break;
        }

        switch (p2.wrestler.name.ToLower())
        {
            case "hero":
                p2Icon = heroIcon;
                break;
            case "luchador":
                p2Icon = luchaIcon;
                break;
            case "technician":
                p2Icon = techIcon;
                break;
            case "giant":
                p2Icon = giantIcon;
                break;
            case "brawler":
                p2Icon = brawlIcon;
                break;
            default:
                p2Icon = heroIcon;
                break;
        }
        foreach (Image image in images)
        {
            if (image.sprite.Equals(comp1))
            {
                image.sprite = p1Icon;
            }
            else if(image.sprite.Equals(comp2))
            {
                image.sprite = p2Icon;
            }
        }
    }
    
    
}
