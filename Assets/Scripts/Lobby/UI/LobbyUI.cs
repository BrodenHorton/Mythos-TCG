using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
    [SerializeField] private Transform lobbyPlayersContainer;
    [SerializeField] private List<LobbyPlayerUI> lobbyPlayers;
    [Header("Prefab")]
    [SerializeField] private LobbyPlayerUI lobbyPlayerUIPrefab;
    
    private void Start() {
        gameObject.SetActive(false);
    }

    public void AddLobbyPlayer(string playerId, string username, bool isReady) {
        LobbyPlayerUI lobbyPlayerUI = Instantiate(lobbyPlayerUIPrefab);
        lobbyPlayerUI.transform.parent = lobbyPlayersContainer;
        lobbyPlayerUI.PlayerId = playerId;
        lobbyPlayerUI.SetUsername(username);
        lobbyPlayerUI.SetPlayerReady(isReady);
        lobbyPlayers.Add(lobbyPlayerUI);
    }

    public void RemoveLobbyPlayer(string playerId) {
        foreach(LobbyPlayerUI lobbyPlayerUI in lobbyPlayers) {
            if(lobbyPlayerUI.PlayerId == playerId) {
                lobbyPlayers.Remove(lobbyPlayerUI);
                break;
            }
        }
    }

    public void UpdatePlayersReadyStatus() {

    }

}

public class LobbyPlayerUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI readyText;
    [SerializeField] private Button readyBtn;

    private string playerId;

    public void SetUsername(string username) {
        usernameText.text = username;
    }

    public void SetPlayerReady(bool isReady) {
        if(readyBtn.gameObject.activeSelf)
            readyBtn.gameObject.SetActive(false);
        if(!readyText.gameObject.activeSelf)
            readyText.gameObject.SetActive(true);
    }

    public string PlayerId { get { return playerId; } set { playerId = value; } }
}

public class LobbyPlayerUIController : MonoBehaviour {
    [SerializeField] TcgLobby tcgLobby;
    [SerializeField] private LobbyUI lobbyUI;

    private void Start() {
        // TODO: Add event listeners for tcgLobby events
    }

    public void AddLobbyPlayer(string playerId, string username, bool isReady) {
        lobbyUI.AddLobbyPlayer(playerId, username, isReady);
    }

    public void RemoveLobbyPlayer(string playerId) {
        lobbyUI.RemoveLobbyPlayer(playerId);
    }

    public void UpdatePlayersReadyStatus() {

    }
}