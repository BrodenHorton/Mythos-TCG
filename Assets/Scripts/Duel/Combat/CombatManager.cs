using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

public class CombatManager : NetworkBehaviour {
    public event EventHandler<DuelistCombatEventArgs> OnDuelistCombatFinsihed;

    private DuelManager duelManager;
    private CombatStateManager combatStateManager;
    private List<DuelistCombat> duelistCombats;

    private void Awake() {
        duelistCombats = new List<DuelistCombat>();

        ServiceLocator.Register(this);
    }

    private void Start() {
        duelManager = ServiceLocator.Get<DuelManager>();
        combatStateManager = ServiceLocator.Get<CombatStateManager>();

        EventBus.Instance.OnReleaseCreatureFieldCardOverCombatArea += DeclareAttacker;
        FieldCardSelectionManager.Instance.OnCreatureReleasedOverCreature += PlayerSelectDeclareDefender;
    }

    private void DeclareAttacker(object sender, CombatFieldCardEventArgs<CreatureFieldCardUI> args) {
        DeclareAttackerServerRpc(args.CombatFieldUI.TargetPlayerId, args.CardUI.CardUuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void DeclareAttackerServerRpc(ulong targetId, FixedString128Bytes cardUuidStr, RpcParams rpcParams = default) {
        MatchPlayer initiator = duelManager.GetPlayerById(rpcParams.Receive.SenderClientId);
        Guid cardUuid = Guid.Parse(cardUuidStr.ToString());
        if (!combatStateManager.CurrentState.CanDeclareAttackers())
            return;
        if (!initiator.ContainsCreatureUuid(cardUuid))
            return;
        CreatureCard creatureCard = initiator.GetCreatureByUuid(cardUuid);
        if (creatureCard == null)
            return;
        if (!creatureCard.CanAttack())
            return;

        CombatCreatureEventArgs combatCreatureEventArgs = new CombatCreatureEventArgs(initiator.PlayerId, targetId, creatureCard);
        EventBus.Instance.InvokeOnDeclareAttacker(combatCreatureEventArgs);
        AddAttacker(initiator.PlayerId, targetId, creatureCard);
        InvokeOnDeclareAttackerPayloadClientRpc(initiator.PlayerId, targetId, new CreatureCardPayload(creatureCard));
        EventBus.Instance.InvokeOnPostDeclareAttacker(combatCreatureEventArgs);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDeclareAttackerPayloadClientRpc(ulong initiatorId, ulong targetId, CreatureCardPayload cardPayload) {
        EventBus.Instance.InvokeOnDeclareAttackerFinished(new CombatCreaturePayloadEventArgs(initiatorId, targetId, cardPayload));
    }

    private void PlayerSelectDeclareDefender(object sender, CreatureReleasedOverCreatureEventArgs args) {
        if (!IsServer)
            throw new Exception("Only the server can call the method DeclareDefender");

        TcgLogger.Log("Player Select Declare Defender called");
        if (combatStateManager.CurrentState.CanDeclareDefenders())
            DeclareDefender(args.DraggingPlayerId, args.HoveredCard, args.HeldCard);
    }

    public void DeclareDefender(ulong targetId, CreatureCard attacker, CreatureCard defender) {
        if (!IsServer)
            throw new Exception("Only the server can call the method DeclareDefender");
        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
        MatchPlayer target = duelManager.GetPlayerById(targetId);
        if (!target.ContainsCreatureUuid(defender.Uuid))
            return;
        if (!defender.CanDefend())
            return;
        if (!initiator.ContainsCreatureUuid(attacker.Uuid))
            return;

        CanDefendEventArgs args = new CanDefendEventArgs(initiator.PlayerId, targetId, attacker, defender);
        EventBus.Instance.InvokeOnSelectAttackerToDefend(args);
        if (!args.CanDefend)
            return;

        CreatureCombatEventArgs creatureCombatEventArgs = new CreatureCombatEventArgs(initiator.PlayerId, targetId, attacker, defender);
        EventBus.Instance.InvokeOnDeclareDefender(creatureCombatEventArgs);
        AddDefender(initiator.PlayerId, targetId, attacker, defender);
        InvokeOnDeclareDefenderPayloadClientRpc(initiator.PlayerId, targetId, new CreatureCardPayload(attacker), new CreatureCardPayload(defender));
        EventBus.Instance.InvokeOnPostDeclareDefender(creatureCombatEventArgs);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDeclareDefenderPayloadClientRpc(ulong initiatorId, ulong targetId, CreatureCardPayload attacker, CreatureCardPayload defender) {
        EventBus.Instance.InvokeOnDeclareDefenderFinished(new CreatureCombatPayloadEventArgs(initiatorId, targetId, attacker, defender));
    }

    public void UndeclareAttacker(ulong targetId, Guid creatureCardUuid) {
        if (!IsServer)
            throw new Exception("Only the server can call the method UndeclareAttacker");
        if (!combatStateManager.CurrentState.CanDeclareAttackers())
            return;
        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
        if (!initiator.ContainsCreatureUuid(creatureCardUuid))
            return;
        CreatureCard creatureCard = initiator.GetCreatureByUuid(creatureCardUuid);
        if (creatureCard == null)
            throw new Exception("Unable to undeclare attacker since attacking creature is null");
        CreatureCombat creatureCombat = GetCreatureCombat(creatureCardUuid);
        if (creatureCombat == null)
            throw new Exception("Unable to find Creature Combat with creature uuid: " + creatureCardUuid);

        if (creatureCombat.Defender != null)
            UndeclareDefender(targetId, creatureCombat.Defender.Uuid);

        CombatCreatureEventArgs combatCreatureEventArgs = new CombatCreatureEventArgs(initiator.PlayerId, targetId, creatureCard);
        EventBus.Instance.InvokeOnUndelcareAttacker(combatCreatureEventArgs);
        RemoveAttacker(initiator.PlayerId, targetId, creatureCardUuid);
        InvokeOnUndeclareAttackerFinishedClientRpc(initiator.PlayerId, targetId, new CreatureCardPayload(creatureCard));
        EventBus.Instance.InvokeOnPostUndeclareAttacker(combatCreatureEventArgs);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnUndeclareAttackerFinishedClientRpc(ulong initiatorId, ulong targetId, CreatureCardPayload card) {
        EventBus.Instance.InvokeOnUndelcareAttackerFinished(new CombatCreaturePayloadEventArgs(initiatorId, targetId, card));
    }

    public void PlayerSelectUndeclareDefender(ulong targetId, Guid creatureCardUuid) {
        if (!IsServer)
            throw new Exception("Only the server can call the method PlayerUndeclareDefender");
        if (targetId == duelManager.GetCurrentPlayerTurn().PlayerId)
            return;
        if (!combatStateManager.CurrentState.CanDeclareDefenders())
            return;

        UndeclareDefender(targetId, creatureCardUuid);
    }

    public void UndeclareDefender(ulong targetId, Guid creatureCardUuid) {
        if (!IsServer)
            throw new Exception("Only the server can call the method UndeclareDefender");
        MatchPlayer target = duelManager.GetPlayerById(targetId);
        if (!target.ContainsCreatureUuid(creatureCardUuid))
            return;
        CreatureCard defender = target.GetCreatureByUuid(creatureCardUuid);
        if (defender == null)
            throw new Exception("Unable to undeclare defender since defending creature is null");

        ulong initiatorId = duelManager.GetCurrentPlayerTurn().PlayerId;
        CombatCreatureEventArgs combatCreatureEventArgs = new CombatCreatureEventArgs(initiatorId, targetId, defender);
        EventBus.Instance.InvokeOnUndeclareDefender(combatCreatureEventArgs);
        RemoveDefender(initiatorId, targetId, creatureCardUuid);
        InvokeOnUndeclareDefenderFinishedClientRpc(initiatorId, targetId, new CreatureCardPayload(defender));
        EventBus.Instance.InvokeOnPostUndeclareDefender(combatCreatureEventArgs);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnUndeclareDefenderFinishedClientRpc(ulong initiatorId, ulong targetId, CreatureCardPayload defender) {
        EventBus.Instance.InvokeOnUndeclareDefenderFinished(new CombatCreaturePayloadEventArgs(initiatorId, targetId, defender));
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
        for (int i = 0; i < duelistCombat.CreatureCombats.Count; i++) {
            CreatureCombat creatureCombat = duelistCombat.CreatureCombats[i];
            // TODO: Change this event so it returns the attack damage
            EventBus.Instance.InvokeOnCreatureAttack(new CreatureCombatEventArgs(duelistCombat.InitiatorId, duelistCombat.TargetId, creatureCombat));
            int damage = creatureCombat.Attacker.GetAtk();
            if (creatureCombat.Defender == null) {
                MatchPlayer target = duelManager.GetPlayerById(duelistCombat.TargetId);
                target.ModifyLifePoints(-damage);
            }
            else {
                CreatureCombatDamageEventArgs args = new CreatureCombatDamageEventArgs(duelistCombat.InitiatorId,
                                                                                       duelistCombat.TargetId,
                                                                                       creatureCombat,
                                                                                       damage);
                EventBus.Instance.InvokeOnCreatureDamagedByCreature(args);
                damage = args.Damage;
                if (!args.IsCanceled) {
                    if(args.ShouldDamageDefender)
                        creatureCombat.Defender.InflictDamage(damage);
                    if(args.DirectDamage > 0) {
                        MatchPlayer target = duelManager.GetPlayerById(duelistCombat.TargetId);
                        target.ModifyLifePoints(-args.DirectDamage);
                    }
                    EventBus.Instance.InvokeOnCreatureDamagedByCreatureFinished(args);
                }
            }
            EventBus.Instance.InvokeOnCreatureCombatFinished(new CreatureCombatDamageEventArgs(duelistCombat.InitiatorId,
                                                                                               duelistCombat.TargetId,
                                                                                               creatureCombat,
                                                                                               damage));
            CreatureCardPayload defender = creatureCombat.Defender != null ? new CreatureCardPayload(creatureCombat.Defender) : null;
            EventBus.Instance.InvokeOnPostCreatureCombatClientRpc(new CreatureCombatNetworkContainer(duelistCombat.InitiatorId,
                                                                                                     duelistCombat.TargetId,
                                                                                                     new CreatureCardPayload(creatureCombat.Attacker),
                                                                                                     defender));
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

    public bool HasExistingDuelistCombat(ulong initiatorId, ulong targetId) {
        foreach(DuelistCombat combat in duelistCombats) {
            if (combat.InitiatorId == initiatorId && combat.TargetId == targetId)
                return true;
        }

        return false;
    }

    public DuelistCombat GetDuelistCombat(ulong initiatorId, ulong targetId) {
        foreach (DuelistCombat combat in duelistCombats) {
            if (combat.InitiatorId == initiatorId && combat.TargetId == targetId)
                return combat;
        }
        throw new Exception("Unable to find duelist combat with initiatorId: " + initiatorId + " and targetId: " + targetId);
    }

    public DuelistCombat GetDuelistCombat(Guid creatureUuid) {
        foreach (DuelistCombat duelistCombat in duelistCombats) {
            foreach (CreatureCombat creatureCombat in duelistCombat.CreatureCombats) {
                if (creatureCombat.Attacker.Uuid == creatureUuid || (creatureCombat.Defender != null && creatureCombat.Defender.Uuid == creatureUuid))
                    return duelistCombat;
            }
        }
        throw new Exception("Unable to find DuelistCombat that has a CreatureCombat with a creature uuid: " + creatureUuid);
    }

    public List<MatchPlayer> GetTargets() {
        List<MatchPlayer> targets = new List<MatchPlayer>();
        foreach (DuelistCombat combat in duelistCombats) {
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

    public CreatureCombat GetCreatureCombat(Guid creatureUuid) {
        foreach (DuelistCombat duelistCombat in duelistCombats) {
            foreach (CreatureCombat creatureCombat in duelistCombat.CreatureCombats) {
                if (creatureCombat.Attacker.Uuid == creatureUuid || (creatureCombat.Defender != null && creatureCombat.Defender.Uuid == creatureUuid))
                    return creatureCombat;
            }
        }
        throw new Exception("Unable to find CreatureCombat with the creature uuid: " + creatureUuid);
    }

    public List<DuelistCombat> DuelistCombats { get { return duelistCombats; } }
}
