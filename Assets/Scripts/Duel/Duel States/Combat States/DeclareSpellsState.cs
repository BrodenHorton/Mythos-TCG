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

        combatStateManager = ServiceLocator.Get<CombatStateManager>();
        duelManager = ServiceLocator.Get<DuelManager>();
        combatManager = ServiceLocator.Get<CombatManager>();
        actionManager = ServiceLocator.Get<ActionManager>();
        spellChainManager = ServiceLocator.Get<SpellChainManager>();

        spellChainManager.OnSpellChainFinished += SkipActionOnSpellChainFinished;
    }

    public void EnterState() {
        if (!IsServer)
            return;
        if (combatManager.DuelistCombats.Count == 0)
            throw new Exception("DeclareSpellsState entered when there are no duelist combats to process");

        duelistCombat = combatManager.DuelistCombats[0];
        declareSpellsPlayerIndices.Add(duelManager.GetPlayerIndex(duelistCombat.InitiatorId));
        declareSpellsPlayerIndices.Add(duelManager.GetPlayerIndex(duelistCombat.TargetId));
        AddNextSpellAction();
    }

    public void UpdateState() { }

    private void AddNextSpellAction() {
        if (declareSpellsPlayerIndices.Count == 0)
            throw new Exception("Attempting to add next spell action when declareSpellsPlayerIndices is empty");

        int playerIndex = declareSpellsPlayerIndices[0];
        ulong playerId = duelManager.Players[playerIndex].PlayerId;
        actionManager.AddAction(playerId, new SkipDeclareSpellDuelistAction(playerId, spellChainManager, this));
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
