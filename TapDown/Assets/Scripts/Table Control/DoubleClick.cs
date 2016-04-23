using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DoubleClick : MonoBehaviour, IPointerDownHandler
{
    float doubleClickStart = 0;
    
    GameObject panel;
    LayoutElement layout;
    public bool movable = true;



    public void OnPointerDown(PointerEventData eventData)
    {
        panel = GameObject.Find("tableArea");
        if (movable)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if ((Time.time - doubleClickStart) < 1f)
                {
                    this.OnDoubleClick();
                    doubleClickStart = -1;
                }
                else
                {
                    doubleClickStart = Time.time;
                }
            }
        }
        
        
    }

   
    void OnDoubleClick()
    {
        layout = gameObject.GetComponent<LayoutElement>();
        string parseName = name;
        string[] card = parseName.Split('_');
        Card temp = Overlord.localPlayer.wrestler.cards[int.Parse(card[1])];
        

        if (Overlord.localPlayer.turn && !Overlord.localPlayer.countering)
        {
           
            
            if (temp.atk)
            {
                if(!temp.toDown)
                {
                    placeCard(temp);
                }
                else if(temp.toDown && Overlord.opponent.stance > 0)
                {
                    placeCard(temp);
                }
                
            }
            
        }
        else if (Overlord.localPlayer.countering)
        {

            Debug.Log(temp.counter);
            if(temp.counter)
            {
                if (!temp.toDown)
                {
                    placeCard(temp);
                    
                }
                else if (temp.toDown && Overlord.opponent.stance > 0)
                {
                    placeCard(temp);
                    
                }
                
                
            }
            
        }
        
        

    }


    void placeCard(Card temp)
    {
        layout.preferredWidth = 200;
        layout.minWidth = 200;
        transform.SetParent(panel.transform);
        StartGame.stack[StartGame.stackCount++] = gameObject;
        Overlord.localPlayer.handCounter--;
        Overlord.addToStack(temp);
        movable = false;
        
    }
}