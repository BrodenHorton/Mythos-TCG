using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeclareSpellsState : NetworkBehaviour, CombatState {
    public event EventHandler<PlayerEventArgs> OnStartDeclareSpells;

    private CombatStateManager combatStateManager;
    private DuelManager duelManager;
    private CombatManager combatManager;

    private List<ulong> readyPlayers;

    private void Awake() {
        readyPlayers = new List<ulong>();
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
    }

    public void EnterState() {
        if (IsServer)
            SwitchToProcessCombatClientRpc();
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToProcessCombatClientRpc() {
        combatStateManager.SwitchState(combatStateManager.ProcessCombatState);
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
