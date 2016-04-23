using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class startMulti : MonoBehaviour, MPLobbyListener {
    //GameObject panel;
    

    //private bool _showLobbyDialog;
    private string _lobbyMessage;

    public void HideLobby()
    {
        SceneManager.LoadScene("gameBoard");
    }

    public void SetLobbyStatusMessage(string message)
    {
        _lobbyMessage = message;
    }

   



    
}
