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
        if (HasAttacker(attacker.Uuid))
            throw new Exception("Attempted to add an attacker that is already in combat");

        creatureCombats.Add(new CreatureCombat(attacker));
    }

    public void AddDefender(Guid attackerUuid, CreatureCard defender) {
        if (!HasAttacker(attackerUuid))
            throw new Exception("Attempted to add a defender to an attacker that is not in combat");
        if (HasDefender(defender.Uuid))
            throw new Exception("Attempted to add a defender that is already in combat");
        CreatureCombat combat = GetCreatureCombatByAttacker(attackerUuid);
        if(combat.Defender != null)
            throw new Exception("Attempted to add a defender to a combat that already has a defender");

        combat.Defender = defender;
    }

    public void RemoveAttacker(Guid attackerUuid) {
        if(!HasAttacker(attackerUuid))
            throw new Exception("No attacker found to remove in CreatureCombat");

        creatureCombats.Remove(GetCreatureCombatByAttacker(attackerUuid));
    }

    public void RemoveDefender(Guid defenderUuid) {
        if(!HasDefender(defenderUuid))
            throw new Exception("No defender found to remove in CreatureCombat");

        GetCreatureCombatByDefender(defenderUuid).Defender = null;
    }

    public CreatureCombat GetCreatureCombatByAttacker(Guid attackerUuid) {
        foreach (CreatureCombat combat in creatureCombats) {
            if (combat.Attacker.Uuid == attackerUuid)
                return combat;
        }

        return null;
    }

    public CreatureCombat GetCreatureCombatByDefender(Guid defenderUuid) {
        foreach (CreatureCombat combat in creatureCombats) {
            if (combat.Defender.Uuid == defenderUuid)
                return combat;
        }

        return null;
    }

    public bool HasAttacker(Guid attackerUuid) {
        foreach(CreatureCombat combat in creatureCombats) {
            if (combat.Attacker.Uuid == attackerUuid)
                return true;
        }

        return false;
    }

    public bool HasDefender(Guid defenderUuid) {
        foreach (CreatureCombat combat in creatureCombats) {
            if (combat.Defender.Uuid == defenderUuid)
                return true;
        }

        return false;
    }

    public List<CreatureCard> GetAllAttackers() {
        List<CreatureCard> result = new List<CreatureCard>();
        for(int i = 0; i < creatureCombats.Count; i++)
            result.Add(creatureCombats[i].Attacker);

        return result;
    }

    public List<CreatureCard> GetAllDefenders() {
        List<CreatureCard> result = new List<CreatureCard>();
        for (int i = 0; i < creatureCombats.Count; i++) {
            if(creatureCombats[i].Defender != null)
                result.Add(creatureCombats[i].Defender);
        }

        return result;
    }

    public MatchPlayer Initiator { get { return initiator; } }

    public MatchPlayer Target { get { return target; } }

    public List<CreatureCombat> CreatureCombats { get { return creatureCombats; } }
}
