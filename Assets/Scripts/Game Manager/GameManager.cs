using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour {
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
        if(IsServer)
            NetworkManager.Singleton.OnClientConnectedCallback += UpdateGameStateOnClientConnected;
    }

    private void Update() {
        if(!IsServer || !isFirstUpdate) {
            return;
        }

        TcgLogger.Log("Entered Update for GameManager");
        if (NetworkManager.Singleton.ConnectedClients.Count >= playerCount)
            StartGameClientRpc();
        isFirstUpdate = false;
    }

    private void UpdateGameStateOnClientConnected(ulong clientId) {
        TcgLogger.Log("GameManager: Client Connected");
        if (gameState == GameState.WaitingForPlayers && NetworkManager.Singleton.ConnectedClients.Count >= 2) {
            StartGameClientRpc();
        }
    }

    [ClientRpc]
    private void StartGameClientRpc() {
        TcgLogger.Log("StarGameClientRpc Entered");
        gameState = GameState.Duel;
        OnGameStart?.Invoke(this, EventArgs.Empty);
    }

    public GameState GameState { get { return gameState; } }
}
