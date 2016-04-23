using UnityEngine;
using System.Collections;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class StartGame : MonoBehaviour /*, MPUpdateListener*/
{
    string startGameAchieve = "CgkI0sHb4LocEAIQAg";
    GameObject handPanel;
    GameObject handBlock;
    GameObject tableArea;
    GameObject tapPanel;
    GameObject[] hand;
    public static GameObject[] stack = new GameObject[3];
    public static int stackCount;
    string cardAtlas;
    GameObject p1Health;
    GameObject p2Health;
    GameObject timeBoard;
    Text timer;
    private float time;
    int submitDam;
    int lastSubmitDam;
    int countDown = 0;


    //private Dictionary<string, float> _finishTimes;


    // Use this for initialization
    void Awake()
    {
        stackCount = 0;
        handPanel = GameObject.Find("handPanel");
        handBlock = GameObject.Find("handBlock");
        tableArea = GameObject.Find("tableArea");
        tapPanel = GameObject.Find("tapPanel");
        timeBoard = GameObject.Find("timeBoard");
        timer = timeBoard.GetComponentInChildren<Text>();
        if (Overlord.localPlayer == null)
        {
            Overlord.localPlayer = new Player("hero");
            Overlord.localPlayer.firstPlayer = true;
        }
        if (Overlord.opponent == null)
        {
            Overlord.opponent = new Player("hero");
        }
        p1Health = GameObject.Find("p1Health");
        p2Health = GameObject.Find("p2Health");
        Overlord.single = true;
        if(Overlord.single)
        {
            Overlord.opponent.aiDraw();
        }
    }
    void Start () {

        time = 10f;
        


        Social.ReportProgress(startGameAchieve, 100f, (bool success) =>
        {
            Debug.Log("start achievement unlocked");
        });

        hand = new GameObject[Overlord.localPlayer.hand.Length];
        cardAtlas = Overlord.localPlayer.wrestler.cardAtlas;
        for(int i = 0; i < Overlord.localPlayer.hand.Length-1; i++)
        {
            Overlord.localPlayer.draw();
            hand[i] = (GameObject)Instantiate((GameObject)Resources.Load(cardAtlas +"_"+ Overlord.localPlayer.hand[i].ToString()));
            hand[i].name = cardAtlas + "_" + Overlord.localPlayer.hand[i].ToString();
            hand[i].transform.SetParent(handPanel.transform, false);
        }
        if(Overlord.localPlayer.firstPlayer)
        {
            Overlord.localPlayer.turn = true;
            Overlord.localPlayer.draw();
            
            
            hand[Overlord.localPlayer.hand.Length - 1] = 
                (GameObject)Instantiate((GameObject)Resources.Load(cardAtlas + "_" + Overlord.localPlayer.hand[Overlord.localPlayer.hand.Length - 1].ToString()));
            hand[Overlord.localPlayer.hand.Length - 1].name = cardAtlas + "_" + Overlord.localPlayer.hand[Overlord.localPlayer.hand.Length - 1].ToString();
            hand[Overlord.localPlayer.hand.Length - 1].transform.SetParent(handPanel.transform, false);

            p1Health.GetComponent<Text>().text = Overlord.localPlayer.currHealth.ToString();
            p2Health.GetComponent<Text>().text = Overlord.opponent.currHealth.ToString();
        }
        else
        {
            p2Health.GetComponent<Text>().text = Overlord.localPlayer.currHealth.ToString();
            p1Health.GetComponent<Text>().text = Overlord.opponent.currHealth.ToString();
        }


        

    }
    
    // Update is called once per frame
    void Update () {

       


        if(Overlord.swapTurns)
        {
            time = 10f;
            Overlord.swapTurns = false;
            
            if (Overlord.localPlayer.turn)
            {
                Overlord.localPlayer.draw();
                hand[Overlord.localPlayer.handCounter - 1] = (GameObject)Instantiate((GameObject)Resources.Load(cardAtlas + "_" + Overlord.localPlayer.hand[Overlord.localPlayer.handCounter - 1].ToString()));
                hand[Overlord.localPlayer.handCounter - 1].name = cardAtlas + "_" + Overlord.localPlayer.hand[Overlord.localPlayer.handCounter-1].ToString();
                hand[Overlord.localPlayer.handCounter - 1].transform.SetParent(handPanel.transform, false);
            }

            else Overlord.opponent.draw();

            


        }

        if(Overlord.localPlayer.currHealth <= 0)
        {
            Overlord.opponent.winner = true;
            Overlord.localPlayer.winner = false;
            SceneManager.LoadScene("endScene");

        }
        else if(Overlord.opponent.currHealth <= 0)
        {
            Overlord.localPlayer.winner = true;
            Overlord.opponent.winner = false;
            SceneManager.LoadScene("endScene");
        }


        time -= Time.deltaTime;
        var minutes = time / 60;
        var seconds = time % 60;
        var fraction = (time * 100) % 100;
        timer.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction);


        if(time < 0)
        {
            Card card = new Card("no counter");
            card.num = -2;
            Overlord.addToStack(card);
            countDown++; 
            if(countDown > 3)
            {
                Overlord.localPlayer.currHealth -= countDown;
                Overlord.opponent.currHealth -= countDown;
            }
            time = 10f;
        }







        if (Overlord.addCard)
        {
            GameObject cardToAdd =Instantiate((GameObject)
                Resources.Load(Overlord.opponent.wrestler.cardAtlas + "_" + Overlord.cardToAdd.ToString()));
            LayoutElement layout = cardToAdd.GetComponent<LayoutElement>();
            layout.preferredWidth = 200;
            layout.minWidth = 200;
            Debug.Log(cardToAdd.name);
            StartGame.stack[StartGame.stackCount++] = cardToAdd;
            cardToAdd.transform.SetParent(tableArea.transform, false);
            Overlord.addCard = false;

        }

        if (Overlord.clearStack)
        {
            Debug.Log("Clearing: " + stackCount);
            for (int i = 2; i >= 0; i--)
            {
                Debug.Log(i);
                if (stack[i] != null)
                {
                    stackCount--;
                    Debug.Log(i);
                    Destroy(stack[i]);
                }
            }


            Overlord.clearStack = false;
        }


        for (int i = 0; i < hand.Length; i++)
        {
            
            if (Overlord.swap[i] > 0)
            {
                Destroy(hand[i]);
                
                hand[i] = hand[hand.Length - 1];
                
                
                Overlord.swap[i]--;
            }
        }



        if (Overlord.localPlayer.tapdown && !Overlord.localPlayer.tapping)
        {
            Overlord.localPlayer.tapping = true;
            Debug.Log("tapping");
            tapDownEvent.sweepHand(Overlord.localPlayer);
            tapPanel.SetActive(true);
        }
        else if(Overlord.localPlayer.tapping)
        {
            tapDownEvent.time += Time.deltaTime;
            if(Overlord.submit)
            {
                submitDam = (int)time % 60;
                if (submitDam > lastSubmitDam)
                {
                    Overlord.localPlayer.currHealth -= Overlord.opponent.wrestler.subMod + submitDam;
                    lastSubmitDam = submitDam;
                }
            }
            if(tapDownEvent.time > 3 && !Overlord.submit)
            {
                Overlord.localPlayer.winner = false;
                Overlord.opponent.winner = true;
            }else if(tapDownEvent.time > 5 && Overlord.submit)
            {
                Overlord.localPlayer.winner = false;
                Overlord.opponent.winner = true;
            }
        }
        else
        {
            tapPanel.SetActive(false);
        }



        if(Overlord.opponent.tapdown)
        {
            Overlord.opponent.tapping = true;
            tapDownEvent.sweepHand(Overlord.opponent);
            if (Overlord.opponent.tapping)
            {
                if (!tapDownEvent.sweepAI())
                {
                    Overlord.localPlayer.winner = true;
                    Overlord.opponent.winner = false;

                }
            }
            Overlord.releaseTurn();
            Overlord.opponent.tapping = false;

        }



        if (Overlord.localPlayer.firstPlayer)
        {
            
            p1Health.GetComponent<Text>().text = Overlord.localPlayer.currHealth.ToString();
            p2Health.GetComponent<Text>().text = Overlord.opponent.currHealth.ToString();
        }
        else
        {
            p2Health.GetComponent<Text>().text = Overlord.localPlayer.currHealth.ToString();
            p1Health.GetComponent<Text>().text = Overlord.opponent.currHealth.ToString();
        }



        if(Overlord.localPlayer.turn && !Overlord.opponent.countering)
        {
            handBlock.SetActive(false);
        }
        else if(Overlord.localPlayer.countering)
        {
            handBlock.SetActive(false);
        }
        else
        {
            
            
            Overlord.AI();
            
            handBlock.SetActive(true);
        }

        if(Overlord.localPlayer.winner || Overlord.opponent.winner)
        {
            SceneManager.LoadScene("endScene");
        }


        
    }

}

