using System;
using System.Collections.Generic;
using Unity.Netcode;

public class CombatManager : NetworkBehaviour {
    public event EventHandler<DuelistCombatEventArgs> OnDuelistCombatFinsihed;
    public event EventHandler OnCombatFinsihed;

    private DuelManager duelManager;
    private List<DuelistCombat> duelistCombats;

    private void Awake() {
        duelistCombats = new List<DuelistCombat>();
    }

    private void Start() {
        if (!IsServer)
            return;

        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        EventBus.Instance.OnDeclareAttacker += DeclareAttacker;
        EventBus.Instance.OnUndeclareAttacker += UndeclareAttacker;
        EventBus.Instance.OnDeclareDefender += DeclareDefender;
        EventBus.Instance.OnUndeclareDefender += UndeclareDefender;
    }

    private void DeclareAttacker(object sender, DeclareAttackerEventArgs args) {
        if (!IsServer)
            return;

        MatchPlayer initiator = duelManager.GetPlayerById(args.InitiatorId);
        MatchPlayer target = duelManager.GetPlayerById(args.TargetId);
        AddAttacker(initiator, target, args.Attacker);
    }

    private void UndeclareAttacker(object sender, UndeclareAttackerEventArgs args) {
        if (!IsServer)
            return;

        MatchPlayer initiator = duelManager.GetPlayerById(args.InitiatorId);
        MatchPlayer target = duelManager.GetPlayerById(args.TargetId);
        RemoveAttacker(initiator, target, args.Attacker.Uuid);
    }

    public void AddAttacker(MatchPlayer initiator, MatchPlayer target, CreatureCard card) {
        if (!IsServer)
            return;

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
        if (!IsServer)
            return;

        MatchPlayer initiator = duelManager.GetPlayerById(args.InitiatorId);
        MatchPlayer target = duelManager.GetPlayerById(args.TargetId);
        AddDefender(initiator, target, args.Attacker, args.Defender);
    }

    private void UndeclareDefender(object sender, UndeclareDefenderEventArgs args) {
        if (!IsServer)
            return;

        MatchPlayer initiator = duelManager.GetPlayerById(args.InitiatorId);
        MatchPlayer target = duelManager.GetPlayerById(args.TargetId);
        RemoveDefender(initiator, target, args.Defender.Uuid);
    }

    public void AddDefender(MatchPlayer initiator, MatchPlayer target, CreatureCard attacker, CreatureCard defender) {
        if (!IsServer)
            return;
        if (!HasExistingDuelistCombat(initiator, target))
            throw new Exception("Attempted to add a defender when there is no existing DuelistCombat between initiator and target");

        GetDuelistCombat(initiator, target).AddDefender(attacker.Uuid, defender);
    }

    public void RemoveAttacker(MatchPlayer initiator, MatchPlayer target, Guid attackerUuid) {
        if (!IsServer)
            return;
        if (!HasExistingDuelistCombat(initiator, target))
            throw new Exception("Attempted to remove an attacker when there is not a DuellistCombat between the initiator and target");

        DuelistCombat combat = GetDuelistCombat(initiator, target);
        combat.RemoveAttacker(attackerUuid);
        if(combat.CreatureCombats.Count == 0)
            duelistCombats.Remove(combat);
    }

    public void RemoveDefender(MatchPlayer initiator, MatchPlayer target, Guid defenderUuid) {
        if (!IsServer)
            return;
        if (!HasExistingDuelistCombat(initiator, target))
            throw new Exception("Attempted to remove a defender when there is not a DuellistCombat between the initiator and target");

        GetDuelistCombat(initiator, target).RemoveDefender(defenderUuid);
    }

    public void ProcessNextDuelistCombat() {
        if (!IsServer)
            return;
        if (duelistCombats.Count == 0)
            throw new Exception("Attempting to process the next DuelistCombat when the duelistCombats list is empty");

        DuelistCombat duelistCombat = duelistCombats[0];
        for (int j = 0; j < duelistCombat.CreatureCombats.Count; j++) {
            CreatureCombat creatureCombat = duelistCombat.CreatureCombats[j];
            EventBus.Instance.InvokeOnCreatureAttack(new CreatureAttackEventArgs(duelistCombat.Initiator, duelistCombat.Target, creatureCombat));
            if (creatureCombat.Defender == null)
                duelistCombat.Target.DamageLifePoints(creatureCombat.Attacker.GetAtk());
            else {
                CreatureDamagedByCreatureEventArgs args = new CreatureDamagedByCreatureEventArgs(duelistCombat.Initiator,
                                                                                                 duelistCombat.Target,
                                                                                                 creatureCombat,
                                                                                                 creatureCombat.Attacker.GetAtk());
                EventBus.Instance.InvokeOnCreatureDamagedByCreature(args);
                if (!args.IsCanceled)
                    creatureCombat.Defender.InflictDamage(creatureCombat.Attacker.GetAtk());

            }
        }
        InvokeOnDuelistCombatFinsihedClientRpc(duelistCombat.Initiator.PlayerId,
                                               duelistCombat.Target.PlayerId,
                                               duelistCombat.CreatureCombats.ToArray());
        duelistCombats.Remove(duelistCombat);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDuelistCombatFinsihedClientRpc(ulong initiatorId, ulong targetId, CreatureCombat[] creatureCombats) {        
        OnDuelistCombatFinsihed?.Invoke(this, new DuelistCombatEventArgs(initiatorId, targetId, new List<CreatureCombat>(creatureCombats)));
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
