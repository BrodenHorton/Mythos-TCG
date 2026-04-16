using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeclareSpellsState : NetworkBehaviour, CombatState {
    public event EventHandler<PlayerEventArgs> OnStartDeclareSpells;

    [SerializeField] private CombatStateManager combatStateManager;
    [SerializeField] private DuelManager duelManager;
    [SerializeField] private CombatManager combatManager;

    private List<ulong> readyPlayers;

    private void Awake() {
        readyPlayers = new List<ulong>();
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
