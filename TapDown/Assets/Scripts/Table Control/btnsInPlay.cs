using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class btnsInPlay : MonoBehaviour {

    GameObject stand;
    GameObject rest;
    // Use this for initialization
    void Start () {
        stand = GameObject.Find("standBtn");
        rest = GameObject.Find("skipHealBtn");
        buttonsOn();
    }
    
    void buttonsOn()
    {
       
        if (!Overlord.localPlayer.turn)
        {
            stand.SetActive(false);
            rest.SetActive(false);
        }
        else if (Overlord.localPlayer.stance == 0)
        {
            stand.SetActive(false);
        }
        else if(Overlord.localPlayer.turn)
        {
            rest.SetActive(true);
            if(Overlord.localPlayer.stance > 0) stand.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update () {
        buttonsOn();
    }

    public void sendStand()
    {
        Overlord.addToStack("stand");
    }

    public void sendRest()
    {
        Overlord.addToStack("rest");
    }
}
