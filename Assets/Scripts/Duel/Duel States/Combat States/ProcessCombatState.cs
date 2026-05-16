using System;
using UnityEngine;
using Unity.Netcode;

public class ProcessCombatState : NetworkBehaviour, CombatState {
    private CombatStateManager combatStateManager;
    private CombatManager combatManager;

    private void Start() {
        combatStateManager = ServiceLocator.Get<CombatStateManager>();
        combatManager = ServiceLocator.Get<CombatManager>();
    }

    public void EnterState() {
        if (!IsServer)
            return;

        combatManager.ProcessNextDuelistCombat();
        if (combatManager.DuelistCombats.Count > 0)
            combatStateManager.SwitchState(combatStateManager.DeclareSpellsState);
        else
            combatStateManager.SwitchState(combatStateManager.OutOfCombatState);
    }

    public void UpdateState() { }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return false;
    }

    public bool CanDeclareAttackers() {
        return false;
    }

    public bool CanDeclareDefenders() {
        return false;
    }
}
