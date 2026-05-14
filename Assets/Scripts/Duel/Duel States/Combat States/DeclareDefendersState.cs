using System;
using System.Collections.Generic;
using Unity.Netcode;

public class DeclareDefendersState : NetworkBehaviour, CombatState {
    public event EventHandler<ulong> OnStartDeclareDefenders;
    public event EventHandler OnSetDeclareDefeners;

    private CombatStateManager combatStateManager;
    private DuelManager duelManager;
    private CombatManager combatManager;
    private ActionManager actionManager;

    private List<ulong> readyPlayers;

    private void Awake() {
        readyPlayers = new List<ulong>();
    }

    private void Start() {
        if (!IsServer)
            return;

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
        if (!IsServer)
            return;

        if (combatManager.GetTargets().Count == 0)
            UpdatePlayerReadyStateServerRpc();
        else
            StartDefenderDeclarationServerRpc();
    }

    public void UpdateState() { }

    [Rpc(SendTo.Server)]
    private void StartDefenderDeclarationServerRpc() {
        StartDeclareDefenderCombatStateClientRpc(duelManager.GetCurrentPlayerTurn().PlayerId);
        List<MatchPlayer> targets = combatManager.GetTargets();
        List<ulong> targetIds = new List<ulong>();
        List<int> targetIndices = new List<int>();
        foreach (MatchPlayer target in targets) {
            targetIds.Add(target.PlayerId);
            targetIndices.Add(duelManager.GetPlayerIndex(target));
            actionManager.AddAction(target.PlayerId, PlayerReadyUp, "Commit", "Waiting for Opponents");
        }
        BaseRpcTarget rpcTarget = RpcTarget.Group(targetIds, RpcTargetUse.Temp);
        actionManager.SetActionFocusPlayerIndices(targetIds.ToArray());
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartDeclareDefenderCombatStateClientRpc(ulong playerId) {
        OnStartDeclareDefenders?.Invoke(this, playerId);
    }

    private void PlayerReadyUp() {
        PlayerReadyUpServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void PlayerReadyUpServerRpc(RpcParams rpcParams = default) {
        ulong playerId = rpcParams.Receive.SenderClientId;
        actionManager.RemoveActionFocusId(playerId);
        readyPlayers.Add(playerId);
        TcgLogger.Log("Player " + playerId + " has readied up");
        UpdatePlayerReadyStateServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void UpdatePlayerReadyStateServerRpc() {
        if (readyPlayers.Count < combatManager.DuelistCombats.Count)
            return;

        actionManager.SetActionFocusPlayerIndices(duelManager.GetCurrentPlayerTurn().PlayerId);
        readyPlayers.Clear();
        if (combatManager.DuelistCombats.Count > 0)
            combatStateManager.SwitchState(combatStateManager.DeclareSpellsState);
        else
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
