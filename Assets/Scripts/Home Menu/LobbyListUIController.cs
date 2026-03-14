using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyListUIController : MonoBehaviour {
    private static readonly float MAX_POLL_LOBBIES_TIMER_DURATION = 7f;

    [SerializeField] private LobbyListUI lobbyListUI;
    [SerializeField] private TcgLobby tcgLobby;

    private float pollLobbiesTimer;

    private void Awake() {
        pollLobbiesTimer = MAX_POLL_LOBBIES_TIMER_DURATION;
    }

    private void Update() {
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
            pollLobbiesTimer = MAX_POLL_LOBBIES_TIMER_DURATION;
            UpdateLobbyList();
        }
    }

    private void AddLobbyBannerCallback(object sender, LobbyBannerClickedEventArgs args) {
        tcgLobby.JoinLobbyById(args.LobbyId);
    }
}
