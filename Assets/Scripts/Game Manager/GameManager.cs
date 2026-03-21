using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour {
    public event EventHandler<StartGameEventArgs> OnGameStart;

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
            NetworkManager.Singleton.OnClientConnectedCallback += UpdateGameStateOnClientConnectedServerRpc;
    }

    private void Update() {
        if(!IsServer || !isFirstUpdate) {
            return;
        }

        TcgLogger.Log("Entered Update for GameManager");
        if (NetworkManager.Singleton.ConnectedClients.Count >= playerCount)
            StartGameRpc();
        isFirstUpdate = false;
    }

    [Rpc(SendTo.Server)]
    private void UpdateGameStateOnClientConnectedServerRpc(ulong clientId) {
        TcgLogger.Log("GameManager: Client Connected");
        if (gameState == GameState.WaitingForPlayers && NetworkManager.Singleton.ConnectedClients.Count >= 2) {
            StartGameRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartGameRpc() {
        TcgLogger.Log("StarGameClientRpc Entered");
        gameState = GameState.Duel;
        List<ulong>  playerOrder = new List<ulong>();
        foreach(ulong playerId in NetworkManager.Singleton.ConnectedClients.Keys)
            playerOrder.Add(playerId);
        OnGameStart?.Invoke(this, new StartGameEventArgs(playerOrder));
    }

    public GameState GameState { get { return gameState; } }
}
