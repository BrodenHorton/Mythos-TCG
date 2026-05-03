using System;
using Unity.Netcode;
using UnityEngine;

public class DrawPhase : NetworkBehaviour, DuelState {
    public event EventHandler<ulong> OnDrawPhase;

    private DuelStateManager stateManager;

    private void Start() {
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
    }

    public void EnterState() {
        if (!IsServer)
            return;

        DuelManager duelManager = stateManager.DuelManager;
        MatchPlayer player = duelManager.GetCurrentPlayerTurn();
        InvokeOnDrawPhaseClientRpc(player.PlayerId);
        player.CurrentMana = duelManager.GetStartOfTurnManaCount();
        player.DrawCard();
        stateManager.SwitchState(stateManager.FirstMainPhase);
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDrawPhaseClientRpc(ulong playerId) {
        Debug.Log("Entered Draw Phase");
        OnDrawPhase?.Invoke(this, playerId);
    }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return false;
    }
}
