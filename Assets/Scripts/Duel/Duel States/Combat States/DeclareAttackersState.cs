using System;
using Unity.Netcode;
using UnityEngine;

public class DeclareAttackersState : MonoBehaviour, CombatState {
    public event EventHandler<PlayerEventArgs> OnStartDeclareAttackers;

    [SerializeField] private CombatStateManager combatStateManager;

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
}
