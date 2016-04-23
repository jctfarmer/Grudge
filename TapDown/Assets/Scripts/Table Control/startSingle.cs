using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class startSingle : MonoBehaviour {


    public void startBtn()
    {
        Overlord.localPlayer.firstPlayer = true;
        Overlord.opponent = new Player("Hero");
        Overlord.single = true;
        SceneManager.LoadScene("gameBoard");

    }
}
