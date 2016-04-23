using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using GooglePlayGames;

public class DisplayStats : MonoBehaviour {
    Text[] childText;

    void Start()
    {
        GameObject panel = GameObject.Find("StatsPanel");
        childText = panel.GetComponentsInChildren<Text>();
        for(int i = 0; i<childText.Length; i++)
        {
            Debug.Log(childText.ToString());
        }

        if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("endScene")))
        {
            if(Overlord.localPlayer.winner)
            {
                Social.ReportProgress("CgkI0sHb4LocEAIQBQ", 100f, (bool success) =>
                {
                    Debug.Log("won achievement unlocked");
                });
                PlayGamesPlatform.Instance.IncrementAchievement(
                   "Cfjewijawiu_QA", 1, (bool success) =>
                   {
                       // handle success or failure
                   });

                childText[0].text = "Winner!";
                
                Social.ReportScore(1, "CgkI0sHb4LocEAIQBg", (bool success) => {
                        // handle success or failure
                    });
            }
                
            else
            {
                childText[0].text = "Loser!";
                Debug.Log("Failure)");
                Social.ReportScore(1, "CgkI0sHb4LocEAIQCA", (bool success) => {
                    // handle success or failure
                });
            }
        }





        

    }


   
}
