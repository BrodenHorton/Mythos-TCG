using System.Collections.Generic;
using UnityEngine;

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
