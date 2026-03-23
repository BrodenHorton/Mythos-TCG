using Unity.Netcode;
using UnityEngine;

public class WaitingForPlayersUIController : MonoBehaviour {
    [SerializeField] private WaitingForPlayersUI waitingForPlayersUI;

    private void Start() {
        waitingForPlayersUI.UpdateUI(NetworkManager.Singleton.ConnectedClients.Count, GameManager.Instance.PlayerCount);
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedUpdate;
        GameManager.Instance.OnGameStart += (sender, args) => {
            waitingForPlayersUI.gameObject.SetActive(false);
        };
    }

    private void ClientConnectedUpdate(ulong clientId) {
        if (GameManager.Instance.GameState != GameState.WaitingForPlayers)
            return;

        waitingForPlayersUI.UpdateUI(NetworkManager.Singleton.ConnectedClients.Count, 2);
    }
}