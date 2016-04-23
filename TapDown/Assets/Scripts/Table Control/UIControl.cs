using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    string startGameAchieve = "CgkI0sHb4LocEAIQAg";
    string tutorialAchieve = "CgkI0sHb4LocEAIQBA";
    string statsAchieve = "CgkI0sHb4LocEAIQAw";

    public void sceneChange(string sceneName)
    {
        if(sceneName == "tutorialScene")
        {
            Social.ReportProgress(tutorialAchieve, 100f, (bool success) => { });
        }else if (sceneName == "statsScene")
        {
            Social.ReportProgress(statsAchieve, 100f, (bool success) => { });
        }
        else if (sceneName == "gameBoard")
        {
            Social.ReportProgress(startGameAchieve, 100f, (bool success) => { });
        }
        SceneManager.LoadScene(sceneName);
    }
   
    public void toMain()
    {
        if(Overlord.loggedIn)
        {
            SceneManager.LoadScene("mainMenu");
        }
    }


}
