using System;
using Unity.Netcode;
using UnityEngine;

public class UntapPhase : NetworkBehaviour, DuelState {
    public event EventHandler<ulong> OnUntapPhase;

    private DuelStateManager stateManager;

    private void Start() {
        stateManager = ServiceLocator.Get<DuelStateManager>();
    }

    public void EnterState() {
        if (!IsServer)
            return;

        MatchPlayer player = stateManager.DuelManager.GetCurrentPlayerTurn();
        InvokeOnUntapPhaseClientRpc(player.PlayerId);
        for (int i = 0; i < player.Creatures.Count; i++) {
            if (player.Creatures[i].IsTapped)
                player.Creatures[i].Untap();
        }
        stateManager.SwitchState(stateManager.DrawPhase);
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnUntapPhaseClientRpc(ulong playerId) {
        Debug.Log("Entered Untap Phase");
        OnUntapPhase?.Invoke(this, playerId);
    }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return false;
    }
}
