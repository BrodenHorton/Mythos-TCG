using System;
using UnityEngine;
using Unity.Netcode;

public class ProcessCombatState : NetworkBehaviour, CombatState {
    [SerializeField] private CombatStateManager combatStateManager;
    [SerializeField] private CombatManager combatManager;

    public void EnterState() {
        if (IsServer) {
            ProcessCombatClientRpc();
            SwitchToOutOfCombatClientRpc();
        }
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void ProcessCombatClientRpc() {
        combatManager.ProcessCombat();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToOutOfCombatClientRpc() {
        combatStateManager.SwitchState(combatStateManager.OutOfCombatState);
    }

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
