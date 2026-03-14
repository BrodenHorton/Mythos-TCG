using System;

public class LobbyBannerClickedEventArgs : EventArgs {
    private string lobbyId;

    public LobbyBannerClickedEventArgs(string lobbyId) {
        this.lobbyId = lobbyId;
    }

    public string LobbyId { get { return lobbyId; } }
}
