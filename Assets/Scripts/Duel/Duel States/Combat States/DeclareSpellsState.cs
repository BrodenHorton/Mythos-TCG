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
        spellChainManager = FindFirstObjectByType<SpellChainManager>();
        if (spellChainManager == null)
            throw new Exception("Could not find SpellChainManager object");

        spellChainManager.OnSpellChainFinished += SkipActionOnSpellChainFinished;
    }

    public void EnterState() {
        if (!IsServer)
            return;
        if (combatManager.DuelistCombats.Count == 0)
            throw new Exception("DeclareSpellsState entered when there are no duelist combats to process");

        duelistCombat = combatManager.DuelistCombats[0];
        declareSpellsPlayerIndices.Add(duelManager.GetPlayerIndex(duelistCombat.Initiator));
        declareSpellsPlayerIndices.Add(duelManager.GetPlayerIndex(duelistCombat.Target));
        AddNextSpellAction();
    }

    public void UpdateState() { }

    private void AddNextSpellAction() {
        if (declareSpellsPlayerIndices.Count == 0)
            throw new Exception("Attempting to add next spell action when declareSpellsPlayerIndices is empty");

        int playerIndex = declareSpellsPlayerIndices[0];
        ulong playerId = duelManager.Players[playerIndex].PlayerId;
        actionManager.AddAction(playerId, new SkipDeclareSpellDuelistAction(playerId, duelManager, spellChainManager, this));
        actionManager.SetActionFocusPlayerIndices(playerId);
    }

    private void SkipActionOnSpellChainFinished(object sender, EventArgs args) {
        if (!IsServer)
            return;
        if (duelistCombat == null)
            return;

        SkipAction();
    }

    public void SkipAction() {
        if (!IsServer)
            return;

        TcgLogger.Log("Entered SkipActin");
        RemoveTopPlayerIndex();
        if (declareSpellsPlayerIndices.Count > 0)
            AddNextSpellAction();
        else {
            ResetState();
            actionManager.SetActionFocusPlayerIndices(duelManager.GetCurrentPlayerTurn().PlayerId);
            combatStateManager.SwitchState(combatStateManager.ProcessCombatState);
        }
    }

    private void RemoveTopPlayerIndex() {
        if (declareSpellsPlayerIndices.Count == 0)
            throw new Exception("Attempting to remove element when declareSpellsPlayerIndices is empty");

        declareSpellsPlayerIndices.RemoveAt(0);
    }

    private void ResetState() {
        duelistCombat = null;
        declareSpellsPlayerIndices.Clear();
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
