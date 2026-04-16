using System;
using Unity.Netcode;
using UnityEngine;

public class DeclareAttackersState : MonoBehaviour, CombatState {
    public event EventHandler<PlayerEventArgs> OnStartDeclareAttackers;

    private CombatStateManager combatStateManager;

    private void Start() {
        combatStateManager = FindFirstObjectByType<CombatStateManager>();
        if (combatStateManager == null)
            throw new Exception("Could not find CombatStateManager object");
    }

    public void EnterState() {
        if (combatStateManager.DuelManager.IsLocalClientPlayerTurn())
            EventBus.OnActionButtonPressed += SwitchToDeclareDefenders;
        OnStartDeclareAttackers?.Invoke(this, new PlayerEventArgs(combatStateManager.DuelManager.GetCurrentPlayerTurn()));
    }

    public void UpdateState() { }

    private void SwitchToDeclareDefenders(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= SwitchToDeclareDefenders;
        SwitchToDeclareDefendersServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void SwitchToDeclareDefendersServerRpc() {
        SwitchToDeclareDefendersClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToDeclareDefendersClientRpc() {
        combatStateManager.SwitchState(combatStateManager.DeclareDefendersState);
    }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return false;
    }

    public bool CanDeclareAttackers() {
        return true;
    }

    public bool CanDeclareDefenders() {
        return false;
    }
}
