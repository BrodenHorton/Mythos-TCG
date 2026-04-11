using System;
using System.Collections.Generic;
using Unity.Netcode;

public class CombatManager : NetworkBehaviour {
    public event EventHandler<DuelistCombatEventArgs> OnDuelistCombatFinsihed;

    private DuelManager duelManager;
    private List<DuelistCombat> duelistCombats;

    private void Awake() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        duelistCombats = new List<DuelistCombat>();
    }

    private void Start() {
        EventBus.OnDeclareAttacker += DeclareAttacker;
        EventBus.OnUndeclareAttacker += UndeclareAttacker;
        EventBus.OnDeclareDefender += DeclareDefender;
        EventBus.OnUndeclareDefender += UndeclareDefender;
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
            combat.AddAttacker(card);
            InsertCombat(combat);
        }
    }

    private void DeclareDefender(object sender, DeclareDefenderEventArgs args) {
        AddDefender(args.Initiator, args.Target, args.Attacker, args.Defender);
    }

    private void UndeclareDefender(object sender, UndeclareDefenderEventArgs args) {
        RemoveDefender(args.Initiator, args.Target, args.Defender);
    }

    public void AddDefender(MatchPlayer initiator, MatchPlayer target, CreatureCard attacker, CreatureCard defender) {
        if (!HasExistingDuelistCombat(initiator, target))
            throw new Exception("Attempted to add a defender when there is no existing DuelistCombat between initiator and target");

        GetDuelistCombat(initiator, target).AddDefender(attacker, defender);
    }

    public void RemoveAttacker(MatchPlayer initiator, MatchPlayer target, CreatureCard attacker) {
        if (!HasExistingDuelistCombat(initiator, target))
            throw new Exception("Attempted to remove an attacker when there is not a DuellistCombat between the initiator and target");

        DuelistCombat combat = GetDuelistCombat(initiator, target);
        combat.RemoveAttacker(attacker);
        if(combat.CreatureCombats.Count == 0)
            duelistCombats.Remove(combat);
    }

    public void RemoveDefender(MatchPlayer initiator, MatchPlayer target, CreatureCard defender) {
        if (!HasExistingDuelistCombat(initiator, target))
            throw new Exception("Attempted to remove a defender when there is not a DuellistCombat between the initiator and target");

        GetDuelistCombat(initiator, target).RemoveDefender(defender);
    }

    public void ProcessCombat() {
        Queue<DuelistCombat> combatQueue = new Queue<DuelistCombat>(duelistCombats);
        while(combatQueue.Count > 0) {
            DuelistCombat duelistCombat = combatQueue.Dequeue();
            for(int j = 0; j < duelistCombat.CreatureCombats.Count; j++) {
                CreatureCombat creatureCombat = duelistCombat.CreatureCombats[j];
                if (creatureCombat.Defender == null)
                    duelistCombat.Target.LifePointsDamage(creatureCombat.Attacker.GetAtk());
                else
                    creatureCombat.Defender.InflictDamage(creatureCombat.Attacker.GetAtk());
            }
            OnDuelistCombatFinsihed?.Invoke(this, new DuelistCombatEventArgs(duelistCombat));
            duelistCombats.Remove(duelistCombat);
        }
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

    private void InsertCombat(DuelistCombat combat) {
        int playerIndex = duelManager.GetPlayerIndex(combat.Initiator.PlayerId);
        for(int i = 0; i < duelistCombats.Count; i++) {
            if (playerIndex < duelManager.GetPlayerIndex(duelistCombats[i].Initiator.PlayerId)) {
                duelistCombats[i] = combat;
                return;
            }
        }

        duelistCombats.Add(combat);
    }

    public void ClearCombats() {
        duelistCombats.Clear();
    }

    public List<MatchPlayer> GetTargets() {
        List<MatchPlayer> targets = new List<MatchPlayer>();
        foreach(DuelistCombat combat in duelistCombats) {
            if(!targets.Contains(combat.Target))
                targets.Add(combat.Target);
        }

        return targets;
    }

    public List<DuelistCombat> DuelistCombats { get { return duelistCombats; } }
}
