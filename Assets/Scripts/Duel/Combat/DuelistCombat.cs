using System;
using System.Collections.Generic;

public class DuelistCombat {
    private MatchPlayer initiator;
    private MatchPlayer target;
    private List<CreatureCombat> creatureCombats;

    public DuelistCombat(MatchPlayer initiator, MatchPlayer target) {
        this.initiator = initiator;
        this.target = target;
        creatureCombats = new List<CreatureCombat>();
    }

    public void AddAttacker(CreatureCard attacker) {
        if (HasAttacker(attacker))
            throw new Exception("Attempted to add an attacker that is already in combat");

        creatureCombats.Add(new CreatureCombat(attacker));
    }

    public void AddDefender(CreatureCard attacker, CreatureCard defender) {
        if (HasDefender(defender))
            throw new Exception("Attempted to add a defender that is already in combat");
        CreatureCombat combat = GetCreatureCombatByDefender(attacker);
        if(combat.Defender != null)
            throw new Exception("Attempted to add a defender to a combat that already has a defender");

        combat.Defender = defender;
    }

    public void RemoveAttacker(CreatureCard attacker) {
        if(!HasAttacker(attacker))
            throw new Exception("No attacker found to remove in CreatureCombat");

        creatureCombats.Remove(GetCreatureCombatByDefender(attacker));
    }

    public void RemoveDefender(CreatureCard defender) {
        if(!HasDefender(defender))
            throw new Exception("No defender found to remove in CreatureCombat");

        GetCreatureCombatByDefender(defender).Defender = null;
    }

    public CreatureCombat GetCreatureCombatByAttacker(CreatureCard attacker) {
        foreach (CreatureCombat combat in creatureCombats) {
            if (attacker == combat.Attacker)
                return combat;
        }

        return null;
    }

    public CreatureCombat GetCreatureCombatByDefender(CreatureCard defender) {
        foreach (CreatureCombat combat in creatureCombats) {
            if (defender == combat.Defender)
                return combat;
        }

        return null;
    }

    public bool HasAttacker(CreatureCard attacker) {
        foreach(CreatureCombat combat in creatureCombats) {
            if (attacker == combat.Attacker)
                return true;
        }

        return false;
    }

    public bool HasDefender(CreatureCard defender) {
        foreach (CreatureCombat combat in creatureCombats) {
            if (defender == combat.Defender)
                return true;
        }

        return false;
    }

    public MatchPlayer Initiator { get { return initiator; } }

    public MatchPlayer Target { get { return target; } }

    public List<CreatureCombat> CreatureCombats { get { return creatureCombats; } }
}
