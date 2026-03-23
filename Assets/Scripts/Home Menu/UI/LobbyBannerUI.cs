using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBannerUI : MonoBehaviour {
    public event EventHandler<LobbyBannerClickedEventArgs> OnLobbyBannerClicked;

    [SerializeField] private Button btn;
    [SerializeField] private TextMeshProUGUI hostName;
    [SerializeField] private TextMeshProUGUI lobbyStatus;
    [SerializeField] private TextMeshProUGUI playerCount;
    [SerializeField] private TextMeshProUGUI maxPlayerCount;

    private string lobbyId;

    public void Awake() {
        btn.onClick.AddListener(() => {
            OnLobbyBannerClicked?.Invoke(this, new LobbyBannerClickedEventArgs(lobbyId));
        });
    }

    public void Init(Lobby lobby) {
        lobbyId = lobby.Id;
        UpdateLobbyBannerUI(lobby);
    }

    public void UpdateLobbyBannerUI(Lobby lobby) {
        if (lobbyId != lobby.Id)
            throw new Exception("Attempting to UpdateLobbyBannerUI for a different lobby");

        for (int i = 0; i < lobby.Players.Count; i++) {
            if (lobby.Players[i].Id == lobby.HostId) {
                hostName.text = lobby.Players[i].Data["playerName"].Value;
                break;
            }
        }
        lobbyStatus.text = lobby.Players.Count < lobby.MaxPlayers ? RichTextUtil.ProcessRichText("&cOpen") : RichTextUtil.ProcessRichText("&aFull");
        playerCount.text = lobby.Players.Count.ToString();
        maxPlayerCount.text = lobby.MaxPlayers.ToString();
    }

    public string LobbyId { get { return lobbyId; } }
}
