using UnityEngine;
using System.Collections;

public class SetChar : MonoBehaviour {

    public void choice(string wrestler)
    {
        Overlord.localPlayer = new Player(wrestler);
        UIControl change = new UIControl();
        change.sceneChange("multiplayerSetup");
    }
}
