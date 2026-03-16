using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public event EventHandler OnGameStart;

    public static GameManager Instance { get; private set; }
    
    private GameState gameState;
    private int playerCount = 2;
    private bool isFirstUpdate = true;

    private void Awake() {
        if(Instance != null) {
            Debug.Log("Instance of GameManager already exisits in scene. Destroying redundant object");
            Destroy(Instance.gameObject);
            return;
        }

        Instance = this;
        gameState = GameState.WaitingForPlayers;
    }

    private void Start() {
        NetworkManager.Singleton.OnClientConnectedCallback += UpdateGameStateOnClientConnected;
    }

    private void Update() {
        if (!isFirstUpdate)
            return;

        if (NetworkManager.Singleton.ConnectedClients.Count >= playerCount)
            StartGame();
        isFirstUpdate = false;
    }

    private void UpdateGameStateOnClientConnected(ulong clientId) {
        if(gameState == GameState.WaitingForPlayers && NetworkManager.Singleton.ConnectedClients.Count >= 2) {
            StartGame();
        }
    }

    private void StartGame() {
        gameState = GameState.Duel;
        OnGameStart?.Invoke(this, EventArgs.Empty);
    }

    public GameState GameState { get { return gameState; } }
}
