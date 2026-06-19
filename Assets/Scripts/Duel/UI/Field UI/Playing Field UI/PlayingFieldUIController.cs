using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class PlayingFieldUIController : NetworkBehaviour {
    [SerializeField] protected PlayingFieldUI playingFieldUI;
    protected ulong playerId;
    protected DuelManager duelManager;
    protected DuelStateManager stateManager;
    protected CombatStateManager combatStateManager;
    protected CombatManager combatManager;
    protected ActionManager actionManager;

    protected virtual void Start() {
        duelManager = ServiceLocator.Get<DuelManager>();
        stateManager = ServiceLocator.Get<DuelStateManager>();
        combatStateManager = ServiceLocator.Get<CombatStateManager>();
        combatManager = ServiceLocator.Get<CombatManager>();
        actionManager = ServiceLocator.Get<ActionManager>();
    }

    public abstract void Init(ulong playerId);

    public abstract void PlayCreatureCard(CreatureCardPayload card);

    public abstract void PlayDomainCard(DomainCardPayload card);

    public abstract void AddCreatureCard(CreatureFieldCardUI card);
    
    public abstract void AddCreatureCards(List<CreatureFieldCardUI> creatures);

    public abstract void RemoveCreature(Guid cardUuid);

    public abstract CreatureFieldCardUI ReleaseCreature(Guid cardUuid);

    public abstract bool ContainsCreature(Guid uuid);
}