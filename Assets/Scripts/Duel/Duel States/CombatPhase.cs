using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CombatPhase : NetworkBehaviour, DuelState {
    public event EventHandler<PlayerEventArgs> OnCombatPhase;
    public event EventHandler<PlayerEventArgs> OnStartDeclareAttackers;
    public event EventHandler<PlayerEventArgs> OnStartDeclareDefenders;
    public event EventHandler<PlayerEventArgs> OnSetDeclareDefeners;
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

    public void UpdateState() { }

    private void StartDefenderDeclaration(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= StartDefenderDeclaration;
        StartDefenderDeclarationServerRpc();
        if (combatManager.GetTargets().Count == 0)
            UpdatePlayerReadyStateServerRpc();

    }

    [Rpc(SendTo.Server)]
    private void StartDefenderDeclarationServerRpc() {
        StartDeclareDefenderCombatStateClientRpc();
        List<MatchPlayer> targets = combatManager.GetTargets();
        List<ulong> targetIds = new List<ulong>();
        foreach (MatchPlayer target in targets)
            targetIds.Add(target.PlayerId);
        BaseRpcTarget rpcTarget = RpcTarget.Group(targetIds, RpcTargetUse.Temp);
        SetDeclareDefenderActionClientRpc(rpcTarget);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartDeclareDefenderCombatStateClientRpc() {
        combatState = CombatState.DeclareDefenders;
        OnStartDeclareDefenders?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SetDeclareDefenderActionClientRpc(RpcParams rpcParams) {
        TcgLogger.Log("You have been target for an attack. Pick a defender");
        EventBus.OnActionButtonPressed += PlayerReadyUp;
        OnSetDeclareDefeners?.Invoke(this, new PlayerEventArgs(stateManager.DuelManager.GetCurrentPlayerTurn()));
    }

    private void PlayerReadyUp(object sender, EventArgs args) {
        TcgLogger.Log("You have readied up");
        EventBus.OnActionButtonPressed -= PlayerReadyUp;
        EventBus.InvokeOnLocalClientPlayerReadyUp(this, EventArgs.Empty);
        PlayerReadyUpServerRpc(stateManager.DuelManager.LocalClientPlayer.PlayerId);
    }

    [Rpc(SendTo.Server)]
    private void PlayerReadyUpServerRpc(ulong playerId) {
        PlayerReadyUpClientRpc(playerId);
        UpdatePlayerReadyStateServerRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayerReadyUpClientRpc(ulong playerId) {
        TcgLogger.Log("Player " + playerId + " has readied up");
        readyPlayers.Add(playerId);
    }

    [Rpc(SendTo.Server)]
    private void UpdatePlayerReadyStateServerRpc() {
        if (combatState != CombatState.DeclareDefenders)
            return;

        if (readyPlayers.Count >= combatManager.DuelistCombats.Count) {
            readyPlayers.Clear();
            combatState = CombatState.ProcessCombat;
            ProcessCombatClientRpc();
            combatState = CombatState.None;
            SwitchToSecondMainPhaseClientRpc();
        }
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

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlayCombatCards() {
        return true;
    }

    public CombatState CombateState { get { return combatState; } }
}
