using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeclareDefendersState : NetworkBehaviour, CombatState {
    public event EventHandler<PlayerEventArgs> OnStartDeclareDefenders;
    public event EventHandler<PlayerEventArgs> OnSetDeclareDefeners;

    [SerializeField] private CombatStateManager combatStateManager;
    [SerializeField] private DuelManager duelManager;
    [SerializeField] private CombatManager combatManager;

    private List<ulong> readyPlayers;

    private void Start() {
        readyPlayers = new List<ulong>();
    }

    public void EnterState() {
        if(IsServer) {
            StartDefenderDeclarationServerRpc();
            if (combatManager.GetTargets().Count == 0)
                UpdatePlayerReadyStateServerRpc();
        }
    }

    public void UpdateState() {

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
        OnStartDeclareDefenders?.Invoke(this, new PlayerEventArgs(duelManager.GetCurrentPlayerTurn()));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SetDeclareDefenderActionClientRpc(RpcParams rpcParams) {
        EventBus.OnActionButtonPressed += PlayerReadyUp;
        OnSetDeclareDefeners?.Invoke(this, new PlayerEventArgs(duelManager.GetCurrentPlayerTurn()));
    }

    private void PlayerReadyUp(object sender, EventArgs args) {
        EventBus.OnActionButtonPressed -= PlayerReadyUp;
        EventBus.InvokeOnLocalClientPlayerReadyUp(this, EventArgs.Empty);
        PlayerReadyUpServerRpc(duelManager.LocalClientPlayer.PlayerId);
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
        if (readyPlayers.Count >= combatManager.DuelistCombats.Count) {
            readyPlayers.Clear();
            SwitchToDeclareSpellsClientRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToDeclareSpellsClientRpc() {
        combatStateManager.SwitchState(combatStateManager.DeclareSpellsState);
    }
}
