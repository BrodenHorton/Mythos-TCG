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

        ServiceLocator.Register(this);
    }

    private void Start() {
        if (!IsServer)
            return;

        duelManager = ServiceLocator.Get<DuelManager>();

        EventBus.Instance.OnDeclareAttacker += DeclareAttacker;
        EventBus.Instance.OnUndeclareAttacker += UndeclareAttacker;
        EventBus.Instance.OnDeclareDefender += DeclareDefender;
        EventBus.Instance.OnUndeclareDefender += UndeclareDefender;
    }

    private void DeclareAttacker(object sender, CombatCreatureEventArgs args) {
        if (!IsServer)
            return;

        AddAttacker(args.InitiatorId, args.TargetId, args.Card);
    }

    private void UndeclareAttacker(object sender, CombatCreatureEventArgs args) {
        if (!IsServer)
            return;

        RemoveAttacker(args.InitiatorId, args.TargetId, args.Card.Uuid);
    }

    public void AddAttacker(ulong initiatorId, ulong targetId, CreatureCard card) {
        if (!IsServer)
            return;

        if(HasExistingDuelistCombat(initiatorId, targetId)) {
            DuelistCombat combat = GetDuelistCombat(initiatorId, targetId);
            combat.AddAttacker(card);
        }
        else {
            DuelistCombat combat = new DuelistCombat(initiatorId, targetId);
            combat.AddAttacker(card);
            InsertCombat(combat);
        }
    }

    private void DeclareDefender(object sender, CreatureCombatEventArgs args) {
        if (!IsServer)
            return;

        AddDefender(args.InitiatorId, args.TargetId, args.Attacker, args.Defender);
    }

    private void UndeclareDefender(object sender, CombatCreatureEventArgs args) {
        if (!IsServer)
            return;

        RemoveDefender(args.InitiatorId, args.TargetId, args.Card.Uuid);
    }

    public void AddDefender(ulong initiatorId, ulong targetId, CreatureCard attacker, CreatureCard defender) {
        if (!IsServer)
            return;
        if (!HasExistingDuelistCombat(initiatorId, targetId))
            throw new Exception("Attempted to add a defender when there is no existing DuelistCombat between initiator and target");

        GetDuelistCombat(initiatorId, targetId).AddDefender(attacker.Uuid, defender);
    }

    public void RemoveAttacker(ulong initiatorId, ulong targetId, Guid attackerUuid) {
        if (!IsServer)
            return;
        if (!HasExistingDuelistCombat(initiatorId, targetId))
            throw new Exception("Attempted to remove an attacker when there is not a DuellistCombat between the initiator and target");

        DuelistCombat combat = GetDuelistCombat(initiatorId, targetId);
        combat.RemoveAttacker(attackerUuid);
        if(combat.CreatureCombats.Count == 0)
            duelistCombats.Remove(combat);
    }

    public void RemoveDefender(ulong initiatorId, ulong targetId, Guid defenderUuid) {
        if (!IsServer)
            return;
        if (!HasExistingDuelistCombat(initiatorId, targetId))
            throw new Exception("Attempted to remove a defender when there is not a DuellistCombat between the initiator and target");

        GetDuelistCombat(initiatorId, targetId).RemoveDefender(defenderUuid);
    }

    public void ProcessNextDuelistCombat() {
        if (!IsServer)
            return;
        if (duelistCombats.Count == 0)
            throw new Exception("Attempting to process the next DuelistCombat when the duelistCombats list is empty");

        DuelistCombat duelistCombat = duelistCombats[0];
        int damage = 0;
        for (int i = 0; i < duelistCombat.CreatureCombats.Count; i++) {
            CreatureCombat creatureCombat = duelistCombat.CreatureCombats[i];
            // TODO: Change this event so it returns the attack damage
            EventBus.Instance.InvokeOnCreatureAttack(new CreatureCombatEventArgs(duelistCombat.InitiatorId, duelistCombat.TargetId, creatureCombat));
            damage = creatureCombat.Attacker.GetAtk();
            if (creatureCombat.Defender == null) {
                MatchPlayer target = duelManager.GetPlayerById(duelistCombat.TargetId);
                target.DamageLifePoints(damage);
            }
            else {
                CreatureCombatDamageEventArgs args = new CreatureCombatDamageEventArgs(duelistCombat.InitiatorId,
                                                                                                 duelistCombat.TargetId,
                                                                                                 creatureCombat,
                                                                                                 ref damage);
                EventBus.Instance.InvokeOnCreatureDamagedByCreature(args);
                if (!args.IsCanceled)
                    creatureCombat.Defender.InflictDamage(damage);

            }
            EventBus.Instance.InvokeOnCreatureCombatFinished(new CreatureCombatDamageEventArgs(duelistCombat.InitiatorId,
                                                                                               duelistCombat.TargetId,
                                                                                               creatureCombat,
                                                                                               ref damage));
            creatureCombat.Attacker?.Tap();
        }
        CreatureCombatPayload[] creatureCombatPayloads = new CreatureCombatPayload[duelistCombat.CreatureCombats.Count];
        for(int i = 0; i < duelistCombat.CreatureCombats.Count; i++)
            creatureCombatPayloads[i] = new CreatureCombatPayload(duelistCombat.CreatureCombats[i]);
        InvokeOnDuelistCombatFinsihedClientRpc(duelistCombat.InitiatorId, duelistCombat.TargetId, creatureCombatPayloads);
        duelistCombats.Remove(duelistCombat);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDuelistCombatFinsihedClientRpc(ulong initiatorId, ulong targetId, CreatureCombatPayload[] creatureCombatPayloads) {        
        OnDuelistCombatFinsihed?.Invoke(this, new DuelistCombatEventArgs(initiatorId, targetId, new List<CreatureCombatPayload>(creatureCombatPayloads)));
    }

    private void InsertCombat(DuelistCombat combat) {
        int playerIndex = duelManager.GetPlayerIndex(combat.InitiatorId);
        for (int i = 0; i < duelistCombats.Count; i++) {
            if (playerIndex < duelManager.GetPlayerIndex(duelistCombats[i].InitiatorId)) {
                duelistCombats[i] = combat;
                return;
            }
        }

        duelistCombats.Add(combat);
    }

    private bool HasExistingDuelistCombat(ulong initiatorId, ulong targetId) {
        foreach(DuelistCombat combat in duelistCombats) {
            if (combat.InitiatorId == initiatorId && combat.TargetId == targetId)
                return true;
        }

        return false;
    }

    private DuelistCombat GetDuelistCombat(ulong initiatorId, ulong targetId) {
        foreach (DuelistCombat combat in duelistCombats) {
            if (combat.InitiatorId == initiatorId && combat.TargetId == targetId)
                return combat;
        }

        return null;
    }

    public void ClearCombats() {
        duelistCombats.Clear();
    }

    public List<MatchPlayer> GetTargets() {
        List<MatchPlayer> targets = new List<MatchPlayer>();
        foreach(DuelistCombat combat in duelistCombats) {
            MatchPlayer target = duelManager.GetPlayerById(combat.TargetId);
            if (!targets.Contains(target))
                targets.Add(target);
        }

        return targets;
    }

    public bool IsCreatureInCombat(Guid creatureUuid) {
        foreach (DuelistCombat duelistCombat in duelistCombats) {
            foreach (CreatureCombat creatureCombat in duelistCombat.CreatureCombats) {
                if (creatureCombat.Attacker.Uuid == creatureUuid || (creatureCombat.Defender != null && creatureCombat.Defender.Uuid == creatureUuid))
                    return true;
            }
        }

        return false;
    }

    public List<DuelistCombat> DuelistCombats { get { return duelistCombats; } }
}
