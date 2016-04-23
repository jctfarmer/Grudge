using UnityEngine;
using UnityEngine.UI;

public class Mute : MonoBehaviour {
    public Sprite muted;
    public Sprite unmuted;
    bool isMute;

    public void doMute()
    {
        isMute = !isMute;
        AudioListener.volume = isMute ? 0 : 1;
    }

    public void muteSwap()
    {
        Image buttonImage = gameObject.GetComponent<Image>();
        if (isMute)
        {
            buttonImage.sprite = muted;
        }
        else
        {
            buttonImage.sprite = unmuted;
        }
    }
}
