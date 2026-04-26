using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeclareSpellsState : NetworkBehaviour, CombatState {
    public event EventHandler<PlayerEventArgs> OnStartDeclareSpells;

    private CombatStateManager combatStateManager;
    private DuelManager duelManager;
    private CombatManager combatManager;
    private ActionManager actionManager;
    private SpellChainManager spellChainManager;

    private DuelistCombat duelistCombat;

    private void Awake() {
        duelistCombat = null;
    }

    private void Start() {
        combatStateManager = FindFirstObjectByType<CombatStateManager>();
        if (combatStateManager == null)
            throw new Exception("Could not find CombatStateManager object");
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        combatManager = FindFirstObjectByType<CombatManager>();
        if (combatManager == null)
            throw new Exception("Could not find CombatManager object");
        actionManager = FindFirstObjectByType<ActionManager>();
        if (actionManager == null)
            throw new Exception("Could not find ActionManager object");
        spellChainManager = FindFirstObjectByType<SpellChainManager>();
        if (spellChainManager == null)
            throw new Exception("Could not find SpellChainManager object");
    }

    public void EnterState() {
        if(IsServer) {
            if (combatManager.DuelistCombats.Count == 0)
                SwitchToOutOfCombatClientRpc();
            else {
                SetDuelistCombatClientRpc();
                SetInitiatorActionClientRpc();
            }
        }
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetDuelistCombatClientRpc() {
        duelistCombat = combatManager.DuelistCombats[0];
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetInitiatorActionClientRpc() {
        // Add the skip action to the initiator and give the initiator the action focus
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToProcessCombatClientRpc() {
        combatStateManager.SwitchState(combatStateManager.ProcessCombatState);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToOutOfCombatClientRpc() {
        combatStateManager.SwitchState(combatStateManager.OutOfCombatState);
    }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return true;
    }

    public bool CanDeclareAttackers() {
        return false;
    }

    public bool CanDeclareDefenders() {
        return false;
    }
}
