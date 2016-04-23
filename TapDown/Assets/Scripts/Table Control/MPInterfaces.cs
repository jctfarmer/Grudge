public interface MPLobbyListener {
	void SetLobbyStatusMessage(string message);
	void HideLobby();
}

public interface MPUpdateListener {
	void UpdateReceived(int cardReceived);
	void PlayerFinished(string senderId, float finalTime);
	void LeftRoomConfirmed();
	void PlayerLeftRoom(string participantId);
}

