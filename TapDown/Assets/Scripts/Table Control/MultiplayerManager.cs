using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class MultiplayerManager : RealTimeMultiplayerListener {
    public MPUpdateListener updateListener;
    public MPLobbyListener lobbyListener;
    private static MultiplayerManager _instance = null;
    const uint MinOpponents = 1;
    const uint MaxOpponents = 1;
    const uint Variant = 0;
    private byte protocol = 1;
    private int msgLength = 6;
    private List<byte> updateMsg;



    public string GetMyParticipantId()
    {
        return PlayGamesPlatform.Instance.RealTime.GetSelf().ParticipantId;
    }
    public List<Participant> GetAllPlayers()
    {
        return PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
    }

    public void CreateQuickGame()
    {
        PlayGamesPlatform.Instance.RealTime.CreateQuickGame(MinOpponents, MaxOpponents, Variant, this);
    }
    

   

    private MultiplayerManager()
    {
        updateMsg = new List<byte>(msgLength);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    private void ShowGMStatus(string message)
    {
        Debug.Log(message);
        if (lobbyListener != null)
        {
            lobbyListener.SetLobbyStatusMessage(message);
        }
    }

    public void SendStart(char character)
    {
        List<byte> bytes = new List<byte>(3);
        bytes.Add(protocol);
        bytes.Add((byte)'S');
        bytes.Add((byte) character);
        byte[] msgToSend = bytes.ToArray();
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, msgToSend);
    }

    public void SendEnd()
    {
        List<byte> bytes = new List<byte>(2);
        bytes.Add(protocol);
        bytes.Add((byte)'E');
        byte[] msgToSend = bytes.ToArray();
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, msgToSend);
    }

    public void SendMyUpdate(int card, byte turn, int type)
    {
        updateMsg.Clear();
        updateMsg.Add(protocol);
        byte[] msgToSend;

        if (type == 0)
        {
            updateMsg.Add((byte)'A');
            updateMsg.Add(turn);
            updateMsg.AddRange(System.BitConverter.GetBytes(card));
            msgToSend = updateMsg.ToArray();
        }
        else if(type == 1)
        {
            updateMsg.Add((byte)'G');
            updateMsg.Add(turn);
            msgToSend = updateMsg.ToArray();
        }
        else
        {
            updateMsg.Add((byte)'R');
            updateMsg.Add(turn);
            msgToSend = updateMsg.ToArray();
        }
        
        PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, msgToSend);
    }

    public void SignInAndStartMPGame()
    {
        if(!IsAuthenticated())
        {
            PlayGamesPlatform.Instance.localUser.Authenticate((bool success) => {
                if (success)
                {
                    Debug.Log("We're signed in! Welcome " + PlayGamesPlatform.Instance.localUser.userName);
                    CreateQuickGame();
                }
                else {
                    Debug.Log("Oh... we're not signed in.");
                }
            });
        }
        else
        {
            CreateQuickGame();
        }
    }

    public void SignOut()
    {
        PlayGamesPlatform.Instance.SignOut();
    }

    public bool IsAuthenticated()
    {
        return PlayGamesPlatform.Instance.localUser.authenticated;
    }

    public static MultiplayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MultiplayerManager();
            }
            return _instance;
        }
    }

    public void OnRoomSetupProgress(float percent)
    {
        ShowGMStatus("We are " + percent + "% done with setup");
    }

    public void OnRoomConnected(bool success)
    {
        if (success)
        {
            ShowGMStatus("We are connected to the room! I would probably start our game now.");
            
            
            List<Participant> participants = GetAllPlayers();
            int max = Int32.MinValue;
            int hashP;
            foreach (Participant p in participants)
            {
                hashP = Math.Abs(p.ParticipantId.GetHashCode());
                if (hashP > max)
                {
                    max = hashP;
                }

            }

            if(Math.Abs(GetMyParticipantId().GetHashCode()) == max)
            {
                Overlord.localPlayer.firstPlayer = true;
                Overlord.opponent.firstPlayer = false;
                Overlord.localPlayer.turn = true;
            }
            else
            {
                Overlord.localPlayer.firstPlayer = false;
                Overlord.opponent.firstPlayer = true;
                Overlord.localPlayer.turn = false;
            }

            lobbyListener.HideLobby();
            lobbyListener = null;
        }
        else {
            ShowGMStatus("Uh-oh. Encountered some error connecting to the room.");
        }
    }

    public void OnLeftRoom()
    {
        ShowGMStatus("We have left the room. We should probably perform some clean-up tasks.");
    }

    public void OnParticipantLeft(Participant participant)
    {
        ShowGMStatus(participant.ParticipantId + " has left the room");
    }

    public void OnPeersConnected(string[] participantIds)
    {
        foreach (string participantID in participantIds)
        {
            ShowGMStatus("Player " + participantID + " has joined.");
        }
    }

    public void OnPeersDisconnected(string[] participantIds)
    {
        foreach (string participantID in participantIds)
        {
            ShowGMStatus("Player " + participantID + " has left.");
        }
    }

    public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
    {
        byte messageVersion = (byte)data[0];
        char messageType = (char)data[1];
        if (messageType == 'A' && data.Length == msgLength)
        {
            if (data[2] == 1)
            {
                Overlord.localPlayer.turn = true;
            }
            else
            {
                Overlord.localPlayer.turn = false;
            }
            int cardReceived = System.BitConverter.ToInt32(data, 3); 

            Debug.Log(cardReceived.ToString());
            // We'd better tell our GameController about this.
            if(updateListener != null)
            {
                updateListener.UpdateReceived(cardReceived);
            }
        }
        else if (messageType == 'E')
        {
            Overlord.localPlayer.winner = true;
        }
        else if (messageType == 'G' && data.Length == msgLength)
        {
            if (data[2] == 1)
            {
                Overlord.localPlayer.turn = true;
            }
            else
            {
                Overlord.localPlayer.turn = false;
            }
            Overlord.opponent.stance -= 1;
            
        }
        else if (messageType == 'R' && data.Length == msgLength)
        {
            if (data[2] == 1)
            {
                Overlord.localPlayer.turn = true;
            }
            else
            {
                Overlord.localPlayer.turn = false;
            }
            Overlord.opponent.currHealth += Overlord.opponent.wrestler.recoverGain;
        }
        else if(messageType == 'S')
        {
            char type = (char)data[2];

            switch(type)
            {
                case 'h': Overlord.opponent = new Player("hero");
                    break;
                case 'l': Overlord.opponent = new Player("luchador");
                    break;
                case 'g': Overlord.opponent = new Player("giant");
                    break;
                case 'b': Overlord.opponent = new Player("brawler");
                    break;
                case 't': Overlord.opponent = new Player("technician");
                    break;                    
                default: Overlord.opponent = new Player("hero");
                    break;
            }
        }
        
    }
}
