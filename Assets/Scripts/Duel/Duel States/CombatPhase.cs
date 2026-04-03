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
    private List<ulong> readyPlayers;

    public enum CombatState {
        None,
        DeclareAttackers,
        DeclareDefenders,
        DeclareSpells,
        ProcessCombat
    }

    private void Start() {
        combatState = CombatState.DeclareAttackers;
        readyPlayers = new List<ulong>();
    }

    public void EnterState() {
        Debug.Log("Entered Combat Phase");
        combatState = CombatState.DeclareAttackers;
        OnCombatPhase?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
        if (stateManager.DuelManager.IsLocalClientPlayerTurn())
            EventBus.OnActionButtonPressed += StartDefenderDeclaration;
        OnStartDeclareAttackers?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
    }

    public void UpdateState() {
        if (!IsServer)
            return;
        if (combatState != CombatState.DeclareDefenders)
            return;

        if (readyPlayers.Count == combatManager.DuelistCombats.Count) {
            combatState = CombatState.ProcessCombat;
            ProcessCombatClientRpc();
            combatState = CombatState.None;
            SwitchToSecondMainPhaseClientRpc();
        }
    }

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
        EventBus.OnActionButtonPressed += PlayerReadyUp;
    }

    private void PlayerReadyUp(object sender, EventArgs args) {
        PlayerReadyUpServerRpc(stateManager.DuelManager.LocalClientPlayer.PlayerId);
    }

    [Rpc(SendTo.Server)]
    private void PlayerReadyUpServerRpc(ulong playerId) {
        PlayerReadyUpClientRpc(playerId);
    }

    [Rpc(SendTo.Server)]
    private void PlayerReadyUpClientRpc(ulong playerId) {
        readyPlayers.Add(playerId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ProcessCombatClientRpc() {
        combatManager.ProcessCombat();
        OnCombatPhaseFinished?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToSecondMainPhaseClientRpc() {
        stateManager.SwitchState(stateManager.SecondMainPhase);
    }

    public CombatState CombateState { get { return combatState; } }
}
