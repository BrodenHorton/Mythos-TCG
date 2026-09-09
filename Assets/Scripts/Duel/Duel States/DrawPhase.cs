using System;
using Unity.Netcode;
using UnityEngine;

public class DrawPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnDrawPhaseEntered;
    public event EventHandler<PlayerEventArgs> OnDrawPhaseEnteredFinished;

    private DuelStateManager stateManager;

    private void Start() {
        stateManager = ServiceLocator.Get<DuelStateManager>();
    }

    public void EnterState() {
        if (!IsServer)
            return;

        DuelManager duelManager = stateManager.DuelManager;
        MatchPlayer player = duelManager.GetCurrentPlayerTurn();
        InvokeOnDrawPhaseEnteredClientRpc(player.PlayerId);
        OnDrawPhaseEnteredFinished?.Invoke(this, new PlayerEventArgs(player.PlayerId));
        player.CurrentMana = duelManager.GetStartOfTurnManaCount();
        player.DrawCard();
        stateManager.SwitchState(stateManager.FirstMainPhase);
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDrawPhaseEnteredClientRpc(ulong playerId) {
        Debug.Log("Entered Draw Phase");
        OnDrawPhaseEntered?.Invoke(this, new PlayerEventArgs(playerId));
    }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return false;
    }
}
