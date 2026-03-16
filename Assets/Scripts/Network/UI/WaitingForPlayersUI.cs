using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class WaitingForPlayersUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI connectedPlayers;
    [SerializeField] private TextMeshProUGUI maxPlayers;

    public void UpdateUI(int connectedPlayersCount, int maxPlayersCount) {
        connectedPlayers.text = connectedPlayersCount.ToString();
        maxPlayers.text = maxPlayersCount.ToString();
    }
}

public class WaitingForPlayersUIController : MonoBehaviour {
    [SerializeField] private WaitingForPlayersUI waitingForPlayersUI;

    private DuelManager duelManager;

    public void Start() {
        duelManager = FindFirstObjectByType<DuelManager>(); 
        if(duelManager == null)
            throw new Exception("Could not find object of type DuelManager in Scene");

        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedUpdate;
    }

    private void ClientConnectedUpdate(ulong clientId) {
        waitingForPlayersUI.UpdateUI(NetworkManager.Singleton.ConnectedClients.Count, 2);
        if(NetworkManager.Singleton.ConnectedClients.Count > 2) {
            //duelManager.StartGame();
        }
    }
}