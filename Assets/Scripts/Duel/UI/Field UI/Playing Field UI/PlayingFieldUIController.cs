using System;
using System.Collections.Generic;
using Unity.Netcode;

public abstract class PlayingFieldUIController : NetworkBehaviour {
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

    public abstract void PlayCreatureCard(CreatureCard card);

    public abstract void PlayDomainCard(DomainCard card);

    public abstract void RemoveCreature(CreatureCard card);

    public abstract void UpdateCreatureFieldCard(CreatureCard card);

    public abstract void TapCreature(CreatureCard card);

    public abstract void UntapCreature(CreatureCard card);

    public abstract void GetCreatureCardsFromCombat(List<CreatureFieldCardUI> creatures);

    public abstract bool ContainsCreature(Guid uuid);
}