using System;
using Unity.Netcode;

public class DeclareAttackersState : NetworkBehaviour, CombatState {
    public event EventHandler<PlayerEventArgs> OnStartDeclareAttackers;

    private CombatStateManager combatStateManager;
    private ActionManager actionManager;

    private void Start() {
        combatStateManager = FindFirstObjectByType<CombatStateManager>();
        if (combatStateManager == null)
            throw new Exception("Could not find CombatStateManager object");
        actionManager = FindFirstObjectByType<ActionManager>();
        if (actionManager == null)
            throw new Exception("Could not find ActionManager object");
    }

    public void EnterState() {
        TcgLogger.Log("DeclareAttackersState Entered");
        if (combatStateManager.DuelManager.IsLocalClientPlayerTurn()) {
            actionManager.AddAction(SwitchToDeclareDefendersServerRpc, "Commit", "Waiting for Opponent");
            actionManager.SetCanPerformAction(true);
        }
        OnStartDeclareAttackers?.Invoke(this, new PlayerEventArgs(combatStateManager.DuelManager.GetCurrentPlayerTurn()));
    }

    public void UpdateState() { }

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
