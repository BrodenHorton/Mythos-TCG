using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyListUIController : MonoBehaviour {
    [SerializeField] private LobbyListUI lobbyListUI;
    [SerializeField] private TcgLobby tcgLobby;
    [SerializeField] private float maxPollLobbiesTimerDuration;
    [SerializeField] private bool isManualRefresh;

    private float pollLobbiesTimer;

    private void Awake() {
        pollLobbiesTimer = maxPollLobbiesTimerDuration;
    }

    private void Update() {
        if (isManualRefresh)
            return;

        PollLobbies();
    }

    public async void UpdateLobbyList() {
        List<Lobby> lobbies = await tcgLobby.GetLobbyList();
        lobbyListUI.UpdateLobbyList(lobbies, AddLobbyBannerCallback);
    }

    public void PollLobbies() {
        pollLobbiesTimer -= Time.deltaTime;
        if (pollLobbiesTimer <= 0f) {
            Debug.Log("Polling lobbies");
            pollLobbiesTimer = maxPollLobbiesTimerDuration;
            UpdateLobbyList();
        }
    }

    private void AddLobbyBannerCallback(object sender, LobbyBannerClickedEventArgs args) {
        tcgLobby.JoinLobbyById(args.LobbyId);
    }
}
