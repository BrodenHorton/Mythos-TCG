using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {
    private List<DuelistCombat> duelistCombats;

    private void Start() {
        EventBus.OnDeclareAttacker += DeclareAttacker;
        EventBus.OnUndeclareAttacker += UndeclareAttacker;
        // TODO: Add defender listeners
    }

    private void DeclareAttacker(object sender, DeclareAttackerEventArgs args) {
        AddAttacker(args.Initiator, args.Target, args.Attacker);
    }

    private void UndeclareAttacker(object sender, UndeclareAttackerEventArgs args) {
        RemoveAttacker(args.Initiator, args.Target, args.Attacker);
    }

    public void AddAttacker(MatchPlayer initiator, MatchPlayer target, CreatureCard card) {
        if(HasExistingDuelistCombat(initiator, target)) {
            DuelistCombat combat = GetDuelistCombat(initiator, target);
            combat.AddAttacker(card);
        }
        else {
            DuelistCombat combat = new DuelistCombat(initiator, target);
            duelistCombats.Add(combat);
            combat.AddAttacker(card);
        }
    }

    public void AddDefender(MatchPlayer initiator, MatchPlayer target, CreatureCard attacker, CreatureCard defender) {
        if (!HasExistingDuelistCombat(initiator, target))
            throw new Exception("Attempted to add a defender when there is no existing DuelistCombat between initiator and target");

        GetDuelistCombat(initiator, target).AddDefender(attacker, defender);
    }

    public void RemoveAttacker(MatchPlayer initiator, MatchPlayer target, CreatureCard attacker) {
        if (HasExistingDuelistCombat(initiator, target))
            throw new Exception("Attempted to remove an attacker when there is not a DuellistCombat between the initiator and target");

        DuelistCombat combat = GetDuelistCombat(initiator, target);
        combat.RemoveAttacker(attacker);
        if(combat.CreatureCombats.Count == 0)
            duelistCombats.Remove(combat);
    }

    public void RemoveDefender(MatchPlayer initiator, MatchPlayer target, CreatureCard defender) {
        if (HasExistingDuelistCombat(initiator, target))
            throw new Exception("Attempted to remove a defender when there is not a DuellistCombat between the initiator and target");

        GetDuelistCombat(initiator, target).RemoveDefender(defender);
    }

    public void ProcessCombat() {
        // TODO: Process all duelist and creature combats for the turn.
    }

    private bool HasExistingDuelistCombat(MatchPlayer initiator, MatchPlayer target) {
        foreach(DuelistCombat combat in duelistCombats) {
            if (initiator == combat.Initiator && target == combat.Target)
                return true;
        }

        return false;
    }

    private DuelistCombat GetDuelistCombat(MatchPlayer initiator, MatchPlayer target) {
        foreach (DuelistCombat combat in duelistCombats) {
            if (initiator == combat.Initiator && target == combat.Target)
                return combat;
        }

        return null;
    }

    public void ClearCombats() {
        duelistCombats.Clear();
    }
}
