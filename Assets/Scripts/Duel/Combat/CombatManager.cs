using System;
using System.Collections.Generic;
using Unity.Netcode;
using static UnityEngine.GraphicsBuffer;

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

    private void DeclareAttacker(object sender, DeclareAttackerEventArgs args) {
        if (!IsServer)
            return;

        AddAttacker(args.InitiatorId, args.TargetId, args.Attacker);
    }

    private void UndeclareAttacker(object sender, UndeclareAttackerEventArgs args) {
        if (!IsServer)
            return;

        RemoveAttacker(args.InitiatorId, args.TargetId, args.Attacker.Uuid);
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

    private void DeclareDefender(object sender, DeclareDefenderEventArgs args) {
        if (!IsServer)
            return;

        AddDefender(args.InitiatorId, args.TargetId, args.Attacker, args.Defender);
    }

    private void UndeclareDefender(object sender, UndeclareDefenderEventArgs args) {
        if (!IsServer)
            return;

        RemoveDefender(args.InitiatorId, args.TargetId, args.Defender.Uuid);
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
        for (int j = 0; j < duelistCombat.CreatureCombats.Count; j++) {
            CreatureCombat creatureCombat = duelistCombat.CreatureCombats[j];
            EventBus.Instance.InvokeOnCreatureAttack(new CreatureAttackEventArgs(duelistCombat.InitiatorId, duelistCombat.TargetId, creatureCombat));
            if (creatureCombat.Defender == null) {
                MatchPlayer target = duelManager.GetPlayerById(duelistCombat.TargetId);
                target.DamageLifePoints(creatureCombat.Attacker.GetAtk());
            }
            else {
                CreatureDamagedByCreatureEventArgs args = new CreatureDamagedByCreatureEventArgs(duelistCombat.InitiatorId,
                                                                                                 duelistCombat.TargetId,
                                                                                                 creatureCombat,
                                                                                                 creatureCombat.Attacker.GetAtk());
                EventBus.Instance.InvokeOnCreatureDamagedByCreature(args);
                if (!args.IsCanceled)
                    creatureCombat.Defender.InflictDamage(creatureCombat.Attacker.GetAtk());

            }
        }
        InvokeOnDuelistCombatFinsihedClientRpc(duelistCombat.InitiatorId,
                                               duelistCombat.TargetId,
                                               duelistCombat.CreatureCombats.ToArray());
        duelistCombats.Remove(duelistCombat);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDuelistCombatFinsihedClientRpc(ulong initiatorId, ulong targetId, CreatureCombat[] creatureCombats) {        
        OnDuelistCombatFinsihed?.Invoke(this, new DuelistCombatEventArgs(initiatorId, targetId, new List<CreatureCombat>(creatureCombats)));
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
