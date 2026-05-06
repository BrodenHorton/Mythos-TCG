using System;
using UnityEngine;
using Unity.Netcode;

public class ProcessCombatState : NetworkBehaviour, CombatState {
    private CombatStateManager combatStateManager;
    private CombatManager combatManager;

    private void Start() {
        combatStateManager = FindFirstObjectByType<CombatStateManager>();
        if (combatStateManager == null)
            throw new Exception("Could not find CombatStateManager object");
        combatManager = FindFirstObjectByType<CombatManager>();
        if (combatManager == null)
            throw new Exception("Could not find CombatManager object");
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
