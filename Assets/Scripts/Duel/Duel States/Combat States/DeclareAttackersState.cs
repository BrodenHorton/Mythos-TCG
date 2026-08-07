using System;
using Unity.Netcode;

public class DeclareAttackersState : NetworkBehaviour, CombatState {
    public event EventHandler<ulong> OnDeclareAttackersStateEntered;
    public event EventHandler<ulong> OnDeclareAttackersStateEnteredFinished;

    private CombatStateManager combatStateManager;
    private ActionManager actionManager;

    private void Start() {
        if (!IsServer)
            return;

        combatStateManager = ServiceLocator.Get<CombatStateManager>();
        actionManager = ServiceLocator.Get<ActionManager>();
    }

    public void EnterState() {
        if (!IsServer)
            return;

        ulong currentPlayerTurnId = combatStateManager.DuelManager.GetCurrentPlayerTurn().PlayerId;
        actionManager.AddAction(currentPlayerTurnId, SwitchToDeclareDefendersServerRpc, "Commit", "Waiting for Opponent");
        InvokeOnDeclareAttackersStateEnteredClientRpc(currentPlayerTurnId);
        OnDeclareAttackersStateEnteredFinished?.Invoke(this, currentPlayerTurnId);
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDeclareAttackersStateEnteredClientRpc(ulong playerId) {
        OnDeclareAttackersStateEntered?.Invoke(this, playerId);
    }

    [Rpc(SendTo.Server)]
    private void SwitchToDeclareDefendersServerRpc(ulong _) {
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
