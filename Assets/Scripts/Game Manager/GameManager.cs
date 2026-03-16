using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public event EventHandler OnGameStart;

    public static GameManager Instance { get; private set; }

    // Remove this and just have it listen for events
    [SerializeField] private WaitingForPlayersUIController waitingForPlayersUIController;
    
    private GameState gameState;

    public void Awake() {
        if(Instance != null) {
            Debug.Log("Instance of GameManager already exisits in scene. Destroying redundant object");
            Destroy(Instance.gameObject);
            return;
        }

        Instance = this;
        gameState = GameState.WaitingForPlayers;
    }

    private void LateUpdate() {
        if (gameState == GameState.WaitingForPlayers && NetworkManager.Singleton.ConnectedClients.Count < 2) {
            //waitingForPlayersUIController.Update(NetworkManager.Singleton.ConnectedClients.Count, 2);
        }
        else {
            gameState = GameState.Duel;
            waitingForPlayersUIController.gameObject.SetActive(false);
            OnGameStart?.Invoke(this, EventArgs.Empty);
        }
    }

    public GameState GameState { get { return gameState; } }
}
