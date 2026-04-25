using System;
using System.Collections.Generic;
using Unity.Netcode;

public class DeclareDefendersState : NetworkBehaviour, CombatState {
    public event EventHandler<PlayerEventArgs> OnStartDeclareDefenders;
    public event EventHandler<PlayerEventArgs> OnSetDeclareDefeners;

    private CombatStateManager combatStateManager;
    private DuelManager duelManager;
    private CombatManager combatManager;
    private ActionManager actionManager;

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
        actionManager = FindFirstObjectByType<ActionManager>();
        if (actionManager == null)
            throw new Exception("Could not find ActionManager object");
    }

    public void EnterState() {
        TcgLogger.Log("Entered DeclareDefendersState");
        if(IsServer) {
            StartDefenderDeclarationServerRpc();
            if (combatManager.GetTargets().Count == 0)
                UpdatePlayerReadyStateServerRpc();
        }
    }

    public void UpdateState() { }

    [Rpc(SendTo.Server)]
    private void StartDefenderDeclarationServerRpc() {
        TcgLogger.Log("StartDefenderDeclarationServerRpc Entered");
        StartDeclareDefenderCombatStateClientRpc();
        List<MatchPlayer> targets = combatManager.GetTargets();
        List<ulong> targetIds = new List<ulong>();
        List<int> targetIndices = new List<int>();
        foreach (MatchPlayer target in targets) {
            targetIds.Add(target.PlayerId);
            targetIndices.Add(duelManager.GetPlayerIndex(target));
        }
        BaseRpcTarget rpcTarget = RpcTarget.Group(targetIds, RpcTargetUse.Temp);
        SetDeclareDefenderActionClientRpc(rpcTarget);
        actionManager.SetActionFocusPlayerIndicesClientRpc(targetIndices.ToArray());
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartDeclareDefenderCombatStateClientRpc() {
        OnStartDeclareDefenders?.Invoke(this, new PlayerEventArgs(duelManager.GetCurrentPlayerTurn()));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SetDeclareDefenderActionClientRpc(RpcParams rpcParams) {
        actionManager.AddAction(PlayerReadyUp, "Commit", "Waiting for Opponents");
        OnSetDeclareDefeners?.Invoke(this, new PlayerEventArgs(duelManager.GetCurrentPlayerTurn()));
    }

    private void PlayerReadyUp() {
        actionManager.RemoveActionFocusIndex(duelManager.GetLocalClientPlayerIndex());
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
        if (readyPlayers.Count < combatManager.DuelistCombats.Count)
            return;

        actionManager.SetActionFocusPlayerIndicesClientRpc(duelManager.CurrentPlayerTurnIndex);
        ClearReadyPlayersClientRpc();
        if (combatManager.DuelistCombats.Count > 0)
            SwitchToDeclareSpellsClientRpc();
        else
            SwitchToOutOfCombatClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ClearReadyPlayersClientRpc() {
        readyPlayers.Clear();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SwitchToDeclareSpellsClientRpc() {
        combatStateManager.SwitchState(combatStateManager.DeclareSpellsState);
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
        return true;
    }
}
