using System;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyListUI : MonoBehaviour {
    [SerializeField] private Transform lobbyListContainer;
    [SerializeField] private List<LobbyBannerUI> openLobbies;
    [Header("Prefabs")]
    [SerializeField] private LobbyBannerUI lobbyBannerUIPrefab;

    public void UpdateLobbyList(List<Lobby> lobbies, EventHandler<LobbyBannerClickedEventArgs> callback) {
        List<LobbyBannerUI> openLobbiesCopy = new List<LobbyBannerUI>(openLobbies);
        for(int i = 0; i < lobbies.Count; i++) {
            bool containsLobby = false;
            for(int j = 0; j < openLobbiesCopy.Count; j++) {
                if(openLobbiesCopy[j].LobbyId == lobbies[i].Id) {
                    openLobbies[j].UpdateLobbyBannerUI(lobbies[i]);
                    openLobbiesCopy.RemoveAt(j);
                    containsLobby = true;
                    break;
                }
            }
            if(!containsLobby) {
                LobbyBannerUI lobbyBannerUI = Instantiate(lobbyBannerUIPrefab);
                lobbyBannerUI.transform.SetParent(lobbyListContainer);
                lobbyBannerUI.Init(lobbies[i]);
                lobbyBannerUI.OnLobbyBannerClicked += callback;
                openLobbies.Add(lobbyBannerUI);
            }
        }

        for(int i = openLobbiesCopy.Count - 1; i >= 0; i--) {
            LobbyBannerUI droppedLobby = openLobbiesCopy[i];
            openLobbies.Remove(droppedLobby);
            openLobbiesCopy.RemoveAt(i);
            Destroy(droppedLobby.gameObject);
        }
    }
}
