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

    public void EnterState() {

    }

    public void UpdateState() {

    }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return true;
    }

    public bool CanDeclareCombatants() {
        return false;
    }
}
