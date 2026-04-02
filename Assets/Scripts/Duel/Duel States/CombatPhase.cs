using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CombatPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnCombatPhase;
    public event EventHandler<PlayerEventArgs> OnStartDeclareAttackers;
    public event EventHandler<PlayerEventArgs> OnStartDeclareDefenders;
    public event EventHandler<PlayerEventArgs> OnCombatPhaseFinished;

    [SerializeField] private DuelStateManager stateManager;
    [SerializeField] private CombatManager combatManager;

    private CombatState combatState;

    public enum CombatState {
        DeclareAttackers,
        DeclareDefenders,
        DeclareSpells,
        ProcessCombat
    }

    private void Start() {
        combatState = CombatState.DeclareAttackers;
    }

    public void EnterState() {
        Debug.Log("Entered Combat Phase");
        combatState = CombatState.DeclareAttackers;
        OnCombatPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if (stateManager.DuelManager.IsLocalClientPlayerTurn())
            EventBus.OnActionButtonPressed += StartDefenderDeclaration;
        OnStartDeclareAttackers?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
    }

    public void UpdateState() { }

    private void StartDefenderDeclaration(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= StartDefenderDeclaration;
        StartDefenderDeclarationServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void StartDefenderDeclarationServerRpc() {
        StartDefenderDeclarationClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartDefenderDeclarationClientRpc() {
        combatState = CombatState.DeclareDefenders;
        OnStartDeclareDefenders?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
    }

    private void ProcessCombat(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= ProcessCombat;
        ProcessCombatRpc();
        SwitchToSecondMainPhaseRpc();
    }

    [Rpc(SendTo.Server)]
    private void ProcessCombatRpc() {
        ProcessCombatClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ProcessCombatClientRpc() {
        combatManager.ProcessCombat();
        OnCombatPhaseFinished?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
    }

    [Rpc(SendTo.Server)]
    private void SwitchToSecondMainPhaseRpc() {
        SwitchToSecondMainPhaseClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToSecondMainPhaseClientRpc() {
        stateManager.SwitchState(stateManager.SecondMainPhase);
    }

    public CombatState CombateState { get { return combatState; } }
}
