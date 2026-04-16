using System;
using System.Collections.Generic;
using Unity.Netcode;

public class ActionChainManager : NetworkBehaviour {
    public event EventHandler<ulong> OnActionChainStart;
    public event EventHandler<ulong> OnActionChainUpdate;
    public event EventHandler<ulong> OnActionChainFinished;

    private DuelManager duelManager;
    private int startingIndex;
    private int currentIndex;
    private Stack<SpellCard> actionChain; // Create a class that holds a spell the initiator and its target

    private void Awake() {
        actionChain = new Stack<SpellCard>();
    }

    private void Start() {
        duelManager = GetComponent<DuelManager>();
        if (duelManager == null)
            throw new Exception("DuelManager not found on GameObject");

        // TODO: Listen for a PlayCard event then start an action chain if the card played isn't an Instant
        // TODO: Listen for a AddActionToActionChain event
    }

    [Rpc(SendTo.Server)]
    private void StartActionChainServerRpc(int playerIndex) {
        StartActionChainClientRpc(playerIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartActionChainClientRpc(int playerIndex) {
        startingIndex = playerIndex;
        currentIndex = playerIndex;
        OnActionChainStart?.Invoke(this, duelManager.Players[currentIndex].PlayerId);
    }

    [Rpc(SendTo.Server)]
    private void AddActionServerRpc() {

    }
}