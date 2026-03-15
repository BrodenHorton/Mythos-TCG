using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour {
    [SerializeField] private Transform lobbyPlayersContainer;
    [SerializeField] private List<LobbyPlayerUI> lobbyPlayers;
    [SerializeField] private Button readyBtn;
    [SerializeField] private Button startGameBtn;
    [Header("Prefab")]
    [SerializeField] private LobbyPlayerUI lobbyPlayerUIPrefab;
    
    private void Start() {
        startGameBtn.gameObject.SetActive(false);
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

    public void UpdateLobbyPlayerData(string playerId, bool isReady) {
        foreach(LobbyPlayerUI lobbyPlayerUI in lobbyPlayers) {
            if(lobbyPlayerUI.PlayerId == playerId) {
                lobbyPlayerUI.SetPlayerReady(isReady);
                break;
            }
        }
    }

    public void SetStartGameButtonActive(bool isActive) {
        startGameBtn.gameObject.SetActive(isActive);
    }

    public Button ReadyBtn { get { return readyBtn; } }

    public Button StartGameBtn { get { return startGameBtn; } }
}
