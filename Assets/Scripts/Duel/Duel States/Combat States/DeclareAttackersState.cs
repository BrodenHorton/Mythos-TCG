using System;
using Unity.Netcode;

public class DeclareAttackersState : NetworkBehaviour, CombatState {
    public event EventHandler<PlayerEventArgs> OnDeclareAttackersStateEntered;
    public event EventHandler<PlayerEventArgs> OnDeclareAttackersStateEnteredFinished;
    public event EventHandler<PlayerEventArgs> OnDeclareAttackersStateExited;

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
        actionManager.AddAction(currentPlayerTurnId, EndDeclareAttackersStateServerRpc, "Commit", "Waiting for Opponent");
        InvokeOnDeclareAttackersStateEnteredClientRpc(currentPlayerTurnId);
        OnDeclareAttackersStateEnteredFinished?.Invoke(this, new PlayerEventArgs(currentPlayerTurnId));
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDeclareAttackersStateEnteredClientRpc(ulong playerId) {
        OnDeclareAttackersStateEntered?.Invoke(this, new PlayerEventArgs(playerId));
    }

    [Rpc(SendTo.Server)]
    private void EndDeclareAttackersStateServerRpc(ulong playerId) {
        OnDeclareAttackersStateExited?.Invoke(this, new PlayerEventArgs(combatStateManager.DuelManager.GetCurrentPlayerTurn().PlayerId));
        SwitchToDeclareDefenders();
    }

    private void SwitchToDeclareDefenders() {
        if (!IsServer)
            throw new Exception("Only the server can call the method SwitchToDeclareDefenders");

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
