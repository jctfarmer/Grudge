
using System.Collections;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using UnityEngine;

public class Login : MonoBehaviour {

    // Use this for initialization
    void Start ()
    {
       
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }
    



    public void logIn()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                Overlord.loggedIn = true;
            }
            else
            {
                Debug.Log("Login failed");

            }

        });
    }

    public void achievements()
    {
        Social.ShowAchievementsUI();
    }

    public void leaderboard()
    {
        Social.ShowLeaderboardUI();
    }

    // Update is called once per frame
    void Update () {
    
    }
}
