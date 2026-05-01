using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeclareSpellsState : NetworkBehaviour, CombatState {
    public event EventHandler<PlayerEventArgs> OnStartDeclareSpells;

    private CombatStateManager combatStateManager;
    private DuelManager duelManager;
    private CombatManager combatManager;
    private ActionManager actionManager;
    private SpellChainManager spellChainManager;

    private DuelistCombat duelistCombat;
    private List<int> declareSpellsPlayerIndices;

    private void Awake() {
        duelistCombat = null;
        declareSpellsPlayerIndices = new List<int>();
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
        spellChainManager = FindFirstObjectByType<SpellChainManager>();
        if (spellChainManager == null)
            throw new Exception("Could not find SpellChainManager object");

        spellChainManager.OnSpellChainFinished += SkipActionOnSpellChainFinished;
    }

    public void EnterState() {
        if(IsServer) {
            if (combatManager.DuelistCombats.Count == 0)
                throw new Exception("DeclareSpellsState entered when there are no duelist combats to process");

            InitializeStateClientRpc();
            AddNextSpellActionClientRpc();
        }
    }

    public void UpdateState() { }

    [Rpc(SendTo.ClientsAndHost)]
    private void InitializeStateClientRpc() {
        duelistCombat = combatManager.DuelistCombats[0];
        declareSpellsPlayerIndices.Add(duelManager.GetPlayerIndex(duelistCombat.Initiator));
        declareSpellsPlayerIndices.Add(duelManager.GetPlayerIndex(duelistCombat.Target));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void AddNextSpellActionClientRpc() {
        if (declareSpellsPlayerIndices.Count == 0)
            throw new Exception("Attempting to add next spell action when declareSpellsPlayerIndices is empty");

        int playerIndex = declareSpellsPlayerIndices[0];
        if (playerIndex == duelManager.GetLocalClientPlayerIndex()) {
            actionManager.AddAction(new SkipDeclareSpellDuelistAction(duelManager, spellChainManager, this));
            actionManager.SetActionFocusPlayerIndicesServerRpc(playerIndex);
        }
    }

    private void SkipActionOnSpellChainFinished(object sender, EventArgs args) {
        if (!IsServer)
            return;
        if (duelistCombat == null)
            return;

        SkipActionServerRpc();
    }

    [Rpc(SendTo.Server)]
    public void SkipActionServerRpc() {
        TcgLogger.Log("Entered SkipActinServerRpc");
        RemoveTopPlayerIndexClientRpc();
        if (declareSpellsPlayerIndices.Count > 0)
            AddNextSpellActionClientRpc();
        else {
            ResetStateClientRpc();
            actionManager.SetActionFocusPlayerIndicesServerRpc(duelManager.CurrentPlayerTurnIndex);
            SwitchToProcessCombatClientRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RemoveTopPlayerIndexClientRpc() {
        if (declareSpellsPlayerIndices.Count == 0)
            throw new Exception("Attempting to remove element when declareSpellsPlayerIndices is empty");

        declareSpellsPlayerIndices.RemoveAt(0);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ResetStateClientRpc() {
        duelistCombat = null;
        declareSpellsPlayerIndices.Clear();
    }


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
